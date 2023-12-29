using MediatR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Handler;
using Microsoft.AspNetCore.Http;

namespace TechParvaLEAO.Areas.Leave.Models.ViewModels
{
    /*
    * View Model for New Leave
    */
    public class NewLeaveViewModel : IRequest<Result>, IValidatableObject
    {
        public NewLeaveViewModel()
        {
            Holidays = new List<HolidayVm>();
        }
        public List<IFormFile> Documents { get; set; }

        public int EmployeeId { get; set; }
        public string EmployeeName;
        public string EmployeeCode;
        public string LeaveApprover;
        public double AnnualLeaveEligibility;
        public double LeavesUtilized;
        public double LeavesPendingApproval;
        public double LeavesCarryForward;
        public double AnnualLeaves;
        public double LeavesWithoutPay;
        public double LeaveBalance;
        public string EmployeeLocation;
        public string EmployeeDesignation;
 
        public string DraftId { get; set; }
        public int? CreatedByEmployeeId { get; set; }

        [Required(ErrorMessage = "Please Select Leave Type.")]
        public int? LeaveTypeId { get; set; }
        [Required(ErrorMessage = "Please Select Leave Category.")]
        public int? LeaveCategoryId { get; set; }
        [Required(ErrorMessage = "Please Select Leave Sub-Category.")]
        public int? LeaveSubCategoryId { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Please enter Leave Reason")]
        public string LeaveNature { get; set; }
        [Required]
        public DateTime? StartDate { get; set; }
        [Required]
        public DateTime? EndDate { get; set; }
        public int? CompOffAgainstDateId { get; set; }
        [Required]
        [Display(Name ="Afternoon")]
        public bool HalfDayStart { get; set; }
        [Required]
        [Display(Name = "Morning")]
        public bool HalfDayEnd { get; set; }
        public string Type { get; set; }
        public string Employee { get; set; }
        public string LoggedInEmployeeId { get; set; }

        public IList<HolidayVm> Holidays;

        public string Email;

        public string teamlist { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (LeaveCategoryId == 1 && StartDate!= null && StartDate.HasValue && StartDate.Value.CompareTo(DateTime.Today)<0) {
                //planned
                yield return new ValidationResult("Planned leave must be in future");
            };
            if (LeaveTypeId == 8 && StartDate != null && StartDate.HasValue && StartDate.Value.CompareTo(DateTime.Today) < 0)
            {
                //planned
                yield return new ValidationResult("Mission leave must be applied for future dates only");

            };
            if (LeaveCategoryId == 2 && StartDate != null && StartDate.HasValue && StartDate.Value.CompareTo(DateTime.Today)> 0)
            {
                    //unplanned
                    yield return new ValidationResult("Unplanned leave should have start date in the past");
            }
            if (StartDate != null && EndDate != null && StartDate.HasValue &&  EndDate.HasValue && StartDate.Value.CompareTo(EndDate.Value) > 0)
            {
                //unplanned
                yield return new ValidationResult("End date must be later than start date");
            }
            if(LeaveTypeId == 2 && CompOffAgainstDateId == null)
            {
                yield return new ValidationResult("Select Comp Off Against Date to avail Comp Off Leave", 
                    new String[] { "CompOffAgainstDateId" });
            }
            /*
            if (LeaveSubCategoryId == 4 && (EndDate.Value - StartDate.Value).TotalDays > 2 )
            {
                //planned
                yield return new ValidationResult("Document Submission is mandatory for sick leave of more than 2 days");

            };
            */
        }
    }

    /*
    * View Model for Leave Draft List Row
    */
    public class LeaveDraftListRow
    {
        public LeaveDraft LeaveDraft { get; set; }
        public LeaveDraftViewModel ViewModel { get; set; }
    }

    /*
    * View Model for Leave Draft
    */
    public class LeaveDraftViewModel
    {
        public LeaveDraftViewModel()
        {
        }
        public int? EmployeeId { get; set; }
        public int? CreatedByEmployeeId { get; set; }
        public int? LeaveTypeId { get; set; }
        public int? LeaveCategoryId { get; set; }
        public int? LeaveSubCategoryId { get; set; }
        public string LeaveNature { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime? EndDate { get; set; }
        public int? CompOffAgainstDateId { get; set; }
        public bool? HalfDayStart { get; set; }
        public bool? HalfDayEnd { get; set; }
        public string Type { get; set; }
        public string DraftId { get; set; }
    }

    /*
   * View Model for Holiday
   */
    public class HolidayVm
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }

    /*
   * View Model for Leave Request Approve Reject
   */
    public class LeaveRequestApproveRejectViewModel : IRequest<bool>
    {
        public int RejectionReasonId { get; set; }
        public int LeaveRequestId { get; set; }
        public int ActionById { get; set; }
        [Display(Name = "Other Reason")]
        public string RejectionReasonOther { get; set; }
    }

    /*
   * View Model for Leave Request Cancel
   */
    public class LeaveRequestCancelViewModel : IRequest<bool>
    {
        public int LeaveRequestId { get; set; }
        public int ActionById { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancellationReason { get; set; }
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
   * View Model for Document Received
   */
    public class DocumentReceivedViewModel : IRequest<bool>
    {
        public int ActionById { get; set; }
        public int LeaveRequestId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
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

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }


    }

    /*
   * View Model for Leave Reminder
   */
    public class LeaveReminderViewModel : BaseReminderViewModel { }

    /*
   * View Model for Leave Final Reminder
   */
    public class LeaveFinalReminderViewModel : BaseReminderViewModel { }

    public class DropDownVO
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class LeaveAccountingPeriodDeactivateVm
    {
        public LeaveAccountingPeriod LeaveAccountingPeriod { get; set; }
        public IEnumerable<LeaveRequest> PendingLeaves { get; set; }
    }

    public class LeaveRequestVm
    {
        public virtual IList<LeaveRequest> LeaveRequestListItems { get; set; }
    }


}
