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
* Model for Leave Type
*/
    public partial class LeaveType: Entity<int>
    {
        public LeaveType() { }        
        [Display(Name="Leave Type")]
        public string Name { get; set; }
        public bool Paid { get; set; }
        public bool Limit { get; set; }
        public int Order { get; set; }
        public bool IsMaternity { get; set; }
        public bool IsMission { get; set; }
        public bool IsCommon { get; set; }
        public bool Deactivated { get; set; } = false;
    }

}