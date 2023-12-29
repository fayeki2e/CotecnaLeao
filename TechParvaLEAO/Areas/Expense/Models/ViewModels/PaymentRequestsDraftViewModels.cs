using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
   * View Model for Advance Draft
   */
    public class AdvanceDraftViewModel
    {
        public string Comment { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RequestNumber { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CustomerMarketId { get; set; }
        public string BusinessActivityId { get; set; }
        public string Type { get; set; }
        public double? Amount { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public string DraftId { get; set; }
    }

    /*
   * View Model for Expense Draft Line Items
   */
    public class ExpenseDraftLineItemsViewModel
    {
        public int? Id { get; set; }
        public string ExpenseHead { get; set; }

        public string BusinessActivity { get; set; }

        public string CustomerMarket { get; set; }

        public string ExpenseVoucherReferenceNo { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? Date { get; set; }

        public double? Amount { get; set; }
    }

    /*
   * View Model for Expense Draft
   */
    public class ExpenseDraftViewModel
    {
        public ExpenseDraftViewModel()
        {
            ExpenseLineItems = new List<ExpenseDraftLineItemsViewModel>();
        }
        public string Comment { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RequestNumber { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        public string CustomerMarketId { get; set; }
        public string BusinessActivityId { get; set; }
        public string Type { get; set; }
        public double? Amount { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }
        public int? AdvancePaymentRequestId { get; set; }
        public string AdvancePaymentRequest { get; set; }
        public int? LoggedInEmployeeId { get; set; }
        public bool CreditCard { get; set; }
        public bool CanHoldCreditCard { get; set; }
        public string DraftId { get; set; }
        public virtual IList<ExpenseDraftLineItemsViewModel> ExpenseLineItems { get; set; }
    }

    /*
   * View Model for Advance Draft List Row
   */
    public class AdvanceDraftListRow
    {
        public PaymentRequestDraft Draft { get; set; }
        public AdvanceViewModel ViewModel { get; set; }
    }

    /*
   * View Model for Expense Draft List Row
   */
    public class ExpenseDraftListRow
    {
        public PaymentRequestDraft Draft { get; set; }
        public ExpenseViewModel ViewModel { get; set; }
    }
}