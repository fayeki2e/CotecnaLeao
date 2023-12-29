using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Techparva.GenericRepository;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
    * Model for Employee Weekly Off
    */
    public class EmployeeWeeklyOff : Entity<int>
    {
        public EmployeeWeeklyOff()
        {

        }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Form Date")]
        public DateTime FormDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [Display(Name = "Weekly Off Day")]
        public DayOfWeek WeeklyOffDay { get; set; }

        [Display(Name = "Other Weekly Off Day")]
        public DayOfWeek? OtherWeeklyOffDay { get; set; }

        [Display(Name = "Other Weekly Off Day Rule")]
        public WorkDayType? OtherWeeklyOffRule { get; set; }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
    }
}
