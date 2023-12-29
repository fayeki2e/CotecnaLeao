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
  * Model for Leave Rejection Reason
  */
    public partial class LeaveRejectionReason: Entity<int>
    {
        public LeaveRejectionReason()
        { }

        [Display(Name ="Leave Rejection Reason")]
        public string Text { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}