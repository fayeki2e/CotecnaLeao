using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
    * Model for Location Work Hours
    */
    public enum WorkDayType
    {
        WEEKLY_OFF,
        FULL_WORKING_DAY,
        HALF_DAY,
        WEEK_OFF_1_3,
        WEEK_OFF_2_4
    }
    public class LocationWorkHours : Entity<int>
    {
        public LocationWorkHours() {}

        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [Display(Name = "Day of Week")]
        public DayOfWeek DayOfWeek { get; set; }

        [Display(Name = "Working Hours")]
        public double WorkingHours { get; set; }

        [Display(Name = "Work Day Type")]
        public WorkDayType WorkDayType { get; set; }
    }
}
