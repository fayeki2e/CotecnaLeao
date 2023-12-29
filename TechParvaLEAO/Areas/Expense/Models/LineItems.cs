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
     * Model for Payment Request Line Items
     */
    public partial class PaymentRequestLineItems : Entity<int>
    {
        public PaymentRequestLineItems()
        { }

        [Display(Name = "Payment Request")]
        [ForeignKey("PaymentRequest")]
        public virtual int? PaymentRequestId { get; set; }
        public virtual PaymentRequest PaymentRequest { get; set; }

        [Display(Name = "Operation Type")]
        public string operationtype { get; set; }

        [Display(Name = "Operation Number")]
        public string operationno { get; set; }

        [Display(Name = "Expense Head")]
        [ForeignKey("ExpenseHead")]
        public virtual int ExpenseHeadId { get; set; }
        public virtual ExpenseHead ExpenseHead { get; set; }

        [Display(Name = "Business Activity")]
        [ForeignKey("BusinessActivity")]
        public virtual int BusinessActivityId { get; set; }
        public virtual BusinessActivity BusinessActivity { get; set; }

        [Display(Name = "Customer Market")]
        [ForeignKey("CustomerMarket")]
        public virtual int CustomerMarketId { get; set; }
        public virtual CustomerMarket CustomerMarket { get; set; }

        public double Amount { get; set; }

        [Display(Name = "Currency")]
        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        public string ExpenseVoucherReferenceNumber { get; set; }
        public string VoucherDescription { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime ExpenseDate { get; set; }

        [NotMapped]
        public double INRAmount { get
            {
                if(PaymentRequest.CurrencyId == 1)
                {
                    return Amount;
                }
                else
                {
                    return Amount * PaymentRequest.ExchangeRate;
                }
            } 
        }

        [NotMapped]
        public double FxAmount
        {
            get
            {
                if (PaymentRequest.CurrencyId == 1)
                {
                    return 0.00;
                }
                else
                {
                    return Amount;
                }
            }
        }

    }
}