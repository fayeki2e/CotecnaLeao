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
     * Model for Payment Request Series Sequence
     */
    public partial class PaymentRequestSeriesSequence: Entity<int>
    {
        public PaymentRequestSeriesSequence()
        { }
        public string AdvanceExpense { get; set; }
        public string LocationCode { get; set; }
        public string FinancialYear { get; set; }
        public int SequenceNumber { get; set; }
    }
}