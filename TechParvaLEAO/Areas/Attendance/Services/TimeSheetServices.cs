using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Areas.Attendance.Services
{
    public class TimeSheetServices
    {
        private IApplicationRepository _context;
        public TimeSheetServices(IApplicationRepository context)
        {
            _context = context;
        }

        public IEnumerable<TimeSheet> GetOwnTimeSheets(Employee employee)
        {
            return _context.Get<TimeSheet>(t => t.EmployeeId == employee.Id, q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetOwnTimeSheets(Employee employee, DateTime start, DateTime end)
        {
            return _context.Get<TimeSheet>(t => t.EmployeeId == employee.Id && t.StartDate >= start && t.StartDate <= end, q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimesheetAttendanceRecord> GetTimeSheetsForEmployee(Employee employee, DateTime start, DateTime end)
        {
            return _context.Get<TimesheetAttendanceRecord>(t => t.EmployeeId == employee.Id && 
                t.WorkDate >= start && t.WorkDate <= end
                && t.WorkingTime > TimeSpan.FromMinutes(1)
                , q => q.OrderBy(s => s.WorkDate));
        }
        public IEnumerable<TimeSheet> GetOnBehalfTimeSheets(Employee employee)
        {
            return _context.Get<TimeSheet>(t => t.TimesheetCreatedById == employee.Id, q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetOnBehalfRejectedTimeSheets(Employee employee)
        {
            return _context.Get<TimeSheet>(t => t.TimesheetCreatedById == employee.Id && t.Status==TimesheetStatus.REJECTED.ToString(), 
                q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetOnBehalfTimeSheets(Employee employee, DateTime start, DateTime end)
        {
            return _context.Get<TimeSheet>(t => t.TimesheetCreatedById == employee.Id && t.StartDate >= start && t.StartDate <= end, q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetForMyApprovalTimeSheets(Employee employee)
        {
            var _reportingToMe = _context.Get<Employee>(e => e.ReportingToId == employee.Id && e.OnFieldEmployee == true).ToList(); 
            return _context.Get<TimeSheet>(t =>_reportingToMe.Contains(t.Employee), 
                q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetForMyApprovalTimeSheets(Employee employee, DateTime start, DateTime end)
        {
            var _reportingToMe = _context.Get<Employee>(e => e.ReportingToId == employee.Id && e.OnFieldEmployee == true).ToList();
            return _context.Get<TimeSheet>(t => _reportingToMe.Contains(t.Employee) 
                && t.StartDate >= start && t.StartDate <= end, q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public IEnumerable<TimeSheet> GetApprovedTimeSheets(Employee employee)
        {
            //var _reportingToMe = _context.Get<Employee>(e => e.ReportingToId == employee.Id).ToList();
            return _context.Get<TimeSheet>(t => t.TimesheetApprovedById == employee.Id && t.Status == TimesheetStatus.APPROVED.ToString(), q => q.OrderByDescending(s => s.TimesheetCreatedOn));
        }
        public void PerformTimesheetCalculations(TimeSheet timesheet)
        {
            var workedHours = TimeSpan.Zero;
            var workedHoursHolidays = TimeSpan.Zero;
            timesheet.OvertimeHours = 0;
            timesheet.OvertimeAmount = 0;
            foreach (var attendance in timesheet.AttendanceRecords)
            {
                if (!(attendance.IsHoliday || attendance.IsWeekend))
                {
                    workedHours = workedHours.Add(attendance.WorkingTime);
                }
            }
            timesheet.TotalWorkHours = workedHours.TotalHours;
            timesheet.TotalWorkedHolidays = workedHoursHolidays.TotalHours;

            if (string.Equals("INSPECTOR", timesheet.Employee.ExpenseProfile.Name))
            {
                if(timesheet.TotalWorkHours>48)
                {
                    timesheet.OvertimeHours = Math.Round(timesheet.TotalWorkHours - 48);
                    timesheet.OvertimeAmount = timesheet.OvertimeHours * (timesheet.BasicSalary / 30 / 8) * timesheet.Employee.OvertimeMultiplier.OvertimeMultiplier;
                }
            }
        }

        public IList<TimesheetCompOff> CalculateCompOffs(TimeSheet timesheet)
        {
            var compOffList = new List<TimesheetCompOff>();

            foreach (var attendance in timesheet.AttendanceRecords)
            {
                if (attendance.IsHoliday || attendance.IsWeekend)
                {
                    if (attendance.WorkingTime > TimeSpan.FromMinutes(15))
                    {
                        var compOff = new TimesheetCompOff
                        {
                            Status = CompOffApprovalStatus.PENDING.ToString(),
                            TimeSheet = timesheet,
                            CompOffDate = attendance.WorkDate,
                        };
                        compOffList.Add(compOff);
                    }
                }
            }
            return compOffList;
        }

        public void PerformTimesheetCalculations(int timesheetId)
        {
            PerformTimesheetCalculations(_context.GetById<TimeSheet>(timesheetId));
        }

    }
}
