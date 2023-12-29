using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TechParvaLEAO.Areas.Organization.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Expense Head
     */
    public partial class ExpenseHead : Entity<int>
    {
        public ExpenseHead()
        { }
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Expense Head Desc")]
        public string ExpenseHeadDesc { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}