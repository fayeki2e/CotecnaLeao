using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Payment Request Payment Record
     */
    public class PaymentRequestPaymentRecord : Entity<int>
    {
        public PaymentRequestPaymentRecord() { }

        [Display(Name = "Payment Request")]
        [ForeignKey("PaymentRequest")]
        public virtual int PaymentRequestId { get; set; }
        public virtual PaymentRequest PaymentRequest { get; set; }
        public string TransactionType { get; set; }
        public string EmployeeCode { get; set; }
        public string BenificiaryCode { get; set; }
        public double TransactionAmount { get; set; }
        public string BenificiaryName { get; set; }
        public string PaymentDetail1 { get; set; }
        public string PaymentDetail2 { get; set; }
        public string PaymentDetail3 { get; set; }
        public string PaymentDetail4 { get; set; }
        public string PaymentDetail5 { get; set; }
        public string PaymentDetail6 { get; set; }
        public string PaymentDetail7 { get; set; }
        public string ChqTime { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
