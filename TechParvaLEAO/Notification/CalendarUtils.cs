using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace CalendarUtilities
{
    public class CalendarUtilities
    {
        public IEnumerable<DateTime> GetDateList(DateTime start, DateTime end)
        {
            return Enumerable.Range(0, 1 + end.Subtract(start).Days)
              .Select(offset => start.AddDays(offset))
              .ToArray();
        }
        public DateTime GetDateFromMonthStr(string dateTimeParam)
        {
            try
            {
                var year = Int16.Parse(dateTimeParam.Substring(0, dateTimeParam.IndexOf("-")));
                var month = Int16.Parse(dateTimeParam.Substring(dateTimeParam.IndexOf("-")));
                return new DateTime(year, month, 1);
            }
            catch
            {
                //Invalid format so simply return today's date
            }
            return DateTime.Today;
        }
        public DateTime GetDateFromWeekStr(string dateTimeParam)
        {
            try
            {
                var year = Int16.Parse(dateTimeParam.Substring(0, dateTimeParam.IndexOf("-")));
                var month = Int16.Parse(dateTimeParam.Substring(dateTimeParam.IndexOf("-")));
                return new DateTime(year, month, 1);
            }
            catch
            {
                //Invalid format so simply return today's date
            }
            return DateTime.Today;
        }

    }


    static class DateTimeExtensions
    {
        static GregorianCalendar gregorianCalendar = new GregorianCalendar();
        public static int GetWeekOfMonth(this DateTime time)
        {
            DateTime first = new DateTime(time.Year, time.Month, 1);
            if(time.GetWeekOfYear(CalendarWeekRule.FirstDay) < 
                first.GetWeekOfYear(CalendarWeekRule.FirstDay))
            {
                return time.GetWeekOfYear(CalendarWeekRule.FirstDay) + 1;
            }
            else
            {
                return time.GetWeekOfYear(CalendarWeekRule.FirstDay) - first.GetWeekOfYear(CalendarWeekRule.FirstDay) + 1;
            }
        }
        public static int GetWeekOfYear(this DateTime time)
        {
            return GetWeekOfYear(time, CalendarWeekRule.FirstFullWeek);
            //return gregorianCalendar.GetWeekOfYear(time, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
        public static int GetWeekOfYear(this DateTime time, CalendarWeekRule rule)
        {
            return gregorianCalendar.GetWeekOfYear(time, rule, DayOfWeek.Monday);
        }
        public static int WeeksInMonth(this DateTime time)
        {
            //extract the month
            int daysInMonth = DateTime.DaysInMonth(time.Year, time.Month);
            var firstOfMonth = new DateTime(time.Year, time.Month, 1);
            //days of week starts by default as Sunday = 0
            var firstDayOfMonth = (int)firstOfMonth.DayOfWeek;
            var weeksInMonth = (int)Math.Ceiling((firstDayOfMonth + daysInMonth) / 7.0);

            return weeksInMonth;
        }
        public static DateTime GetFirstDayOfMonth(this DateTime anyDate)
        {
            var firstDayOfMonth = new DateTime(anyDate.Year, anyDate.Month, 1);
            return firstDayOfMonth;
        }
        public static DateTime GetLastDayOfMonth(this DateTime anyDate)
        {
            var firstDayOfMonth = new DateTime(anyDate.Year, anyDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddTicks(-1);
            return lastDayOfMonth;
        }
        public static IList<DateTime> MondaysInMonth(this DateTime thisMonth)
        {
            var mondayList = new List<DateTime>();
            int month = thisMonth.Month;
            int year = thisMonth.Year;
            int daysThisMonth = DateTime.DaysInMonth(year, month);
            DateTime beginingOfThisMonth = new DateTime(year, month, 1);
            for (int i = 0; i < daysThisMonth; i++)
                if (beginingOfThisMonth.AddDays(i).DayOfWeek == DayOfWeek.Monday)
                    mondayList.Add(beginingOfThisMonth.AddDays(i));
            return mondayList;
        }
    }
}
