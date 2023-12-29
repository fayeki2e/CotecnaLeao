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
     * Model for Customer Market
     */
    public partial class CustomerMarket : Entity<int>
    {
        public CustomerMarket()
        {
        }
        public string Name { get; set; }
        public string Code { get; set; }

        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int? LocationId { get; set; }
        public virtual Location Location { get; set; }
        [Display(Name = "VOC")]
        public bool IsVOC { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}