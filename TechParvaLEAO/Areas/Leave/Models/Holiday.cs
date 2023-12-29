using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
  * Model for Holiday
  */
    public class Holiday: Entity<int>
    {
        public Holiday()
        { }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Holiday Date")]
        public DateTime HolidayDate { get; set; }

        public string Reason { get; set; }
        
        [Display(Name = "Is Half Day")]
        public bool IsHalfDay { get; set; }

        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int LocationId { get; set; }
        public virtual Location Location { get; set; }

        public bool Deactivated { get; set; } = false;

    }
}