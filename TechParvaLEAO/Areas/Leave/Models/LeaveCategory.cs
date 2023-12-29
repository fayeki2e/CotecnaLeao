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
  * Model for Leave Category
  */
    public partial class LeaveCategory: Entity<int>
    {
        public LeaveCategory()
        { }

        [Display(Name = "Leave Category")]
        public string Text { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}