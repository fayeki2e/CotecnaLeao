using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Expense Head Expense Profile Mapping
     */
    public partial class ExpenseHeadExpenseProfileMapping : Entity<int>
    {
        public ExpenseHeadExpenseProfileMapping()
        { }

        [Display(Name = "Expense Head")]
        [ForeignKey("ExpenseHead")]
        public virtual int ExpenseHeadId { get; set; }
        public virtual ExpenseHead ExpenseHead { get; set; }

        [Display(Name = "Expense Profile")]
        [ForeignKey("ExpenseProfile")]
        public virtual int ExpenseProfileId { get; set; }
        public virtual ExpenseProfile ExpenseProfile { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}