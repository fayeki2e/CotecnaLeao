using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * Model for Timesheet Attendance Record
     */
    public partial class TimesheetAttendanceRecord: Entity<int>
    {
        public TimesheetAttendanceRecord()
        {
        }

        [Display(Name = "TimeSheet")]
        [ForeignKey("TimeSheet")]
        public virtual int TimeSheetId { get; set; }
        public virtual TimeSheet TimeSheet { get; set; }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Display(Name = "Business Activity")]
        public string Client { get; set; }

        public string Job { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Work Date")]
        public DateTime WorkDate { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Time In")]
        public TimeSpan TimeIn { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Time Out")]
        public TimeSpan TimeOut { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Working Time")]
        public TimeSpan WorkingTime { get; set; }

        [DisplayFormat(DataFormatString = @"{0:hh\:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Approved Time")]
        public TimeSpan ApprovedTime { get; set; }

        [Display(Name = "Is Weekend")]
        public bool IsWeekend { get; set; }

        [Display(Name = "Is Holiday")]
        public bool IsHoliday { get; set; }

        [Display(Name = "Is Leave")]
        public bool IsLeave { get; set; }

        [Display(Name = "Travelling Day")]
        public bool IsTravellingDay { get; set; }

        [Display(Name = "Holiday Reason")]
        public string HolidayReason { get; set; }

        [Display(Name = "Is Half Day")]
        public bool IsHalfDay { get; set; }

        [Display(Name = "Is Half Day Leave")]
        public bool IsHalfDayLeave { get; set; }
    }
}