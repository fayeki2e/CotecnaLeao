using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MediatR;

namespace TechParvaLEAO.Areas.Expense.Models
{
   /*
   * View Model for Payment Request Email Model
   */
    public class PaymentRequestEmailModel
    {

        public PaymentRequestEmailModel()
        {
            PaymentRequestDetails = new List<PaymentRequestDetails>();
        }

        public int Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime ToDate { get; set; }
        public string Employee { get; set; }
        public string EmployeeCode { get; set; }
        public string ReportingManager { get; set; }
        public string Currency { get; set; }
        public string RequestNumber { get; set; }
        public double Amount { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime PostedOn { get; set; }
        public string RejectionReason { get; set; } 
        public string PaymentMode { get; set; }
        public string UnsettledCurrency { get; set; }
        public double UnsettledAmount { get; set; }
        public string ActionByEmployee { get; set; }
        public string NextActionByEmployee { get; set; }

        public virtual IList<PaymentRequestDetails> PaymentRequestDetails { get; set; }
    }

    public class PaymentRequestDetails
    {

    }

}
