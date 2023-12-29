using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * Model for Timesheet CompOff
     */
    public class TimesheetCompOff : Entity<int>
    {
        public TimesheetCompOff() { }

        [ForeignKey("TimeSheet")]
        public int TimeSheetId { get; set; }
        public virtual TimeSheet TimeSheet { get; set; }

        [Display(Name="Comp Off For Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CompOffDate { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; }

        [Display(Name = "Action By")]
        [ForeignKey("Employee")]
        public virtual int? ActionById { get; set; }
        public virtual Employee ActinbBy { get; set; }

        [Display(Name = "Action Date")]
        public virtual DateTime? ActionDate { get; set; }
    }

    public enum CompOffApprovalStatus
    {
        PENDING,
        APPROVED,
        REJECTED
    }
}
