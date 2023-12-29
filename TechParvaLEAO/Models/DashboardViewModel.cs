using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using TechParvaLEAO.Areas.Attendance.Models;

namespace TechParvaLEAO.Models
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            ExpenseListItems = new List<ExpenseListItemsViewModel>();
            AdvanceListItems = new List<AdvanceListItemsViewModel>();
            LeaveItems = new List<LeaveItemsViewModel>();
            TimesheetItems = new List<TimesheetItemsViewModel>();
            PendingDocuments = new List<PendingDocumentViewModel>();
        }
        public int EmployeeLeaveStatus { get; set; } = 0;
        public int ReimbursementClaims { get; set; } = 0;
        public int NoAdvanceClaims { get; set; } = 0;
        public double ReimbursementClaimedAmount { get; set; } = 0;
        public int EmployeeTimesheetApproved { get; set; } = 0;

        public int SupportingPendingToBeReceived { get; set; } = 0;
        public int ApprovedAdvanceRequest { get; set; } = 0;
        public int AdvanceRequestPaid { get; set; } = 0;
        public int ApprovedReimbursementUnpaid { get; set; } = 0;

        public double LeaveBalance { get; set; } = 0;
        public double CompOffAvailable { get; set; } = 0;
        public int AdvanceRequestPending { get; set; } = 0;
        public int ExpenseReimbursementUnpaid { get; set; } = 0;

        public int OvertimeRejectedEmployees { get; set; } = 0;
        public int ReimbursementPendingClaim { get; set; } = 0;
        public double ReimbursementClaimed { get; set; } = 0;

        public virtual IList<ExpenseListItemsViewModel> ExpenseListItems { get; set; }
        public virtual IList<AdvanceListItemsViewModel> AdvanceListItems { get; set; }
        public virtual IList<LeaveItemsViewModel> LeaveItems { get; set; }
        public virtual IList<TimesheetItemsViewModel> TimesheetItems { get; set; }
        public virtual TimeSheetOverviewViewModel ManagerTimesheets { get; set; }
        public virtual IList<PendingDocumentViewModel> PendingDocuments { get; set; }
    }

    public class ExpenseListItemsViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public string AppliedBy { get; set; }
        public string AppliedFor { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }

    public class AdvanceListItemsViewModel
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Claim Date")]
        public DateTime ClaimDate { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public string AppliedBy { get; set; }
        public string AppliedFor { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }

    public class PendingDocumentViewModel
    {
        public string RequestNumber { get; set; }
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Claim Date")]
        public int Id { get; set; }
        public DateTime ClaimDate { get; set; }
        public string Role { get; set; }
        public string Status { get; set; }
        public double Amount { get; set; }
        public string AppliedBy { get; set; }
        public bool HardCopyReceived { get; set; }
        public DateTime? ReceivedDate { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyName { get; set; }
    }

    public class LeaveItemsViewModel
    {
        public int Id { get; set; }
        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Apply Date")]
        public DateTime ApplyDate { get; set; }
        public double TotalDays { get; set; }
        public double BalanceLeaves { get; set; }
        public string Status { get; set; }
        public string Location { get; set; }
        public string Summary { get; set; }
        public string LeaveType { get; set; }
    }

    public class TimesheetItemsViewModel
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Designation { get; set; }
        public string WorkingHours { get; set; }
        public string OvertimeHours { get; set; }
        public double CompOffs { get; set; }
        public string Role { get; set; }
        public string RejectedBy { get; set; }
    }

   

}
