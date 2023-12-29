using TechParvaLEAO.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using SmartBreadcrumbs.Attributes;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Attendance.Models;
using System.Globalization;
using CalendarUtilities;
using TechParvaLEAO.Areas.Organization.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Services;

namespace Cotecna.Controllers
{
    public class HomeController : BaseViewController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly PaymentRequestService paymentRequestService;
        private readonly LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices;
        private readonly TimeSheetServices timeSheetServices;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IEmployeeServices employeeServices;

      

        public HomeController(UserManager<ApplicationUser> userManager,
            PaymentRequestService paymentRequestService,
            TimeSheetServices timeSheetServices,
            LeaveRequestServices leaveRequestServices,
            SignInManager<ApplicationUser> signInManager,
            LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices,
            IEmployeeServices employeeServices)
        {
            this.userManager = userManager;
            this.paymentRequestService = paymentRequestService;
            this.timeSheetServices = timeSheetServices;
            this.leaveRequestServices = leaveRequestServices;
            this.signInManager = signInManager;
            this.leaveCreditAndUtilizationServices = leaveCreditAndUtilizationServices;
            this.employeeServices = employeeServices;
        }

        [Authorize(Roles=AuthorizationRoles.MANAGER)]
        public async Task<ActionResult> ManagerDashboard(bool IncludeDeacivatedEmployees)
        {
            var viewModel = await BuildManagerDashboard(IncludeDeacivatedEmployees);
            return View("ManagerDashboard", viewModel);
        }
         
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult FinanceDashboard()
        {
            var viewModel = BuildFinanceDashboard();
            return View("FinanceDashboard", viewModel);
        }

        [Authorize(Roles = AuthorizationRoles.HR)]
        public ActionResult HRDashboard()
        {
            var viewModel = BuildHRDashboard();
            return View("HRDashboard", viewModel);
        }

        [Authorize(Roles = AuthorizationRoles.LOCATION_COORDINATOR)]
        public ActionResult LocationCoordinatorDashboard()
        {
            var viewModel = BuildCoordinatorDashboard();
            return View("CoOrdinatorDashboard", viewModel);
        }
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public ActionResult EmployeeDashboard()
        {
            var viewModel = BuildEmployeeDashboard();
            return View("EmployeeDashboard", viewModel);
        }

        [Authorize]
        [DefaultBreadcrumb("Home")]
        public ActionResult Index()
        {
            return RedirectToAction("EmployeeDashboard");
        }

        private DashboardViewModel BuildEmployeeDashboard()
        {
           
            var dashboardViewModel = new DashboardViewModel();            
            var employeeExpenses = paymentRequestService.
                GetOwnExpenseDashboard(GetEmployee())
                .Select(e => new ExpenseListItemsViewModel
                {
                    Amount = e.Amount,
                    AppliedBy = e.PaymentRequestCreatedBy.Name,
                    AppliedFor = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var employeeAdvances = paymentRequestService.GetOwnAdvanceDashboard(GetEmployee())
                .Select(e => new AdvanceListItemsViewModel
                {
                    Amount = e.Amount,
                    AppliedBy = e.PaymentRequestCreatedBy.Name,
                    AppliedFor = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();


            var employeeLeaves = leaveRequestServices.GetOwnLeaves(GetEmployee(), null)
                .Select(e => new LeaveItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    ApplyDate = e.LeaveRequestCreatedDate,
                    TotalDays = e.NumberOfDays,
                    BalanceLeaves = e.GetLeaveBalance(),
                    Status = e.Status,
                    LeaveType = e.LeaveType?.Name,
                    Location = e.Employee.Location.Name,
                }).ToList();

            var employeeTimeSheets = timeSheetServices.GetOwnTimeSheets(GetEmployee())
                .Select(e => new TimesheetItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    Designation = e.Employee.Designation.Name,
                    WorkingHours = e.TotalWorkHoursTime(),
                    OvertimeHours = e.OvertimeHoursTS(),
                }).ToList();

            dashboardViewModel.ExpenseListItems = employeeExpenses;
            dashboardViewModel.AdvanceListItems = employeeAdvances;
            dashboardViewModel.LeaveItems = employeeLeaves;
            dashboardViewModel.TimesheetItems = employeeTimeSheets;
            dashboardViewModel.CompOffAvailable = leaveRequestServices.GetAvailableCompOffsCount(GetEmployee());
            dashboardViewModel.ExpenseReimbursementUnpaid = paymentRequestService.GetUnsettledExpensePaymentRequests(GetEmployee(), null).Count();
            dashboardViewModel.AdvanceRequestPending = paymentRequestService.GetUnsettledPaymentRequests(GetEmployee(), null).Count();
            dashboardViewModel.LeaveBalance = leaveRequestServices.GetAnnaulLeaveEligibility(GetEmployee()) 
                + leaveRequestServices.GetAnnaulLeavesCarryForward(GetEmployee())  
                - leaveRequestServices.GetAnnaulLeaveUtilized(GetEmployee());


            //var viewModel  = new NewLeaveViewModel();

            //viewModel.AnnualLeaveEligibility = leaveRequestServices.GetAnnaulLeaveEligibility(GetEmployee());
            //viewModel.LeavesCarryForward = leaveRequestServices.GetAnnaulLeavesCarryForward(GetEmployee());


            ////var prev_year = Convert.ToDateTime("12/31/"+ (DateTime.Today.Year - 1));
            //var prev_year = Convert.ToDateTime((DateTime.Today.Year - 1) + "/12/31");
            //var prev_utilize = leaveRequestServices.GetAnnaulLeaveUtilized(GetEmployee(), prev_year);

            //if (viewModel.LeavesCarryForward > 0)
            //{
            //    viewModel.LeavesCarryForward = viewModel.LeavesCarryForward - (viewModel.AnnualLeaveEligibility - prev_utilize);
            //}


            //viewModel.AnnualLeaves = viewModel.AnnualLeaveEligibility + viewModel.LeavesCarryForward;
            //viewModel.LeavesUtilized = leaveRequestServices.GetAnnaulLeaveUtilized(GetEmployee());
            //viewModel.LeavesPendingApproval = leaveRequestServices.GetAnnaulLeavePending(GetEmployee());
            //viewModel.LeavesWithoutPay = leaveRequestServices.GetLWPDays(GetEmployee());
            //viewModel.LeaveBalance = viewModel.AnnualLeaves - viewModel.LeavesUtilized;


            //dashboardViewModel.LeaveBalance = viewModel.LeaveBalance;



            return dashboardViewModel;
        }

        /*
         * Calculates first and last date of the given months
         */
        private class MonthRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        private MonthRange GetMonthRange(string month)
        {
            var date = DateTime.Today;
            if (!string.IsNullOrEmpty(month))
            {
                date = DateTime.Parse(month);
            }
            var firstDateOfMonth = date.GetFirstDayOfMonth();
            var lastDateOfMonth = date.GetLastDayOfMonth();
            return new MonthRange { StartDate = firstDateOfMonth, EndDate = lastDateOfMonth };
        }

        private async Task<DashboardViewModel> BuildManagerDashboard(bool IncludeDeacivatedEmployees)
        {

            PaymentRequestSearchViewModel data1 = new PaymentRequestSearchViewModel
            {
                IncludeDeacivatedEmployees = IncludeDeacivatedEmployees

            };
            LeaveRequestSearchViewModel data2 = new LeaveRequestSearchViewModel
            {
                IncludeDeacivatedEmployees = IncludeDeacivatedEmployees
            };
            var dashboardViewModel = new DashboardViewModel();
            //Manager
            var managerExpenses = paymentRequestService.GetForMyApprovalExpenseDashboard(GetEmployee(), data1)
                .Select(e => new ExpenseListItemsViewModel
                {
                    Amount = e.Amount,
                    EmployeeName = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Role = e.Employee.Designation.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var manageradvances = paymentRequestService.GetForMyApprovalAdvanceDashboard(GetEmployee(),data1)
                .Select(e => new AdvanceListItemsViewModel
                {
                    Amount = e.Amount,
                    EmployeeName = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Role = e.Employee.Designation.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();
            
            var managerLeaves = leaveRequestServices.GetForMyApprovalLeaves(GetEmployee(), data2)
                .Select(e => new LeaveItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    ApplyDate = e.LeaveRequestCreatedDate,
                    TotalDays = e.NumberOfDays,
                    BalanceLeaves = e.GetLeaveBalance(),
                    Status = e.Status,
                    LeaveType = e.LeaveType?.Name,
                    Location = e.Employee.Location.Name,
                }).ToList();

            var employees = await employeeServices.GetOnFieldSubordinates(User);
            var timesheetMonth = DateTime.Today.ToString("MM/yyyy");
            var monthRange = GetMonthRange(timesheetMonth);
            var ownTimesheets = timeSheetServices.GetForMyApprovalTimeSheets(GetEmployee(), monthRange.StartDate, monthRange.EndDate);
            var overview = CreateViewModel(timesheetMonth, ownTimesheets, employees);

            dashboardViewModel.ExpenseListItems = managerExpenses;
            dashboardViewModel.AdvanceListItems = manageradvances;
            dashboardViewModel.LeaveItems = managerLeaves;
            dashboardViewModel.ManagerTimesheets = overview;
            dashboardViewModel.EmployeeLeaveStatus = leaveRequestServices.GetForReportingToLeavesPending(GetEmployee()).Count();
            //dashboardViewModel.ReimbursementClaims = paymentRequestService.GetReimbursementClaims(GetEmployee()).Count();
            //dashboardViewModel.NoAdvanceClaims = paymentRequestService.GetNoAdvanceClaims(GetEmployee()).Count();

            dashboardViewModel.ReimbursementClaims = paymentRequestService.GetForMyApprovalExpenseDashboard(GetEmployee(),data1).Count();
            dashboardViewModel.NoAdvanceClaims = paymentRequestService.GetForMyApprovalAdvanceDashboard(GetEmployee(),data1).Count();

            var expenses = paymentRequestService.GetForMyApprovalExpenseDashboard(GetEmployee(),data1);
            var expense = 0.0;
            foreach (var exp in expenses) expense += exp.Amount;
            dashboardViewModel.ReimbursementClaimedAmount = expense;
            return dashboardViewModel;
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
                timesheetMonth = DateTime.Today.ToString("yyyy-MM");
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

        private DashboardViewModel BuildCoordinatorDashboard()
        {
            var dashboardViewModel = new DashboardViewModel();
            var coordinatorExpenses = paymentRequestService.GetOnBehalfExpenseDashboard(GetEmployee())
                .Select(e => new ExpenseListItemsViewModel
                {
                    Amount = e.Amount,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    AppliedFor = e.Employee.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var coordinatorAdvances = paymentRequestService.GetOnBehalfAdvanceDashboard(GetEmployee())
                .Select(e => new AdvanceListItemsViewModel
                {
                    Amount = e.Amount,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    AppliedFor = e.Employee.Name,
                    AppliedBy = e.PaymentRequestCreatedBy.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var coordinatorLeaves = leaveRequestServices.GetOnBehalfLeaves(GetEmployee(), null)
                .Select(e => new LeaveItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    ApplyDate = e.LeaveRequestCreatedDate,
                    TotalDays = e.NumberOfDays,
                    BalanceLeaves = e.GetLeaveBalance(),
                    LeaveType = e.LeaveType?.Name,
                    Status = e.Status,
                    Location = e.Employee.Location.Name,
                }).ToList();

            var coordinatorTimesheets = timeSheetServices.GetOnBehalfRejectedTimeSheets(GetEmployee())
                .Select(e => new TimesheetItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    Role = e.Employee.Designation.Name,
                    RejectedBy = e.TimesheetApprovedBy?.Name,
                    OvertimeHours = e.OvertimeHoursTS(),
                    CompOffs = e.CompOffs
                }).ToList();
            dashboardViewModel.ExpenseListItems = coordinatorExpenses;
            dashboardViewModel.AdvanceListItems = coordinatorAdvances;
            dashboardViewModel.LeaveItems = coordinatorLeaves;
            dashboardViewModel.TimesheetItems = coordinatorTimesheets;
            dashboardViewModel.ReimbursementPendingClaim = paymentRequestService.GetOnBehalfExpenseDashboard(GetEmployee()).Count();
            dashboardViewModel.OvertimeRejectedEmployees = timeSheetServices.GetOnBehalfRejectedTimeSheets(GetEmployee()).Count();
            dashboardViewModel.NoAdvanceClaims = paymentRequestService.GetOnBehalfAdvanceDashboard(GetEmployee()).Count();
            dashboardViewModel.ReimbursementClaimed = paymentRequestService.GetOnBehalfExpensePendingApprovalDashboard(GetEmployee()).Count();
            //var amount = 0.0;
            //var expenses = paymentRequestService.GetOnBehalfExpenseDashboard(GetEmployee());
            //foreach (var expense in expenses) amount += expense.Amount;
            //dashboardViewModel.ReimbursementClaimed = amount;
            return dashboardViewModel;
            //dashboardViewModel.ReimbursementPendingClaim = paymentRequestService.GetReimbursementClaims(GetEmployee()).Count();
            //dashboardViewModel.NoAdvanceClaims = paymentRequestService.GetNoAdvanceClaims(GetEmployee()).Count();
            //dashboardViewModel.ReimbursementClaimed = paymentRequestService.GetReimbursementClaimed(GetEmployee()).Count();

        }

        private DashboardViewModel BuildHRDashboard()
        {
            var dashboardViewModel = new DashboardViewModel();
            var hrExpenses = paymentRequestService.GetHRExpenseDashboard (GetEmployee())
                .Select(e => new ExpenseListItemsViewModel
                {
                    Amount = e.Amount,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    AppliedBy = e.PaymentRequestCreatedBy.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var hrAdvances = paymentRequestService.GetHRAdvanceDashboard(GetEmployee())
                .Select(e => new AdvanceListItemsViewModel
                {
                    Amount = e.Amount,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    AppliedBy = e.PaymentRequestCreatedBy.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var hrLeaves = leaveRequestServices.GetHRDashboardLeaves(GetEmployee(), null)
                .Select(e => new LeaveItemsViewModel
                {
                    Id = e.Id,
                    EmployeeCode = e.Employee.EmployeeCode,
                    EmployeeName = e.Employee.Name,
                    ApplyDate = e.LeaveRequestCreatedDate,
                    //BalanceLeaves = e.GetLeaveBalance(),
                    TotalDays = e.NumberOfDays,
                    LeaveType = e.LeaveType?.Name,
                    Status = e.Status,
                    Location = e.Employee.Location.Name,
                }).ToList();

            dashboardViewModel.ExpenseListItems = hrExpenses;
            dashboardViewModel.AdvanceListItems = hrAdvances;
            dashboardViewModel.LeaveItems = hrLeaves;
            dashboardViewModel.EmployeeLeaveStatus = leaveRequestServices.GetForHrLeavesPending(GetEmployee()).Count();
            dashboardViewModel.ReimbursementClaims = paymentRequestService.GetReimbursementClaims(GetEmployee()).Count();
            dashboardViewModel.NoAdvanceClaims = paymentRequestService.GetNoAdvanceClaims(GetEmployee()).Count();
            dashboardViewModel.EmployeeTimesheetApproved = timeSheetServices.GetApprovedTimeSheets(GetEmployee()).Count();
            return dashboardViewModel;
        }

        private DashboardViewModel BuildFinanceDashboard()
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            var financeExpenses = paymentRequestService.GetFinanceDashboardApprovedExpenseList(GetEmployee(), null)
                .Select(e => new ExpenseListItemsViewModel
                {
                    Amount = e.Amount,
                    EmployeeName = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Role = e.Employee.Designation.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var financeAdvances = paymentRequestService.GetFinanceDashboardApprovedAdvanceList(GetEmployee(), null)
                .Select(e => new AdvanceListItemsViewModel
                {
                    Amount = e.Amount,
                    EmployeeName = e.Employee.Name,
                    RequestNumber = e.RequestNumber,
                    ClaimDate = e.PaymentRequestCreatedDate,
                    Status = e.Status,
                    Role = e.Employee.Designation.Name,
                    Id = e.Id,
                    CurrencyName = e.Currency.Name,
                }).ToList();

            var financePendingDocuments = paymentRequestService.GetFinancePendingDocumentsList(GetEmployee(), null)
                .Select(p => new PendingDocumentViewModel
                {
                    Id=p.Id,
                    Amount = p.Amount,
                    EmployeeName = p.Employee.Name,
                    RequestNumber = p.RequestNumber,
                    ClaimDate = p.PaymentRequestCreatedDate,
                    Status = p.Status,
                    Role = p.Employee.Designation.Name,
                    HardCopyReceived = p.SupportingDocumentsReceivedDate.HasValue,
                    ReceivedDate = p.SupportingDocumentsReceivedDate,
                    CurrencyName = p.Currency.Name,
                }).ToList();

            dashboardViewModel.ExpenseListItems = financeExpenses;
            dashboardViewModel.AdvanceListItems = financeAdvances;
            dashboardViewModel.PendingDocuments = financePendingDocuments;
            dashboardViewModel.SupportingPendingToBeReceived = paymentRequestService.GetSupportingPendingToBeReceived(GetEmployee()).Count();
            dashboardViewModel.ApprovedAdvanceRequest = paymentRequestService.GetAdvanceRequestApproved(GetEmployee()).Count();
            dashboardViewModel.AdvanceRequestPaid = paymentRequestService.GetAdvanceRequestPaid(GetEmployee()).Count();
            dashboardViewModel.ApprovedReimbursementUnpaid = paymentRequestService.GetApprovedReimbursementUnpaid(GetEmployee()).Count();
            return dashboardViewModel;

        }
        [HttpGet]
        public async Task<IActionResult> LoginCallback(string token, string userId, string type, string targetaction, string objectId, string area, string employeeCode)
        {
            var user = await userManager.FindByIdAsync(userId);
            var isValid = await userManager.VerifyUserTokenAsync(user, "Default", "passwordless-auth", token);
            isValid = true;
            if (isValid)
            {
                await userManager.UpdateSecurityStampAsync(user);
                await signInManager.SignInAsync(user, true);


                await HttpContext.SignInAsync(
                    IdentityConstants.ApplicationScheme,
                    new ClaimsPrincipal(new ClaimsIdentity(
                        new List<Claim>{ new Claim("sub", user.Id),
                                        new Claim(ClaimTypes.Name, user.UserName)},
                        IdentityConstants.ApplicationScheme)));

                return RedirectToAction(targetaction, type, new { area=area, id=objectId, EmployeeCode = employeeCode});
            }

            return View("Error");
        }

        [Route("get-captcha-image")]
        public IActionResult GetCaptchaImage()
        {
            int width = 100;
            int height = 36;
            var captchaCode = Captcha.GenerateCaptchaCode();
            var result = Captcha.GenerateCaptchaImage(width, height, captchaCode);
            HttpContext.Session.SetString("CaptchaCode", result.CaptchaCode);
            Stream s = new MemoryStream(result.CaptchaByteData);
            return new FileStreamResult(s, "image/png");
        }

    }
}