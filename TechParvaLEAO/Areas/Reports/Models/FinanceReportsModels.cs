using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Reports.Models
{
    public class FinanceReportSearchVm
    {
        [Key]
        int? Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpenseFromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ExpenseToDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AdvanceFromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? AdvanceToDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ApproveRejectFromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ApproveRejectToDate { get; set; }
        public int? Branch { get; set; }
        public string BranchName { get; set; }
        public int? AmountGt { get; set; }
        public int? AmountLt { get; set; }
        public int? BusinessActivity { get; set; }
        public string BusinessActivityName { get; set; }
        public int? CustomerMarket { get; set; }
        public string CustomerMarketName { get; set; }
        public int? Employee { get; set; }
        public string EmployeeName { get; set; }
        public string AccountNumber { get; set; }
        public int? SubmittedBy { get; set; }
        public string SubmittedByName { get; set; }
        public int? ApproveRejectedBy { get; set; }
        public string ApproveRejectedByName { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public int is_reporting_exist { get; set; }
        public int current_emp_id { get; set; }

    }

    public class BaseFinanceReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Sr. No.")]
        public int SerialNumber { get; set; }
        [DisplayFormat(DataFormatString = "{0:MMM-yyyy}")]
        public DateTime? Month { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "Expense Date")]
        public DateTime ExpenseDate { get; set; }
        public string Branch { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:##,##,##0.00}")]
        public double Amount { get; set; }
        public string Currency { get; set; }

        public string str_date { get; set; }

    }

    public class TrackerReportVm: BaseFinanceReportVm
    {
        [Display(Name = "Submission Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime SubmissionDate { get; set; }
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]

        public DateTime SubmissionMonth { get; set; }
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]

        public DateTime? ApprovalDate { get; set; }
        [Display(Name = "Paid Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? PaidDate { get; set; }
        public string Description { get; set; }
        [Display(Name = "Paid In Days")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public double PaidInDays { get; set; }
    }

    public class EmployeeExpenseTrackerReportVm : TrackerReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
    }

    public class EmployeeAdvanceTrackerReportVm : TrackerReportVm
    {
        [Display(Name = "Advance Request Number")]
        public string PaymentRequestNumber { get; set; }
    }

    public class UnsettledAdvanceReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Advance Request Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Paid Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime PaidDate { get; set; }
        [Display(Name = "Days Since Paid")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public int DaysSincePaid { get; set; }
    }

    public class SettledAdvanceReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Advance Request Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Paid Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime PaidDate { get; set; }
        [Display(Name = "Claim Submission Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ClaimSubmissionDate { get; set; }
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ApprovalDate { get; set; }
        [Display(Name = "Days Since Paid")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public double DaysSincePaid { get; set; }
    }

    public class HardcopyPendingReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Claim Submission Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ClaimSubmissionDate { get; set; }
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ApprovalDate { get; set; }
        [Display(Name = "Days Since Approved")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public double DaysSincePaid { get; set; }
    }

    public class ExpenseTransactionSummaryReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Claim Amount")]
        new public double Amount { get; set; }

        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
        public string Comment { get; set; }
        [Display(Name = "Approved/ Rejected")]
        public string ApprovedRejected { get; set; }
        [Display(Name = "Approved/ Rejected Escalated")]
        public string ApprovedRejectedEscalated { get; set; }
        [Display(Name = "Status")]
        public string Status { get; set; }
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }
        [Display(Name = "Approved/ Rejected By")]
        public string ApproveRejectedBy { get; set; }
        [Display(Name = "Esclated To")]
        public string ApproveRejectEscalatedTo { get; set; }
        [Display(Name = "Rejected Reason")]
        public string RejectionReason { get; set; }
        [Display(Name = "Adjusted Against")]
        public string AdjustedAgainst { get; set; }
        [Display(Name = "Amount Of Claim")]
        public double AmountOfClaim { get; set; }
        [Display(Name = "Amount Of Advance")]
        public double AmountOfAdvance { get; set; }
        [Display(Name = "Forex Rate")]
        public double ForexRate { get; set; }
        [Display(Name = "Supportings In System")]
        public int SupportingsInSystem { get; set; }
        [Display(Name = "Hard Copy Submitted")]
        public string HardcopySubmitted { get; set; }
        [Display(Name = "Number of Line Items")]
        public int NumberOfLineItems { get; set; }
        [Display(Name = "Credit Card")]
        public string CreditCard { get; set; }
        [Display(Name = "Employee Status")]
        public string EmployeeStatus { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
    }

    public class ExpenseTransactionDetailReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Expense Head")]
        public string ExpenseHead { get; set; }
        public string Comment { get; set; }
        [Display(Name = "Operation Number")]
        public string OperationNumber { get; set; }
        public string Description { get; set; }
        [Display(Name = "Business Activity")]
        public string BusinessActivity { get; set; }
        [Display(Name = "Customer Market")]
        public string CustomerMarket { get; set; }
        [Display(Name = "Approved/ Rejected")]
        public string ApprovedRejected { get; set; }
        public string Status { get; set; }
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }
        [Display(Name = "Approved/ Rejected By")]
        public string ApproveRejectedBy { get; set; }
        [Display(Name = "Escalated To")]
        public string ApproveRejectEscalatedTo { get; set; }
        [Display(Name = "Approve/ Reject Escalated")]
        public string ApproveRejectEscalated { get; set; }
        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }
        [Display(Name = "Adjusted Against")]
        public string AdjustedAgainst { get; set; }
        [Display(Name = "Currency")]
        public double AmountOfClaim { get; set; }
        [Display(Name = "Amount of Advance")]
        public double AmountOfAdvance { get; set; }
        [Display(Name = "Forex Rate")]
        public double ForexRate { get; set; }
        [Display(Name = "Supportings in System")]
        public int SupportingsInSystem { get; set; }
        [Display(Name = "Hardcopy Submitted")]
        public string HardcopySubmitted { get; set; }
        [Display(Name = "Number of Line Items")]
        public int NumberOfLineItems { get; set; }
        [Display(Name = "Credit Card")]
        public string CreditCard { get; set; }
        [Display(Name = "Employee Status")]
        public string EmployeeStatus { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }

        public string operationtype { get; set; }
        public string operationno { get; set; }

    }

    public class RejectedAdvanceReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Advance Request Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }
        [Display(Name = "Rejected By")]
        public string RejectedBy { get; set; }
    }

    public class RejectedExpensesReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Rejection Reason")]
        public string RejectionReason { get; set; }
        [Display(Name = "Rejected By")]
        public string RejectedBy { get; set; }
    }


    public class UnapprovedAdvanceExpensesReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Advance Request Number")]
        public string AdvancePaymentRequestNumber { get; set; }
        [Display(Name = "Submitted By")]
        public string SubmittedBy{ get; set; }
        [Display(Name = "Last Approved By")]
        public string LastApprovedBy { get; set; }
        public string Status { get; set; }
    }

    public class FNFAdvancesReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Expense Claim Number")]
        public string ExpensePaymentRequestNumber { get; set; }
        [Display(Name = "Advance Request Number")]
        public string AdvancePaymentRequestNumber { get; set; }
        [Display(Name = "Submitted By")]
        public string SubmittedBy { get; set; }
        [Display(Name = "Last Approved By")]
        public string LastApprovedBy { get; set; }
        public string Status { get; set; }
    }

    public class AdvanceTransactionDetailReportVm : BaseFinanceReportVm
    {
        [Display(Name = "Advance Request Number")]
        public string PaymentRequestNumber { get; set; }
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        [Display(Name = "Business Activity")]
        public string BusinessActivity { get; set; }
        [Display(Name = "Customer Market")]
        public string CustomerMarket { get; set; }
        public string Status { get; set; }
        [Display(Name = "Approved/Rejected By")]
        public string ApproveRejectedBy { get; set; }
        [Display(Name = "Rejection Reason")]
        public string RejectedReason { get; set; }
    }

    public class BalancePayableOrReceivableReportVm : BaseFinanceReportVm
    {
       
        [Display(Name = "Balance")]
        public string Balance { get; set; }

        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }


        public int EmpId { get; set; }
    }

}
