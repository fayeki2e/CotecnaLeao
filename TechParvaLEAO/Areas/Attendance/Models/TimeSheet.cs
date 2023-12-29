using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Expense.Models;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * Model for TimeSheet
     */
    public partial class TimeSheet:Entity<int>, IAggregateRoot
    {
        public TimeSheet(){}
        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Worked Hours")]
        public double TotalWorkHours { get; set; }
        public string TotalWorkHoursTime()
        {
            var hours = (int)this.TotalWorkHours;
            var minutes_dec = (int)Math.Round((this.TotalWorkHours - (int)this.TotalWorkHours) * 60);
            return string.Format("{0}:{1:00}",
                     hours,
                     minutes_dec);
        }

        [Display(Name = "Overtime Hours")]
        public double OvertimeHours { get; set; }
        public string OvertimeHoursTS()
        {
            var hours = (int)this.OvertimeHours;
            var minutes_dec = (int)Math.Round((this.OvertimeHours - (int)this.OvertimeHours) * 60);
            return string.Format("{0}:{1:00}",
                     hours,
                     minutes_dec);
        }

        [Display(Name = "Worked Holidays")]
        public double TotalWorkedHolidays { get; set; }

        [Display(Name = "Basic Salary")]
        public double BasicSalary { get; set; }

        [Display(Name = "Overtime Amount")]
        public double OvertimeAmount { get; set; }

        [Display(Name = "Comp Offs")]
        public double CompOffs { get; set; }

        [Display(Name = "Timesheet Created By")]
        [ForeignKey("TimesheetCreatedBy")]
        public virtual int? TimesheetCreatedById { get; set; }
        public virtual Employee TimesheetCreatedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Timesheet Created On")]
        public DateTime TimesheetCreatedOn { get; set; }

        [Display(Name = "Timesheet Approved By")]
        [ForeignKey("TimesheetApprovedBy")]
        public virtual int? TimesheetApprovedById { get; set; }
        public virtual Employee TimesheetApprovedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Approved On")]
        public DateTime ApprovedOn { get; set; }

        public string Status { get; set; }

        public int NumberOfDays { get; set; }
        public int WeekInMonth { get; set; }
        public int WeekInYear { get; set; }

        public virtual ICollection<TimesheetAttendanceRecord> AttendanceRecords { get; set; }
        public virtual ICollection<TimesheetCompOff> TimesheetCompOffs { get; set; }
    }

    public enum TimesheetStatus
    {
        DRAFT,
        PENDING,
        APPROVED,
        REJECTED
    }
    
}