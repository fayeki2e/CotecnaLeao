using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
     * Model for Approval Limit Profile
     */
    public partial class ApprovalLimitProfile: Entity<int>
    {
        public ApprovalLimitProfile()
        {

        }
        [Display(Name = "Approval Limit Profile Name")]
        public string Name { get; set; }

        public double Approval_Limit { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}