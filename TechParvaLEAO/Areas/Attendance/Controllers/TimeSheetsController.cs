using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Areas.Organization.Services;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Controllers;
using MediatR;
using CalendarUtilities;
using System.Globalization;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Leave.Services;

namespace Cotecna.Areas.Attendance.Controllers
{
    /*
     * Controller for Timesheets
     */
    [Area("Attendance")]
    public class TimeSheetsController : BaseViewController
    {
        private readonly IEmployeeServices employeeServices;
        private readonly TimeSheetServices timeSheetServices;
        private readonly IMediator mediator;
        private readonly IApplicationRepository repository;
        private readonly LocationWorkdaysService locationWorkdaysService;
        private readonly LeaveRequestServices leaveRequestServices;
        public TimeSheetsController(IApplicationRepository repository, IEmployeeServices employeeServices,
            TimeSheetServices timeSheetServices, LeaveRequestServices leaveRequestServices, 
            IMediator mediator, LocationWorkdaysService locationWorkdaysService)
        {
            this.employeeServices = employeeServices;
            this.timeSheetServices = timeSheetServices;
            this.mediator = mediator;
            this.repository = repository;
            this.locationWorkdaysService = locationWorkdaysService;
            this.leaveRequestServices = leaveRequestServices;
        }


        /*
         * Show List of own timesheets
         */
        // GET: Attendance/TimeSheets
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> Index(string timesheetMonth)
        {
            ViewData["CanApproveReject"] = false;
            var ownTimesheets = timeSheetServices.GetOwnTimeSheets(GetEmployee());
            var employees = await employeeServices.GetOwnEnumerable(User);
            var overview = CreateViewModel(timesheetMonth, ownTimesheets, employees);
            return View("Index", overview);
        }

        /*
         * Show List of timesheets for co-ordinator
         */
        [Authorize(Roles = AuthorizationRoles.TIMESHEET + "," + AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> OnBehalfTimesheets([Bind]string timesheetMonth)
        {
            var monthRange = GetMonthRange(timesheetMonth);
            var enteredTimesheets = timeSheetServices.GetOnBehalfTimeSheets(GetEmployee(),monthRange.StartDate, monthRange.EndDate);
            var employees = await employeeServices.GetOnFieldEmployeeForTimesheet(User);
            var overview = CreateViewModel(timesheetMonth, enteredTimesheets, employees);
            return View("Index", overview);
        }

        /*
         * Helper class to hold Start Date and End Date of the month
         */
        private class MonthRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        /*
         * Calculates first and last date of the given months
         */
        private MonthRange GetMonthRange(string month)
        {
            var date = DateTime.Today;
            if (!string.IsNullOrEmpty(month)){
                date = DateTime.Parse(month);
            }
            var firstDateOfMonth = date.GetFirstDayOfMonth();
            var lastDateOfMonth = date.GetLastDayOfMonth();
            return new MonthRange { StartDate=firstDateOfMonth, EndDate=lastDateOfMonth };
        }

        /*
         * Create view model for timehseets of the month which is shown as a grid
         */
        private TimeSheetOverviewViewModel CreateViewModel(string timesheetMonth, IEnumerable<TimeSheet> timesheets, IEnumerable<Employee> employees)
        {
            //First get timesheets entered for this month
            //Create a map to help assign to correct week
            var timesheetMap = new Dictionary<int, Dictionary<int, TimeSheet>>();
            foreach (var timesheet in timesheets)
            {
                if (timesheetMap.ContainsKey(timesheet.EmployeeId.Value))
                {
                    var timesheetMonthMap = timesheetMap[timesheet.EmployeeId.Value];
                    timesheetMonthMap[timesheet.WeekInMonth] = timesheet;
                }
                else
                {
                    var timesheetMonthMap = new Dictionary<int, TimeSheet>();
                    timesheetMonthMap[timesheet.WeekInMonth] = timesheet;
                    timesheetMap[timesheet.EmployeeId.Value] = timesheetMonthMap;
                }

            }

            ViewData["CanApproveReject"] = false;
            if (timesheetMonth == null || string.Equals("", timesheetMonth))
            {
                timesheetMonth = DateTime.Now.ToString("yyyy-MM");
            }
            ViewData["MonthYear"] = timesheetMonth;

            //Prepaer view model
            var overview = new TimeSheetOverviewViewModel();

            var timesheetMonthDt = DateTime.Parse(timesheetMonth);
            var timesheetDates = timesheetMonthDt.MondaysInMonth();

            var cal = CultureInfo.CurrentCulture.Calendar;

            foreach (var date in timesheetDates)
            {
                var week = new Week();
                week.StartDate = date;
                week.EndDate = date + TimeSpan.FromDays(6);
                int weekOfYear = cal.GetWeekOfYear(date, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
                week.WeekInYear = weekOfYear;
                overview.Weeks.Add(week);
            }

            //Get All Available Employees
            
            //For each employee start making entry in the view model
            foreach (var employee in employees)
            {
                //Monthly timesheet is the holding object that holds weekly timesheets for that week
                var monthlyTimesheet = new EmployeeTimeSheetMonth();
                monthlyTimesheet.Employee = employee.Name;
                monthlyTimesheet.EmployeeId = employee.Id;
                monthlyTimesheet.Designation = employee.Designation.Name;
                monthlyTimesheet.EmployeeCode = employee.EmployeeCode;

                overview.EmployeeTimeSheetMonth.Add(monthlyTimesheet);
                var weekNumber = 1;
                var employeeTsMap = null as Dictionary<int, TimeSheet>;
                if (timesheetMap.ContainsKey(employee.Id))
                {
                    employeeTsMap = timesheetMap[employee.Id];
                }

                foreach (var date in timesheetDates)
                {
                    var tsWeek = new TimeSheetWeek();
                    tsWeek.StartDate = date;
                    tsWeek.EndDate = date + TimeSpan.FromDays(6);
                    tsWeek.WeekInMonth = weekNumber;

                    if (employeeTsMap != null && employeeTsMap.ContainsKey(weekNumber))
                    {
                        var timesheet = employeeTsMap[weekNumber];
                        tsWeek.TimeSheetId = timesheet.Id;
                        tsWeek.OvertimeHours = timesheet.OvertimeHours;
                        tsWeek.CompOffs = timesheet.CompOffs;
                        tsWeek.Status = timesheet.Status;
                    }
                    monthlyTimesheet.TimeSheetWeeks.Add(tsWeek);
                    weekNumber++;
                }
            }
            return overview;
        }
        public async Task<ActionResult> TimesheetForApproval(string timesheetMonth)
        {
            ViewData["CanApproveReject"] = true;
            var monthRange = GetMonthRange(timesheetMonth);
            var ownTimesheets = timeSheetServices.GetForMyApprovalTimeSheets(GetEmployee(), monthRange.StartDate, monthRange.EndDate);
            var employees = await employeeServices.GetOnFieldSubordinates(User);
            var overview = CreateViewModel(timesheetMonth, ownTimesheets, employees);
            return View("Index", overview);
        }

        /*
         * Details Timesheet
         */
        // GET: Attendance/TimeSheets/Details/5
        [Authorize(Roles = AuthorizationRoles.TIMESHEET + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            TimeSheet timeSheet = repository.GetById<TimeSheet>(id);
            if (timeSheet == null)
            {
                return NotFound();
            }
            return View(timeSheet);
        }

        /*
         * Month Details
         */
        // GET: Attendance/TimeSheets/Details/5
        [Authorize(Roles = AuthorizationRoles.TIMESHEET + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult MonthDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var timeSheets = repository.GetAll<TimeSheet>();
            return View(timeSheets.ToList());
        }

        /*
         * Shows form to create a new timesheet.
         */
        // GET: Attendance/TimeSheets/Create
        [Authorize(Roles = AuthorizationRoles.TIMESHEET)]
        [HttpGet]
        public async Task<ActionResult> Create(string startDate, string endDate, int weekInMonth, int employeeId)
        {
            CultureInfo provider = CultureInfo.InvariantCulture;

            if (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR) || User.IsInRole(AuthorizationRoles.TIMESHEET))
            {
                ViewBag.EmployeeId = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name");
            }
            var timeSheetVm = new TimeSheetViewModel();
            timeSheetVm.StartDate = DateTime.ParseExact(startDate, "dd-MM-yyyy", provider);
            timeSheetVm.EndDate = DateTime.ParseExact(endDate, "dd-MM-yyyy", provider);
            timeSheetVm.EmployeeId = employeeId;
            timeSheetVm.WeekInMonth = weekInMonth;
            timeSheetVm.WeekNumber = timeSheetVm.StartDate.Year + "-W" + timeSheetVm.StartDate.GetWeekOfYear();
            if (ModelState.IsValid && timeSheetVm.TimeSheetEntries.Count() == 0 && weekInMonth!=0)
            {
                await InitializeNewTimesheetViewModel(timeSheetVm, employeeId);
                timeSheetVm.CreatedByEmployeeId = GetEmployee().Id;
                var result = await mediator.Send<int>(timeSheetVm);
                timeSheetVm.Id = result;
                return RedirectToAction("TimeSheetEdit", new {Id=result });
                //return RedirectToAction("OnBehalfTimesheets", new { timesheetMonth = timeSheetVm.StartDate.ToString("yyyy-MM") });
            }
            return View();
        }

        /*
         * Creates a new TimeSheet from submitted form data
         */
        // POST: Attendance/TimeSheets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.TIMESHEET)]
        public async Task<ActionResult> Create([Bind]TimeSheetViewModel timeSheetVm)
        {
            if (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR) || User.IsInRole(AuthorizationRoles.TIMESHEET))
            {
                ViewBag.EmployeeId = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name");
            }
            if (!ModelState.IsValid)
            {
                //InitializeGetTimesheetViewModel(timeSheetVm);
                return View(timeSheetVm);
            }

            if (ModelState.IsValid && timeSheetVm.TimeSheetEntries.Count() == 0)
            {
                await InitializeNewTimesheetViewModel(timeSheetVm, timeSheetVm.EmployeeId);
                return View(timeSheetVm);
            }
            else
            {
                timeSheetVm.CreatedByEmployeeId = GetEmployee().Id;
                var result = await mediator.Send<int>(timeSheetVm);
                if (result>0)
                {
                    return RedirectToAction("OnBehalfTimesheets",new { timesheetMonth = timeSheetVm.StartDate.ToString("yyyy-MM") });
                }
                else
                {
                    return View(timeSheetVm);
                }
            }
        }

        /*
         * Show form to edit TimeSheet
         */
        // GET: Attendance/TimeSheets/Create
        [Authorize(Roles = AuthorizationRoles.TIMESHEET)]
        [HttpGet]
        public async Task<ActionResult> TimeSheetEdit(int? Id)
        {
            TimeSheetEditViewModel timeSheetVm = new TimeSheetEditViewModel();
            if (Id.HasValue)
            {
                if (Id == null)
                {
                    return NotFound();
                }
                var previousTimesheet = await repository.GetByIdAsync<TimeSheet>(Id);
                timeSheetVm.TimesheetId = Id.Value;
                timeSheetVm.StartDate = previousTimesheet.StartDate;
                timeSheetVm.EndDate = previousTimesheet.EndDate;
                timeSheetVm.EmployeeId = previousTimesheet.EmployeeId.Value;
                timeSheetVm.WeekInMonth = previousTimesheet.WeekInMonth;
                timeSheetVm.WeekNumber = timeSheetVm.StartDate.Year + "-W" + timeSheetVm.StartDate.GetWeekOfYear();
                timeSheetVm.Designation = previousTimesheet.Employee.Designation.Name;
                timeSheetVm.EmployeeCode = previousTimesheet.Employee.EmployeeCode;
                timeSheetVm.Employee = previousTimesheet.Employee.Name;
                foreach (var attendance in previousTimesheet.AttendanceRecords)
                {
                    timeSheetVm.TimeSheetEntries.Add(new TimeSheetEntryViewModel
                    {
                        Weekday = attendance.WorkDate.ToString("dddd"),
                        TimesheetEntryId = attendance.Id,
                        Client = attendance.Client,
                        Date = attendance.WorkDate,
                        HolidayReason = attendance.HolidayReason,
                        JobNumber = attendance.Job,
                        InTime = attendance.TimeIn.ToString(@"hh\:mm"),
                        OutTime = attendance.TimeOut.ToString(@"hh\:mm"),
                        HoursWorked = attendance.WorkingTime.ToString(@"hh\:mm"),
                        IsWeekOff = attendance.IsWeekend,
                        IsHoliday = attendance.IsHoliday,
                        IsHalfDay = attendance.IsHalfDay,
                        IsLeave = attendance.IsLeave,
                        IsHalfDayLeave = attendance.IsHalfDayLeave
                    });
                }
            }
            await InitializeEditTimesheetViewModel(timeSheetVm);
            return View(timeSheetVm);
        }

        /*
         * Updates TimeSheet from submitted form data
         */
        // POST: Attendance/TimeSheets/Create
        [HttpPost]
        [Authorize(Roles = AuthorizationRoles.TIMESHEET)]
        public async Task<ActionResult> TimeSheetEdit([FromForm]TimeSheetEditViewModel timeSheetVm)
        {
            if (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR) || User.IsInRole(AuthorizationRoles.TIMESHEET))
            {
                ViewBag.EmployeeId = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name");
            }

            if ("Submit".Equals(timeSheetVm.Submit) && !ModelState.IsValid)
            {
                //InitializeGetTimesheetViewModel(timeSheetVm);
                return View(timeSheetVm);
            }

            timeSheetVm.CreatedByEmployeeId = GetEmployee().Id;
            var result = await mediator.Send<int>(timeSheetVm);
            if (result==1)
            {
                return RedirectToAction("OnBehalfTimesheets", new { timesheetMonth = timeSheetVm.StartDate.ToString("yyyy-MM") });
            }
            else
            {
                return View(timeSheetVm);
            }
        }

        private async Task<TimeSheetViewModel> InitializeNewTimesheetViewModel(TimeSheetViewModel timeSheetVm, int employeeId)
        {
            var weeknumber = Int32.Parse(timeSheetVm.WeekNumber.Substring(timeSheetVm.WeekNumber.IndexOf('-') + 2));
            var year = Int32.Parse(timeSheetVm.WeekNumber.Substring(0, timeSheetVm.WeekNumber.IndexOf('-')));
            var firstDayOfWeek = timeSheetVm.StartDate;// DateTimeUtils.FirstDateOfWeekISO8601(year, weeknumber);
            var weekDays = DateTimeUtils.GetDatesFrom(firstDayOfWeek, 7);
            var timeSheetEntries = new List<TimeSheetEntryViewModel>();

            var employee = repository.GetById<Employee>(employeeId);
            var locationCalendar = null as IList<LocationCalendar>;

            if (employee.SpecificWeeklyOff)
            {
                locationCalendar = locationWorkdaysService.GetEmployeeCalendar(
                            GetEmployee().LocationId.Value, employeeId, weekDays.First(), weekDays.Last());
            }
            else
            {
                locationCalendar = locationWorkdaysService.GetLocationCalendar(
                        GetEmployee().LocationId.Value, weekDays.First(), weekDays.Last());
            }

            await leaveRequestServices.UpdateEmployeeLeaves(locationCalendar, employeeId, weekDays.First(), weekDays.Last());

            var locationCalendarMap = new Dictionary<DateTime, LocationCalendar>();

            
            foreach (var calendarDay in locationCalendar)
            {
                locationCalendarMap[calendarDay.Date] = calendarDay;
            }

            foreach (var date in weekDays)
            {
                var calendar = locationCalendarMap[date];
                timeSheetEntries.Add(new TimeSheetEntryViewModel
                {
                    Date = date,
                    Weekday = date.DayOfWeek.ToString(),
                    IsHoliday = calendar.IsHoliday,
                    IsWeekOff = calendar.IsWeekOff,                    
                    IsHalfDay = calendar.IsHalfDay,
                    IsLeave = calendar.IsLeave,
                    IsHalfDayLeave = calendar.IsHalfDayLeave
                });
            }
            timeSheetVm.TimeSheetEntries = timeSheetEntries;
            return timeSheetVm;
        }

        private async Task<TimeSheetViewModel> InitializeEditTimesheetViewModel(TimeSheetViewModel timeSheetVm)
        {
            var employeeId = timeSheetVm.EmployeeId;
            var weeknumber = Int32.Parse(timeSheetVm.WeekNumber.Substring(timeSheetVm.WeekNumber.IndexOf('-') + 2));
            var year = Int32.Parse(timeSheetVm.WeekNumber.Substring(0, timeSheetVm.WeekNumber.IndexOf('-')));
            var firstDayOfWeek = timeSheetVm.StartDate; //DateTimeUtils.FirstDateOfWeekISO8601(year, weeknumber);
            var weekDays = DateTimeUtils.GetDatesFrom(firstDayOfWeek, 7);
            var timeSheetEntries = timeSheetVm.TimeSheetEntries;

            var employee = repository.GetById<Employee>(employeeId);
            var locationCalendar = null as IList<LocationCalendar>;

            if (employee.SpecificWeeklyOff)
            {
                locationCalendar = locationWorkdaysService.GetEmployeeCalendar(
                            GetEmployee().LocationId.Value, employeeId, weekDays.First(), weekDays.Last());
            }
            else
            {
                locationCalendar = locationWorkdaysService.GetLocationCalendar(
                        GetEmployee().LocationId.Value, weekDays.First(), weekDays.Last());
            }

            await leaveRequestServices.UpdateEmployeeLeaves(locationCalendar, employeeId, weekDays.First(), weekDays.Last());

            var locationCalendarMap = new Dictionary<DateTime, LocationCalendar>();


            foreach (var calendarDay in locationCalendar)
            {
                locationCalendarMap[calendarDay.Date] = calendarDay;
            }

            foreach (var date in weekDays)
            {
                var calendar = locationCalendarMap[date];
                var entry = GetEntryForDate(timeSheetEntries, date);
                entry.IsHoliday = calendar.IsHoliday;
                entry.IsWeekOff = calendar.IsWeekOff;
                entry.IsHalfDay = calendar.IsHalfDay;
                entry.IsLeave = calendar.IsLeave;
                entry.IsHalfDayLeave = calendar.IsHalfDayLeave;
            }
            return timeSheetVm;
        }

        private TimeSheetEntryViewModel GetEntryForDate(List<TimeSheetEntryViewModel> entries, DateTime dateTime)
        {
            foreach(var entry in entries)
            {
                if (entry.Date.Equals(dateTime))
                {
                    return entry;
                }
            }
            return null;
        }

        /*
         * Shows form to confirm approve timesheet
         */
        [Authorize(Roles = AuthorizationRoles.MANAGER)]
        [HttpGet]
        public async Task<ActionResult> Approve(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var timesheet = repository.GetById<TimeSheet>(id);
            var vm = new TimeSheetApproveRejectViewModel
            {
                Id = id,
                RejecttionReason = 0,
                ApproveReject = "Approve",
                ApprovedById = GetEmployee().Id
            };
            var result = await mediator.Send(vm);
            return RedirectToAction("TimesheetForApproval", new { timesheetMonth = timesheet.StartDate.ToString("yyyy-MM") });
        }

        /*
         * Approved timesheet
         */
        [Authorize(Roles = AuthorizationRoles.MANAGER)]
        [HttpPost]
        public async Task<ActionResult> Approve(int[] id)
        {
            if (id == null)
            {
                return NotFound();
            }
            if (id.Count() > 0)
            {
                var timesheet = repository.GetById<TimeSheet>(id[id.Count() - 1]);
                foreach (int tsid in id)
                {
                    var vm = new TimeSheetApproveRejectViewModel
                    {
                        Id = tsid,
                        RejecttionReason = 0,
                        ApproveReject = "Approve",
                        ApprovedById = GetEmployee().Id
                    };
                    var result = await mediator.Send(vm);
                }
                return RedirectToAction("TimesheetForApproval", new { timesheetMonth = timesheet.StartDate.ToString("yyyy-MM") });
            }
            else
            {
                return RedirectToAction("TimesheetForApproval", new { timesheetMonth = DateTime.Now.ToString("yyyy-MM") });
            }
        }


        /*
         * Shows form to reject timesheet
         */
        public ActionResult Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            TimeSheet timeSheet = repository.GetById<TimeSheet>(id);
            if (timeSheet == null)
            {
                return NotFound();
            }
            
            return View(timeSheet);
        }

        /*
          * Reject timesheet
          */
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.MANAGER)]
        public async Task<ActionResult> Reject(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }
            var vm = new TimeSheetApproveRejectViewModel
            {
                Id = id,
                RejecttionReason = 0,
                ApproveReject = "Reject",
                ApprovedById = GetEmployee().Id
            };
            var result = await mediator.Send(vm);
            return RedirectToAction("TimesheetForApproval");
        }

        /*
          * Timesheet Detail
          */
        public ActionResult TimesheetDetail(int? id)
        {
            if (User.IsInRole(AuthorizationRoles.MANAGER))
            {
                ViewData["CanApproveReject"] = true;
            }
            else
            {
                ViewData["CanApproveReject"] = false;
            }

            if (id == null)
            {
                return NotFound();
            }
            TimeSheet timeSheet = repository.GetById<TimeSheet>(id);
            if (timeSheet.TimesheetCreatedById == GetEmployee().Id)
            {
                ViewData["CanEditTimesheet"] = true;
            }
            else
            {
                ViewData["CanEditTimesheet"] = false;
            }
            if (timeSheet == null)
            {
                return NotFound();
            }
            return View(timeSheet);
        }

        /*
          * Employee Detail
          */
        public ActionResult EmployeeDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee employee = repository.GetById<Employee>(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

    }
}
