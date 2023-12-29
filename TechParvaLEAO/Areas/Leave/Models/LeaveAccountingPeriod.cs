using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
  * Model for Leave Accounting Period
  */
    public class LeaveAccountingPeriod: Entity<int>
    {
        public LeaveAccountingPeriod()
        { }

        [Display(Name = "Year")]
        public string Name { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public bool Active { get; set; }

        [Display(Name = "Number of Days of Leave to Credit")]
        public int NumberOfDaysOfLeave { get; set; }

        [Display(Name = "Maximum Carry Forward")]
        public int MaxCarryForwardFromLastYear { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}