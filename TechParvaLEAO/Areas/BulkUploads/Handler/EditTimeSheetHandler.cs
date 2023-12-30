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

namespace TechParvaLEAO.Areas.BulkUploads.Handler
{
    /*
     * Handler class for Edit timesheet functionality.
     */
    public class EditTimeSheetHandler : IRequestHandler<TimeSheetEditViewModel, int>
    {
        private readonly IApplicationRepository repository;
        private readonly ApplicationDbContext dbContext;
        private readonly TimeSheetServices timeSheetServices;
        private readonly IMapper mapper;
        private readonly IEmployeeServices employeeServices;
        public EditTimeSheetHandler(IApplicationRepository repository, IMapper mapper,
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

        /*
         * Handler method for Edit Timesheet
         * Updates the status of the timesheet to Pending.
         * Updates all individual attendance records from data.
         */
        public async Task<int> Handle(TimeSheetEditViewModel timeSheetVm, CancellationToken cancellationToken)
        {
            var basicSalary = await employeeServices.GetBasicSalary(timeSheetVm.EmployeeId, timeSheetVm.StartDate);
            var timeSheetRecord = await repository.GetByIdAsync<TimeSheet>(timeSheetVm.TimesheetId);
            var employee = timeSheetRecord.Employee;
            var attendaceEntries = new List<TimesheetAttendanceRecord>();
            //If timesheetwas already submitted, update status to Pending
            if ("Submit".Equals(timeSheetVm.Submit))
            {
                timeSheetRecord.Status = "PENDING";
            }

            //Update each timehseet entry row from the view model
            foreach (var timesheetEntry in timeSheetVm.TimeSheetEntries)
            {
                var timeIn = TimeSpan.Zero;
                var timeOut = TimeSpan.Zero;
                var timeWorked = TimeSpan.Zero;
                TimeSpan.TryParse(timesheetEntry.InTime, out timeIn);
                TimeSpan.TryParse(timesheetEntry.OutTime, out timeOut);
                var timeOutCalc = timeOut;
                //In case timeIn is less than timeOut, add 24 hours as it's in next day
                if (timeIn > timeOut)
                {
                    timeOutCalc = timeOut.Add(TimeSpan.FromHours(24));
                }
                timeWorked = timeOutCalc.Subtract(timeIn);
                TimesheetAttendanceRecord attendance = await repository.GetByIdAsync<TimesheetAttendanceRecord>(timesheetEntry.TimesheetEntryId);
                attendance.Client = timesheetEntry.Client;

                attendance.Job = timesheetEntry.JobNumber;
                attendance.TimeIn = timeIn;
                attendance.TimeOut = timeOut;
                attendance.WorkingTime = timeWorked;
                attendance.WorkDate = timesheetEntry.Date;
                attendance.IsHoliday = timesheetEntry.IsHoliday;
                attendance.IsWeekend = timesheetEntry.IsWeekOff;
                attendance.IsLeave = timesheetEntry.IsLeave;
                attendance.IsTravellingDay = false;
                attendance.IsHalfDay = timesheetEntry.IsHalfDay;
                attendance.IsHalfDayLeave = timesheetEntry.IsHalfDayLeave;
                dbContext.TimesheetAttendanceRecord.Update(attendance);
            }
            timeSheetServices.PerformTimesheetCalculations(timeSheetRecord);
            var compOffList = timeSheetServices.CalculateCompOffs(timeSheetRecord);
            timeSheetRecord.CompOffs = 0;
            //Remove all existing Compoffs already saved and create new comp offs as
            //Comp offs may have been changed as per newly submitted data.
            dbContext.TimesheetCompOff.RemoveRange(timeSheetRecord.TimesheetCompOffs);
            foreach (var compOff in compOffList)
            {
                timeSheetRecord.CompOffs += 1;
                dbContext.TimesheetCompOff.Add(compOff);
            }
            dbContext.SaveChanges();
            return 1;
        }
    }
}
