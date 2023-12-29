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
    * Model for Expense Profile
    */
    public partial class ExpenseProfile : Entity<int>
    {
        public ExpenseProfile()
        {
        }
        [Display(Name="Expense Profile")]
        public string Name { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}