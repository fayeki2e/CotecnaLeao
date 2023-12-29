using CalendarUtilities;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Data;



namespace TechParvaLEAO.Areas.Reports.Services
{
    public class TimesheetReportsServices
    {
        private readonly ApplicationDbContext dbContext;

        public TimesheetReportsServices(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        private IEnumerable<TimeSheet> ApplySearchOnTimesheet(IQueryable<TimeSheet> query, TimesheetReportSearchVm searchVm)
        {
            if (searchVm == null) return query;

            if (searchVm.FromDate != null)
            {
                query = query.Where(t => t.StartDate >= searchVm.FromDate);
            }
            if (searchVm.ToDate != null)
            {
                query = query.Where(t => t.EndDate <= searchVm.ToDate);
            }
            if (searchVm.Location != null)
            {
                query = query.Where(t => t.Employee.LocationId == searchVm.Location);
            }
            if (searchVm.Employee != null)
            {
                query = query.Where(t => t.EmployeeId == searchVm.Employee);
            }
            return query;
        }

        private IEnumerable<TimesheetAttendanceRecord> ApplySearchOnAttendanceRecord(IQueryable<TimesheetAttendanceRecord> query, TimesheetReportSearchVm searchVm)
        {
            if (searchVm == null) return query;

            if (searchVm.FromDate != null)
            {
                query = query.Where(t => t.WorkDate >= searchVm.FromDate);
            }
            if (searchVm.ToDate != null)
            {
                query = query.Where(t => t.WorkDate <= searchVm.ToDate);
            }
            if (searchVm.Location != null)
            {
                query = query.Where(t => t.Employee.LocationId == searchVm.Location);
            }
            if (searchVm.Employee != null)
            {
                query = query.Where(t => t.EmployeeId == searchVm.Employee);
            }
            if (searchVm.JobNumber != null)
            {
                query = query.Where(t => EF.Functions.Like(t.Job, "%" + searchVm.JobNumber + "%"));
            }
            if (searchVm.Client != null)
            {
                query = query.Where(t => EF.Functions.Like(t.Client, "%" + searchVm.Client + "%"));
            }
            return query;
        }


        public IEnumerable<TimesheetEntriesReportVm> SearchTimesheetEntries(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimesheetAttendanceRecord.Where(t => 
                t.TimeSheet.Status == TimesheetStatus.APPROVED.ToString());
            var searchResult = ApplySearchOnAttendanceRecord(query, searchVm);
            var result = new List<TimesheetEntriesReportVm>();
            foreach (var entry in searchResult)
            {
                var otHours = entry.IsHoliday || entry.IsLeave || entry.IsWeekend ?
                    TimeSpan.FromMilliseconds(0) :
                    (entry.WorkingTime - TimeSpan.FromHours(8));
                if (otHours.TotalSeconds < 0)
                {
                    otHours = TimeSpan.FromSeconds(0);
                }
                result.Add(new TimesheetEntriesReportVm
                {
                    EmployeeCode = entry.Employee.EmployeeCode,
                    Name = entry.Employee.Name,
                    Branch = entry.Employee.Location.Name,
                    JobNumber = entry.Job,
                    Client = entry.Client,
                    JobDescription = entry.Job,
                    InTime = entry.TimeIn.ToString(@"hh\:mm"),
                    OutTime = entry.TimeOut.ToString(@"hh\:mm"),
                    TimesheetDate = entry.WorkDate,
                    TotalHours = entry.WorkingTime.ToString(@"hh\:mm"),
                    WorkingHours = entry.WorkingTime.ToString(@"hh\:mm"),
                    OvertimeHours = otHours.ToString(@"hh\:mm")
                });
            }
            return result;
        }

        private void SetDefaults(TimesheetReportSearchVm searchVm)
        {
            var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15).AddMonths(-1);
            var to = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 15);

            if (!searchVm.FromDate.HasValue)
            {
                searchVm.FromDate = DateTimeUtils.GetLast(from, DayOfWeek.Monday);
            }
            if (!searchVm.ToDate.HasValue)
            {
                searchVm.ToDate = DateTimeUtils.GetLast(to, DayOfWeek.Sunday);
            }
        }

        public IEnumerable<LocationOvertimeReportVm> SearchLocationOvertime(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimeSheets.Where(t => t.Status == TimesheetStatus.APPROVED.ToString());
            SetDefaults(searchVm);
            var searchResult = ApplySearchOnTimesheet(query, searchVm);
            var result = searchResult.GroupBy(t => new {t.Employee.Location}).Select(t =>
                new LocationOvertimeReportVm
                {
                    Branch = t.Key.Location.Name,
                    StartDate = searchVm.FromDate,
                    EndDate = searchVm.ToDate,
                    TotalNumberOfOTHours = t.Sum(item => item.OvertimeHours),
                }
            );
            return result;
        }

        public IEnumerable<LocationOvertimePaymentReportVm> SearchLocationOvertimePayment(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimeSheets.Where(t => t.Status == TimesheetStatus.APPROVED.ToString());
            SetDefaults(searchVm);
            var searchResult = ApplySearchOnTimesheet(query, searchVm);
            var result = searchResult.GroupBy(t => new { t.Employee.Location, t.Employee }).Select(t =>
                new LocationOvertimePaymentReportVm{ Branch = t.Key.Location.Name,
                    EmployeeCode = t.Key.Employee.EmployeeCode,
                    EmployeeName = t.Key.Employee.Name,
                    StartDate = searchVm.FromDate,
                    EndDate = searchVm.ToDate,
                    OTRule = t.Key.Employee.OvertimeMultiplier?.Name,
                    TotalNumberOfOTHours = t.Sum(item => item.OvertimeHours),
                    OvertimeAmount = t.Sum(item => item.OvertimeAmount),
                }
            );
            return result;
        }

        public IEnumerable<EmployeeWeeklyWorkedHoursReportVm> SearchEmployeeWeeklyWorkedHours(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimeSheets.Where(t => t.Status == TimesheetStatus.APPROVED.ToString());
            var searchResult = ApplySearchOnTimesheet(query, searchVm);
            var result = searchResult.Select(t => new EmployeeWeeklyWorkedHoursReportVm
            {
                EmployeeCode = t.Employee.EmployeeCode,
                EmployeeName = t.Employee.Name,
                Branch = t.Employee.Location.Name,
                FromDate = t.StartDate,
                ToDate = t.EndDate,
                TotalNumberOfHours = t.TotalWorkHoursTime(),
            });
            return result;
        }

        public IEnumerable<EmployeeWeeklyOTReportVm> SearchEmployeeWeeklyOT(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimeSheets.Where(t => t.Status == TimesheetStatus.APPROVED.ToString());
            var searchResult = ApplySearchOnTimesheet(query, searchVm);
            var result = searchResult.Select(t => new EmployeeWeeklyOTReportVm
            {
                EmployeeCode = t.Employee.EmployeeCode,
                EmployeeName = t.Employee.Name,
                Branch = t.Employee.Location.Name,
                FromDate = t.StartDate,
                ToDate = t.EndDate,
                TotalNumberOfHours = t.TotalWorkHoursTime(),
                TotalNumberOfOTHours = t.OvertimeHoursTS(),
                OvertimeAmount = t.OvertimeAmount
            });
            return result;
        }

        public IEnumerable<TimeSheet> SearchTimesheetPrint(TimesheetReportSearchVm searchVm)
        {
            var query = dbContext.TimeSheets.Where(t => t.Status == TimesheetStatus.APPROVED.ToString());
            var searchResult = ApplySearchOnTimesheet(query, searchVm);
            return searchResult;

        }
    }
}
