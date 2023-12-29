using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
     * View Model for Expense Head Search
     */
    public class ExpenseHeadSearchVM
    {
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Expense Head Desc")]
        public string ExpenseHeadDesc { get; set; }
    }
}
