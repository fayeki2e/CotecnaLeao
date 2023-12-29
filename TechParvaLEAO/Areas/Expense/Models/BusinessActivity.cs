using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Business Activity
     */
    public partial class BusinessActivity: Entity<int>
    {
        public BusinessActivity()
        {
        }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Display(Name="VOC")]
        public bool IsVOC { get; set; }
        public virtual int? BusinessUnitid { get; set; }
        public virtual BusinessUnit BusinessUnit { get; set; }

        public bool Deactivated { get; set; } = false;

        public string DisplayBusinessName
        {
            get
            {
                return this.Code + " - " + this.Name;
            }
        }

    }
}