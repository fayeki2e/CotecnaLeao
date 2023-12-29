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
     * Model for Payment Request Rejection Reason 
     */
    public partial class PaymentRequestRejectionReason: Entity<int>
    {
        public PaymentRequestRejectionReason()
        { }

        public string Reason { get; set; }

        public string Type { get; set; }
    }
}