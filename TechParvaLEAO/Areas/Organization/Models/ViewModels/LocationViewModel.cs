using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    public class LocationViewModel
    {
        public int Id { get; set; }
        [Display(Name = "Location")]
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Deactivated { get; set; } = false;
        [Display(Name = "Monday")]
        public WorkDayType MondayWorkDayType { get; set; }
        [Display(Name = "Tuesday")]
        public WorkDayType TuesdayWorkDayType { get; set; }
        [Display(Name = "Wednesday")]
        public WorkDayType WednesdayWorkDayType { get; set; }
        [Display(Name = "Thursday")]
        public WorkDayType ThursdayWorkDayType { get; set; }
        [Display(Name = "Friday")]
        public WorkDayType FridayWorkDayType { get; set; }
        [Display(Name = "Saturday")]
        public WorkDayType SaturdayWorkDayType { get; set; }
        [Display(Name = "Sunday")]
        public WorkDayType SundayWorkDayType { get; set; }
    }
}
