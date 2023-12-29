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
 * Model for Leave Request Action
 */
    public partial class LeaveRequestAction: Entity<int>
    {
        public LeaveRequestAction()
        { }

        [Display(Name = "Leave Request")]     
        public virtual int LeaveRequestId { get; set; }
        public virtual LeaveRequest LeaveRequest { get; set; }

        public string Action { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Timestamp { get; set; }

        [Display(Name = "Action By")]
        [ForeignKey("ActionBy")]
        public virtual int ActionById { get; set; }
        public virtual Employee ActionBy { get; set; }
    }
}