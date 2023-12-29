using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
    * Model for Location
    */
    public partial class Location : Entity<int>
    {
        public Location()
        {

        }
        [Display(Name = "Location")]
        public string Name { get; set; }

        public string Code { get; set; }

        public bool Deactivated { get; set; } = false;

        public virtual IEnumerable<LocationWorkHours> Workdays { get; set; }

        public LocationWorkHours GetWorkday(DayOfWeek dow)
        {
            foreach (var d in Workdays)
            {
                if (d.DayOfWeek == dow) return d;
            }
            return null;
        }

    }
}