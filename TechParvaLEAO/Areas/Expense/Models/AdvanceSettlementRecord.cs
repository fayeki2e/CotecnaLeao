using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Advance Settlement Record
     */
    public partial class AdvanceSettlementRecord : Entity<int>, IAggregateRoot
    {
        public AdvanceSettlementRecord()
        {
        }

        [Display(Name = "Advance Payment Request")]
        [ForeignKey("PaymentRequest")]
        public virtual int AdvancePaymentRequestId { get; set; }
        public virtual PaymentRequest AdvancePaymentRequest { get; set; }
        [Display(Name = "Reimbursement Payment Request")]
        [ForeignKey("PaymentRequest")]
        public virtual int ReimbursementPaymentRequestId { get; set; }
        public virtual PaymentRequest ReimbursementPaymentRequest { get; set; }
        public double OpeningBalanceOfAdvance { get; set; }
        public double ReimbursementAmount { get; set; }
        public double ClosingBalanceOfReimbursementAmount { get; set; }
    }
}
