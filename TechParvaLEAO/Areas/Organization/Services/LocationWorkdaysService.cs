using CalendarUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Organization.Services
{
    public class LocationCalendar
    {
        public DateTime Date { get; set; }
        public bool IsHoliday { get; set; }
        public bool IsWeekOff { get; set; }
        public bool IsHalfDay { get; set; }
        public bool IsLeave { get; set; }
        public bool IsHalfDayLeave { get; set; }
    }

    public class LocationWorkdaysService
    {
        IApplicationRepository repository;

        public LocationWorkdaysService(IApplicationRepository repository)
        {
            this.repository = repository;
        }
        public bool IsHoliday(int locationId, DateTime dateTime)
        {
            var count = repository.Get<Holiday>
                (h => h.LocationId == locationId && h.HolidayDate == dateTime).Count();
            return count == 0 ? false : true;

        }

        public IEnumerable<Holiday> GetHolidays(int locationId, DateTime startDate, DateTime endDate)
        {
            return repository.Get<Holiday>
                (h => h.LocationId == locationId && 
                (h.HolidayDate>=startDate && h.HolidayDate<=endDate))
                .OrderBy(h=>h.HolidayDate);
        }

        public IList<LocationCalendar> GetLocationCalendar(int locationId, DateTime startDate, DateTime endDate)
        {
            var locationCalendarList = new List<LocationCalendar>();
            var locationWorkMap = new Dictionary<DayOfWeek, LocationWorkHours>();

            var locationWorkDays = repository.Get<LocationWorkHours>(lwh => lwh.LocationId == locationId);
            var locationHolidays = repository.Get<Holiday>(h => h.LocationId == locationId && h.HolidayDate >= startDate && h.HolidayDate <= endDate).ToList();

            foreach (var workday in locationWorkDays)
            {
                locationWorkMap[workday.DayOfWeek] = workday;
            }

            for (var date=startDate; date<=endDate; date += TimeSpan.FromDays(1))
            {
                var locationWorkDay = locationWorkMap[date.DayOfWeek];
                var locationCalendar = new LocationCalendar
                {
                    Date = date,
                    IsLeave = false,
                    IsHalfDayLeave = false,
                    IsHalfDay = IsHalfDay(date, locationWorkDay),
                    IsHoliday = IsHoliday(date, locationHolidays),
                    IsWeekOff = IsWeekOff(date, locationWorkDay)
                };
                locationCalendarList.Add(locationCalendar);
            }
            return locationCalendarList;
        }

        public IList<LocationCalendar> GetEmployeeCalendar(int locationId, int employeeId, DateTime startDate, DateTime endDate)
        {
            var locationCalendarList = new List<LocationCalendar>();
            var employeeWorkdayMap = new Dictionary<DateTime, WorkDayType>(); 
            var locationHolidays = repository.Get<Holiday>(h => h.LocationId == locationId && h.HolidayDate >= startDate 
                            && h.HolidayDate <= endDate).ToList();
            var employeeWeekdays = repository.Get<EmployeeWeeklyOff>(w => w.FormDate <= startDate && w.ToDate >= endDate && w.EmployeeId==employeeId);
            var workdate = startDate;
            while (workdate <= endDate)
            {
                employeeWorkdayMap[workdate] = IsWorkDay(workdate, employeeWeekdays);
                workdate += TimeSpan.FromDays(1);
            }
            for (var date = startDate; date <= endDate; date += TimeSpan.FromDays(1))
            {
                var locationCalendar = new LocationCalendar
                {
                    Date = date,
                    IsLeave = false,
                    IsHalfDayLeave = false,
                    IsHalfDay = employeeWorkdayMap[date] == WorkDayType.HALF_DAY,
                    IsHoliday = IsHoliday(date, locationHolidays),
                    IsWeekOff = employeeWorkdayMap[date] == WorkDayType.WEEKLY_OFF
                };
                locationCalendarList.Add(locationCalendar);
            }
            return locationCalendarList;
        }

        private bool IsHoliday(DateTime date, List<Holiday> holidayMap)
        {
            foreach(var holidaydate in holidayMap)
            {
                if (holidaydate.HolidayDate.Equals(date))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsHalfDay(DateTime date, LocationWorkHours locationWorkHours)
        {
            if (locationWorkHours.WorkDayType == WorkDayType.HALF_DAY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool IsWeekOff(DateTime date, LocationWorkHours locationWorkHours)
        {
            int weekOfMonth = date.GetWeekOfMonth();
            
            if (date.GetFirstDayOfMonth().DayOfWeek == DayOfWeek.Sunday)
            {
                weekOfMonth = weekOfMonth- 1;
            }
            
            if (locationWorkHours.WorkDayType == WorkDayType.WEEKLY_OFF)
            {
                return true;
            }else if (locationWorkHours.WorkDayType == WorkDayType.WEEK_OFF_1_3
                && (weekOfMonth == 1 || weekOfMonth == 3)) //weekOfMonth == 5
            {
                return true;
            }
            else if (locationWorkHours.WorkDayType == WorkDayType.WEEK_OFF_2_4
               && (weekOfMonth == 2 || weekOfMonth == 4)) //weekOfMonth == 5
            {
                return true;
            }
            return false;
        }

        private WorkDayType IsWorkDay(DateTime date, IEnumerable<EmployeeWeeklyOff> employeeWeekOffs)
        {
            foreach(var employeeWeekOff in employeeWeekOffs)
            {
                if (employeeWeekOff.FormDate < date && date < employeeWeekOff.ToDate)
                {
                    if (date.DayOfWeek == employeeWeekOff.WeeklyOffDay){
                        return WorkDayType.WEEKLY_OFF;
                    }else if(date.DayOfWeek == employeeWeekOff.OtherWeeklyOffDay &&
                        (employeeWeekOff.OtherWeeklyOffRule == WorkDayType.WEEKLY_OFF ||
                        employeeWeekOff.OtherWeeklyOffRule == WorkDayType.HALF_DAY))
                    {
                        return employeeWeekOff.OtherWeeklyOffRule.HasValue? employeeWeekOff.OtherWeeklyOffRule.Value: 
                                WorkDayType.FULL_WORKING_DAY;
                    }
                    else if (date.DayOfWeek == employeeWeekOff.OtherWeeklyOffDay &&
                       employeeWeekOff.OtherWeeklyOffRule == WorkDayType.WEEK_OFF_1_3)
                    {
                        int weekOfMonth = date.GetWeekOfMonth();
                        if (weekOfMonth == 1 || weekOfMonth == 3)
                            return WorkDayType.WEEKLY_OFF;
                    }else if (date.DayOfWeek == employeeWeekOff.OtherWeeklyOffDay &&
                      employeeWeekOff.OtherWeeklyOffRule == WorkDayType.WEEK_OFF_2_4)
                    {
                        int weekOfMonth = date.GetWeekOfMonth();
                        if (weekOfMonth == 2 || weekOfMonth == 4)
                            return WorkDayType.WEEKLY_OFF;
                    }
                }
            }
            return WorkDayType.FULL_WORKING_DAY;
        }
    }
}
