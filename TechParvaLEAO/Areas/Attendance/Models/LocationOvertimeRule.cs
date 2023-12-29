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
     * Model for Location Overtime Rule
     */
    public partial class LocationOvertimeRule : Entity<int>
    {
        public LocationOvertimeRule()
        {
        }

        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int? LocationId { get; set; }
        public virtual Location Location { get; set; }

        [Display(Name = "Overtime Multiplier")]
        public double OvertimeMultiplier { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}