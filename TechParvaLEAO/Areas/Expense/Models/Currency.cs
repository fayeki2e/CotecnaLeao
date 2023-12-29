using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Currency
     */
    public partial class Currency : Entity<int>
    {
        public Currency()
        {
        }
        public string Code { get; set; }
        public string Name { get; set; }
        [Display(Name = "Is Forex?")]
        public bool IsForex { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}