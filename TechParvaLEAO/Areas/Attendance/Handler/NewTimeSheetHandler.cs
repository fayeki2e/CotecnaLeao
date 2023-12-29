using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;

namespace TechParvaLEAO.Areas.Attendance.Handler
{
    /*
     * Handler class for New TimeSheet.
     */
    public class Result
    {
        public bool Success { get; set; }
        public int Id { get; set; } 
    }
    public class NewTimeSheetHandler : IRequestHandler<TimeSheetViewModel, int>
    {
        private readonly IApplicationRepository repository;
        private readonly ApplicationDbContext dbContext;
        private readonly TimeSheetServices timeSheetServices;
        private readonly IMapper mapper;
        private readonly IEmployeeServices employeeServices;
        public NewTimeSheetHandler(IApplicationRepository repository, IMapper mapper,
            ApplicationDbContext dbContext,
            TimeSheetServices timeSheetServices,
            IEmployeeServices employeeServices
            )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.timeSheetServices = timeSheetServices;
            this.employeeServices = employeeServices;
        }

        public async Task<int> Handle(TimeSheetViewModel timeSheetVm, CancellationToken cancellationToken)
        {
            var employee = repository.GetOne<Employee>(e => e.Id == timeSheetVm.EmployeeId);
            TimeSheet timeSheetRecord = null;
            var basicSalary = await employeeServices.GetBasicSalary(timeSheetVm.EmployeeId, timeSheetVm.StartDate);
            timeSheetRecord = new TimeSheet
            {
                EmployeeId = timeSheetVm.EmployeeId,
                StartDate = timeSheetVm.StartDate,
                EndDate = timeSheetVm.EndDate,
                TimesheetCreatedById = timeSheetVm.CreatedByEmployeeId,
                TotalWorkHours = 0.0f,
                TotalWorkedHolidays = 0.0f,
                BasicSalary = basicSalary,
                OvertimeAmount = 0,
                OvertimeHours = 0,
                CompOffs = 0,
                WeekInMonth =timeSheetVm.WeekInMonth,
                WeekInYear = timeSheetVm.WeekInYear,
                Status = TimesheetStatus.DRAFT.ToString()
            };
            dbContext.TimeSheets.Add(timeSheetRecord);

            var attendaceEntries = new List<TimesheetAttendanceRecord>();

            foreach (var timesheetEntry in timeSheetVm.TimeSheetEntries)
            {
                var timeIn = TimeSpan.Zero;
                var timeOut = TimeSpan.Zero;
                var timeWorked = TimeSpan.Zero;
                TimeSpan.TryParse(timesheetEntry.InTime, out timeIn);
                TimeSpan.TryParse(timesheetEntry.OutTime, out timeOut);
                var timeOutCalc = timeOut;
                if (timeIn > timeOut)
                {
                    timeOutCalc = timeOut.Add(TimeSpan.FromHours(24));
                }
                timeWorked = timeOutCalc.Subtract(timeIn);
                TimesheetAttendanceRecord attendance = new TimesheetAttendanceRecord
                {
                    TimeSheet = timeSheetRecord,
                    EmployeeId = timeSheetVm.EmployeeId,
                    Client = timesheetEntry.Client,
                    Job = timesheetEntry.JobNumber,
                    TimeIn = timeIn,
                    TimeOut = timeOut,
                    WorkingTime = timeWorked,
                    WorkDate = timesheetEntry.Date,
                    IsHoliday = timesheetEntry.IsHoliday,
                    IsWeekend = timesheetEntry.IsWeekOff,
                    IsLeave = timesheetEntry.IsLeave,
                    IsTravellingDay = false,
                    IsHalfDay = timesheetEntry.IsHalfDay,
                    IsHalfDayLeave = timesheetEntry.IsHalfDayLeave
                };
                dbContext.TimesheetAttendanceRecord.Add(attendance);
            }
            timeSheetServices.PerformTimesheetCalculations(timeSheetRecord);
            var compOffList = timeSheetServices.CalculateCompOffs(timeSheetRecord);
            foreach(var compOff in compOffList)
            {
                timeSheetRecord.CompOffs += 1;
                dbContext.TimesheetCompOff.Add(compOff);
            }
            dbContext.SaveChanges();
            return timeSheetRecord.Id;
        }
    }
}
