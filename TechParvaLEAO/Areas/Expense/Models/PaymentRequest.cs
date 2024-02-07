using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Payment Request
     */
    public partial class PaymentRequest : Entity<int>, IAggregateRoot
    {
        public PaymentRequest()
        {
        }
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; }

        public int? VersionNumber { get; set; }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        [Required(ErrorMessage = "Please Select Employee.")]
        public virtual int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        public double Amount { get; set; }
        public double ForexAmount { get; set; }
        public double ForexAmountDisplay()
        {
            if (CurrencyId == 1)
            {
                return 0.00;
            }
            else
            {
                return Amount;
            }
        }

        public double ExchangeRate{ get; set; }
        public bool CreditCard { get; set; } = false;
        public double INRAmount { get; set; }

        public double INRAmountDisplay()
        {
            if (CurrencyId == 1)
            {
                return Amount;
            }
            else
            {
                return INRAmount;
            }
        }
        public string SettlementMode { get; set; }

        public double BalanceAmount { get; set; }

        public double PaidAmount { get; set; }

        public double ClaimedAmount { get; set; }

        [Required(ErrorMessage = "Please Select Currency.")]
        [Display(Name = "Currency")]
        [ForeignKey("Currency")]
        public virtual int CurrencyId { get; set; }
        public virtual Currency Currency { get; set; }

        [Display(Name = "Financial year")]
        [ForeignKey("FinancialYear")]
        public virtual int FinancialYearId { get; set; }
        public virtual FinancialYear FinancialYear { get; set; }
        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int LocationId { get; set; }
        public virtual Location Location { get; set; }

        [Display(Name = "Business Activity")]
        [ForeignKey("BusinessActivity")]
        public virtual int? BusinessActivityId { get; set; }
        public virtual BusinessActivity BusinessActivity { get; set; }

        [Display(Name = "Customer Market")]
        [ForeignKey("CustomerMarket")]
        public virtual int? CustomerMarketId { get; set; }
        public virtual CustomerMarket CustomerMarket { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Created Date")]
        public DateTime PaymentRequestCreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "PaymentRequest Created By")]
        [ForeignKey("PaymentRequestCreatedBy")]
        public virtual int? PaymentRequestCreatedById { get; set; }
        public virtual Employee PaymentRequestCreatedBy { get; set; }

        [Display(Name = "PaymentRequest Actioned By")]
        [ForeignKey("PaymentRequestActionedBy")]
        public virtual int? PaymentRequestActionedById { get; set; }
        public virtual Employee PaymentRequestActionedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Action Date")]
        public DateTime? ActionDate { get; set; }
        
        [Display(Name = "Posted By")]
        [ForeignKey("PostedBy")]
        public virtual int? PostedById { get; set; }
        public virtual Employee PostedBy { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Posted On")]
        public DateTime? PostedOn { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Paid On")]
        public DateTime? PaidDate { get; set; }

        [Required(ErrorMessage = "Please Select Applicant type.")]
        public string Type { get; set; }
        
        public string Status { get; set; }
        [Required(ErrorMessage = "Please enter an Comment.")]
        public string Comment { get; set; }

        public string operationtype { get; set; }
        public string operationno { get; set; }

        [Display(Name = "Initial Advance Payment Request")]
        [ForeignKey("AdvancePaymentRequest")]
        public virtual int? AdvancePaymentRequestId { get; set; }
        public virtual PaymentRequest AdvancePaymentRequest { get; set; }

        [Display(Name = "Rejection Reason")]
        [ForeignKey("RejectionReasons")]
        public virtual int? RejectionReasonsId { get; set; }
        public virtual PaymentRequestRejectionReason RejectionReasons { get; set; }

        [Display(Name = "Other Rejection Reason")]
        public string RejectionReasonOther { get; set; }

        public virtual IEnumerable<PaymentRequestApprovalAction> ApprovalActions { get; set; }
        public virtual IEnumerable<PaymentRequestLineItems> LineItems { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Supporting Documents Received Date")]
        public DateTime? SupportingDocumentsReceivedDate { get; set; }

        [Display(Name = "Supporting Documents Comment ")]
        public string SupportingDocumentsComment { get; set; }

        [Display(Name = "Supporting Documents")]
        public string SupportingDocumentsPath { get; set; }

        public List<string> GetSupportingDocuments()
        {
            string paths = SupportingDocumentsPath;
            if (!string.IsNullOrEmpty(paths))
            {
                return paths.Split("*").ToList();
            }
            return new List<string>();
        }

        public bool Settled { get; set; }

        public double BalanceThisExpense
        {
            get
            {
                return this.Amount - this.PaidAmount;
            }
        }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? DownloadedDate { get; set; } = DateTime.Today;

        public string EscalationStatus
        {
            get
            {
                var result = "";
                var actionCount = 0;
                foreach (var action in this.ApprovalActions)
                {
                    if (PaymentRequestStatus.APPROVED.ToString().Equals(action.Action)||
                        PaymentRequestStatus.REJECTED.ToString().Equals(action.Action)
                        ){
                        result = result + action.Action.ToString() + " ";
                        actionCount++;
                    }
                }
                for (var i = actionCount; i < 3; i++)
                {
                    result = result + "NA" + " ";
                }
                return result;
            }
        }
        public string EscalationApprover
        {
            get
            {
                var result = "";
                var actionCount = 0;
                foreach (var action in this.ApprovalActions)
                {
                    if (PaymentRequestStatus.APPROVED.ToString().Equals(action.Action) ||
                        PaymentRequestStatus.REJECTED.ToString().Equals(action.Action)
                        )
                    {
                        result = result + action.ActionBy.Name.ToString() + " ";
                        actionCount++;
                    }
                }
                for (var i = actionCount; i<3; i++)
                {
                    result = result + "NA" + " ";
                }
                return result;
            }
        }

    }

    public partial class AdvanceExpenseAdjustment : Entity<int>
    {
        public AdvanceExpenseAdjustment()
        {
        }
        [ForeignKey("Expense")]
        public virtual int? ExpenseId { get; set; }
        [ForeignKey("Advance")]
        public virtual int? AdvanceId { get; set; }
        public virtual PaymentRequest Expense { get; set; }
        public virtual PaymentRequest Advance { get; set; }
        public double AdjustedAdvanceAmount { get; set; }
    }

    /*
     * Constant for Payment Request Type
     */
    public enum PaymentRequestType
    {
        [Description("ADV")]
        ADVANCE,
        [Description("EXP")]
        REIMBURSEMENT
    }

    /*
    * Constant for Payment Request Status
    */
    public enum PaymentRequestStatus
    {
        PENDING,
        APPROVED,
        APPROVED_ESCALATED,
        REJECTED,
        POSTED,
        PAID
    }

    /*
   * Constant for Payment Request Actions
   */
    public enum PaymentRequestActions
    {
        SAVED,
        SUBMITTED,
        REMINDER,
        FINAL_REMINDER,
        APPROVED,
        REJECTED,
        POSTED,
        PAID,
        EDITED,
        SUPPORTING_RECEIVED,
        FOREX_RATE_UPDATE,
        PAYMENT_MODE_UPDATE
    }
}