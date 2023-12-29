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
     * Model for Payment Request Approval Action
     */
    public partial class PaymentRequestApprovalAction: Entity<int>
    {
        public PaymentRequestApprovalAction()
        { }

        [Display(Name = "Payment Request")]
        [ForeignKey("PaymentRequest")]
        public virtual int? PaymentRequestId { get; set; }
        public virtual PaymentRequest PaymentRequest { get; set; }

        [Display(Name = "Action By")]
        [ForeignKey("ActionBy")]
        public virtual int ActionById { get; set; }
        public virtual Employee ActionBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Timestamp { get; set; }

        public string Type { get; set; }

        public string Action { get; set; }

        public string Note { get; set; }
    }
}