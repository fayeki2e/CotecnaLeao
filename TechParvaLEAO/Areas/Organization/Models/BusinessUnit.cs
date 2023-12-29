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
     * Model for Business Unit
     */
    public partial class BusinessUnit: Entity<int>
    {
        public BusinessUnit()
        {

        }
        [Display(Name = "BU Name")]
        public string Name { get; set; }

        [Display(Name = "BU Head")]
        [ForeignKey("Employee")]
        public virtual int? BUHeadId { get; set; }
        public virtual Employee BUHead { get; set; }
    }
}