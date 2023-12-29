using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Organization.Models;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
  * View Model for Advance Request
  */
    public class AdvanceViewModel: IRequest<bool>
    {
        [Required(ErrorMessage = "Please enter Comment.")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Please Select Employee.")]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; }
        [Required(ErrorMessage = "Please Select Currency.")]
        [Display(Name = "Currency")]
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        [Display(Name = "Customer Market")]
        [Required]
        public string CustomerMarketId { get; set; }
        [Display(Name = "Business Activity")]
        [Required]
        public string BusinessActivityId { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Enter amount")]
        [Range(1.00, double.MaxValue, ErrorMessage = "Amount cannot be zero")]
        public double? Amount { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public string DraftId { get; set; }
    }

    public class DateAttribute : RangeAttribute
    {
        public DateAttribute()
          : base(typeof(DateTime), new DateTime(2000,01,01).ToString("yyyy-MM-dd"), DateTime.Now.ToString("yyyy-MM-dd")) { }
    }

    /*
  * View Model for Expense Request
  */
    public class ExpenseViewModel : IRequest<bool>, IValidatableObject
    {
        public ExpenseViewModel()
        {
            ExpenseLineItems = new List<ExpenseLineItemsViewModel>();
        }
        [Required(ErrorMessage = "Please enter Comment.")]
        public string Comment { get; set; }
        [Required(ErrorMessage = "Please Select Employee.")]
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        [Display(Name = "Request Number")]
        public string RequestNumber { get; set; }
        [Required(ErrorMessage = "Please Select Currency.")]
        [Display(Name = "Currency")]
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        [Display(Name = "Customer Market")]
        public string CustomerMarketId { get; set; }
        [Display(Name = "Business Activity")]
        public string BusinessActivityId { get; set; }
        public string Type { get; set; }
        [Required(ErrorMessage = "Enter amount")]
        [Range(1.00, double.MaxValue, ErrorMessage = "Amount cannot be zero")]
        public double? Amount { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public List<IFormFile> Supportings { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        [Required]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [DataType(DataType.Date)]
        [Display(Name = "To Date")]
        [Date(ErrorMessage ="To Date cannot be in future")]
        [Required]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Advance Payment to Settle Against")]
        public int? AdvancePaymentRequestId { get; set; }

        public string AdvancePaymentRequest { get; set; }

        public int? LoggedInEmployeeId { get; set; }

        [Display(Name = "Credit Card")]
        public bool CreditCard { get; set; }

        public bool CanHoldCreditCard { get; set; }

        public string DraftId { get; set; }
        public virtual IList<ExpenseLineItemsViewModel> ExpenseLineItems { get; set; }
      
        [Display(Name = "Operation Type")]
        public string operationtype { get; set; }

        [Display(Name = "Operation Number")]
        public string operationno { get; set; }


        public string fake_path { get; set; }

        public IEnumerable<ValidationResult> Validate(System.ComponentModel.DataAnnotations.ValidationContext validationContext)
        {
            var i = 0;
            foreach (var lineItem in ExpenseLineItems)
            {
                if (lineItem.Date < FromDate || lineItem.Date > ToDate)
                {
                    yield return new ValidationResult("Exepnse date must be between Start Date and End Date",  
                        new String[] { string.Format("ExpenseLineItems["+i+"].Date", i) });

                }
                i++;
            }
        }
    }

    /*
  * View Model for Expense Edit
  */
    public class EditExpenseViewModel
    {
        public EditExpenseViewModel(){
            ExpenseLineItems = new List<ExpenseLineItemsViewModel>();
        }
        public string Comment { get; set; }
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string RequestNumber { get; set; }
        [Display(Name = "Currency")]
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
        [Display(Name = "Business Activity")]
        public string BusinessActivityId { get; set; }
        public string Type { get; set; }
        public double Amount { get; set; }
        public int CreatedByEmployeeId { get; set; }

        public List<IFormFile> Supportings { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [Display(Name = "Advance Payment to Settle Against")]
        public int? AdvancePaymentRequestId { get; set; }

        public string AdvancePaymentRequest { get; set; }

        public int LoggedInEmployeeId { get; set; }

        [Display(Name = "Credit Card")]
        public bool CreditCard { get; set; }

        public bool CanHoldCreditCard { get; set; }

        public virtual IList<ExpenseLineItemsViewModel> ExpenseLineItems { get; set; }

    }

    /*
  * View Model for Expense Line Items
  */
    public class ExpenseLineItemsViewModel
    {
        public int? Id { get; set;}
        [Display(Name = "Operation Type")]
        public string operationtype { get; set; }

        [Display(Name = "Operation Number")]
        public string operationno { get; set; }

        [Required(ErrorMessage = "Please select expense head")]
        public string ExpenseHead { get; set; }

        [Required(ErrorMessage = "Please select business activity")]
        public string BusinessActivity { get; set; }

        [Required(ErrorMessage = "Please select Customer Market")]
        public string CustomerMarket { get; set; }

        [Required(ErrorMessage = "Please enter expense voucher reference number")]
        [MaxLength(50)]
        public string ExpenseVoucherReferenceNo { get; set; }

        [Required(ErrorMessage = "Please select description")]
        [MaxLength(50)]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Required(ErrorMessage = "Please enter expense date")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [Required(ErrorMessage = "Enter expense amount")]
        [Range(1.00,double.MaxValue, ErrorMessage ="Amount cannot be zero")]
        public double? Amount { get; set; }
    }

    /*
  * View Model for Payment Request Approve Reject
  */
    public class PaymentRequestApproveRejectViewModel : IRequest<bool>
    {
        public int RejectionReasonId { get; set; }
        public int PaymentRequestId { get; set; }
        public int[] PaymentRequestsId { get; set; }
        public int ActionById { get; set; }
        [Display(Name ="Other Reason")]
        public string RejectionReasonOther { get; set; }
    }

    /*
  * View Model for Payment Mode
  */
    public class PaymentModeViewModel : IRequest<bool>
    {
        public int ActionById { get; set; }
        public int PaymentRequestId { get; set; }
        public string PaymentMode { get; set; }
    }

    /*
  * View Model for Employee Settlement
  */
    public class EmployeeSettlementViewModel : IRequest<bool>
    {
        public Employee Employee { get; set; }
        public int ActionById { get; set; }
        public int EmployeeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SettlementDate { get; set; }
        public double SettlementAmount { get; set; }
    }

    /*
  * View Model for Supporting Received
  */
    public class SupportingReceivedViewModel : IRequest<bool>
    {
        public int ActionById { get; set; }
        public int PaymentRequestId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }

    /*
  * View Model for Forex Settlement
  */
    public class ForexSettlementViewModel : IRequest<bool>
    {
        public int ActionById { get; set; }
        public int PaymentRequestId { get; set; }
        public double ForexRate { get; set; }
    }

    /*
  * View Model for Payment Request Posted
  */
    public class PaymentRequestPostedViewModel : IRequest<bool>
    {
        public int[] PaymentRequestId { get; set; }
        public int ActionById { get; set; }
    }

    /*
  * View Model for Payment Request Paid
  */
    public class PaymentRequestPaidViewModel : IRequest<bool>
    {
        public IEnumerable<PaymentUploadRecord> PaymentUploadRecords { get; set; }
        public string ActionById { get; set; }
    }

    /*
  * View Model for Payment Upload Record
  */
    public class PaymentUploadRecord
    {


    }

    /*
  * View Model for Base Reminder
  */
    public class BaseReminderViewModel : IRequest<bool>
    {
        [Display(Name = "For Date")]
        public DateTime ForDate { get; set; }
    }

    /*
  * View Model for Pending Document Confirm
  */
    public class PendingDocumentConfirmViewModel : IRequest<bool>
    {
        public int PaymentRequestId { get; set; }
        public int ActionById { get; set; }
    }

    /*
 * View Model for Advance Reminder
 */
    public class AdvanceReminderViewModel : BaseReminderViewModel{}

    /*
 * View Model for Advance Fianl Reminder
 */
    public class AdvanceFianlReminderViewModel : BaseReminderViewModel{}

    /*
 * View Model for Advance Reminder More Than Three Days
 */
    public class AdvanceReminderMoreThanThreeDaysViewModel : BaseReminderViewModel { }

    /*
 * View Model for Expense Reminder
 */
    public class ExpenseReminderViewModel : BaseReminderViewModel { }

    /*
 * View Model for Expense Final Reminder
 */
    public class ExpenseFinalReminderViewModel : BaseReminderViewModel { }

    /*
 * View Model for Finance Advance Reminder
 */
    public class FinanceAdvanceReminderViewModel : BaseReminderViewModel { }

    /*
 * View Model for Finance Expense Reminder
 */
    public class FinanceExpenseReminderViewModel : BaseReminderViewModel { }

    /*
 * View Model for Document Submission Reminder
 */
    public class DocumentSubmissionReminderViewModel : BaseReminderViewModel { }

    /*
* View Model for Advance Finance Reminder
*/
    public class AdvanceFinanceReminderViewModel : BaseReminderViewModel { }

    /*
* View Model for Expense Finance Reminder
*/
    public class ExpenseFinanceReminderViewModel : BaseReminderViewModel { }

    /*
* View Model for Employee Expense Tracker Report
*/
    public class EmployeeExpenseTrackerReportVM : IRequest<bool>
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeCode { get; set; }

        public double Amount { get; set; }     

        public string Location { get; set; }

        public DateTime DateofExpense { get; set; }

        public DateTime DateofApproval { get; set; }

        public DateTime DateofPayment { get; set; }

        public string ExpenseDiscription { get; set; }

        public string NoOfDays { get; set; }
    }

    /*
* View Model for Employee Advance Tracker Report
*/
    public class EmployeeAdvanceTrackerReportVM : IRequest<bool>
    {
        public int Id { get; set; }

        public string EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        public string EmployeeCode { get; set; }

        public double Amount { get; set; }

        public string Location { get; set; }

        public DateTime DateofAdvance { get; set; }

        public DateTime DateofApproval { get; set; }

        public DateTime DateofPayment { get; set; }

        public string AdvanceDiscription { get; set; }

        public string NoOfDays { get; set; }
    }

    /*
* View Model for Unsettled Advance Report
*/
    public class UnsettledAdvanceReportVM : IRequest<bool>
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public double Amount { get; set; }

        public DateTime DateofPayment { get; set; }

        public string NoOfDays { get; set; }
    }

    /*
* View Model for Reports Search
*/
    public class ReportsSearchVM : IRequest<bool>
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }

       
    }

    /*
* View Model for Hard Copy Supporting Not Submitted Report
*/
    public class HardCopySupportingNotSubmittedReportVM : IRequest<bool>
    {
        public int Id { get; set; }

        public string Location { get; set; }

        public string EmployeeCode { get; set; }

        public string EmployeeName { get; set; }

        public double Amount { get; set; }

        public DateTime DateofExpenseClaim { get; set; }

        public DateTime DateofApproval { get; set; }

        public string NoOfDays { get; set; }
    }
}
