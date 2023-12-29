using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Leave.Models;
using Techparva.GenericRepository;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
 * Model for Leave Request
 */
    public partial class LeaveRequest: Entity<int>
    {
        public LeaveRequest()
        { }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Display(Name = "Created By")]
        [ForeignKey("CreatedBy")]
        public virtual int? CreatedByEmployeeId { get; set; }
        public virtual Employee CreatedByEmployee { get; set; }

        [Required(ErrorMessage = "Please enter Leave Reason.")]
        [Display(Name = "Leave Reason")]
        public string LeaveNature { get; set; }

        [Display(Name = "Leave Type")]
        [ForeignKey("LeaveType")]
        public virtual int? LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType{ get; set; }

        [Display(Name = "Leave Category")]
        [ForeignKey("LeaveCategory")]
        public virtual int? LeaveCategoryId { get; set; }
        public virtual LeaveCategory LeaveCategory { get; set; }

        [Display(Name = "Leave Sub Category")]
        [ForeignKey("LeaveSubCategory")]
        public virtual int? LeaveSubCategoryId { get; set; }
        public virtual LeaveSubCategory LeaveSubCategory { get; set; }

        [Display(Name = "Leave Year")]
        [ForeignKey("LeaveAccountingPeriod")]
        public virtual int? LeaveAccountingPeriodId { get; set; }
        public virtual LeaveAccountingPeriod LeaveAccountingPeriod { get; set; }

        [Display(Name = "Leave Rejection Reason")]
        [ForeignKey("LeaveRejectionReason")]
        public virtual int? RejectionReasonId { get; set; }
        public virtual LeaveRejectionReason RejectionReason { get; set; }

        [Display(Name = "Comp Off Against Date")]
        [ForeignKey("CompOffAgainstDate")]
        public virtual int? CompOffAgainstDateId { get; set; }
        public virtual LeaveCreditAndUtilization CompOffAgainstDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Created Date")]
        public DateTime LeaveRequestCreatedDate { get; set; } = DateTime.Now;


        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Half Day Start")]
        public bool HalfDayStart { get; set; }

        [Display(Name = "Half Day End")]
        public bool HalfDayEnd { get; set; }

        public string Status { get; set; }

        //public string LeaveReason { get; set; }

        [Display(Name = "Number Of Days")]
        private double _NumberOfDays;
        public double NumberOfDays { get { return LeaveTypeId == 4 ? 180 : _NumberOfDays; }
            set { _NumberOfDays = value; } 
        }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Actual Start Date")]
        public DateTime? ActualStartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Actual End Date")]
        public DateTime? ActualEndDate { get; set; }

        [Display(Name = "Actual Half Day Start")]
        public bool? ActualHalfDayStart { get; set; }

        [Display(Name = "Actual Half Day End")]
        public bool? ActualHalfDayEnd { get; set; }

        [Display(Name = "Leave Request Approved By")]
        [ForeignKey("LeaveRequestApprovedBy")]
        public virtual int? LeaveRequestApprovedById { get; set; }
        public virtual Employee LeaveRequestApprovedBy { get; set; }

        public virtual ICollection<LeaveRequestAction> LeaveRequestActions { get; set; }

        public double LeaveEligibility { get; set; }
        public double LeavesCarriedForward { get; set; }
        public double LeavesOpeningBalance { get; set; }
        public double LeavesAvailed { get; set; }
        public double LeavesPending { get; set; }
        public double LWPDays { get; set; }

        public bool IsHalfDay
        {
            get{
                return this.HalfDayStart || this.HalfDayEnd;
            }
        }

        public bool CanCancelLeaves(bool isHr)
        {       //(string.Equals(Status, LeaveRequestStatus.PENDING.ToString()) || 
            if (isHr && string.Equals(Status, LeaveRequestStatus.APPROVED.ToString()))
            {
                return true;
            }else if (string.Equals(Status, LeaveRequestStatus.APPROVED.ToString()) &&(DateTime.Now < this.StartDate)) { 
                return true;
            }
            return false;
        }

        [Display(Name = "Documents")]
        public string documentsPath { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Documents Received Date")]
        public DateTime? DocumentsReceivedDate { get; set; }

        [Display(Name = "Documents Comment ")]
        public string DocumentsComment { get; set; }

        public List<string> GetDocuments()
        {
            string paths = documentsPath;
            if (!string.IsNullOrEmpty(paths))
            {
                return paths.Split("*").ToList();
            }
            return new List<string>();
        }

        [Display(Name = "Other Rejection Reason")]
        public string RejectionReasonOther { get; set; }

        [Display(Name = "Cancel Reason")]
        public string CancellationReason { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Last Action Date")]
        public DateTime? LastActionDate
        {
            get
            {
                if (this.LeaveRequestActions.Count() > 1)
                {
                    return this.LeaveRequestActions.Last().Timestamp;

                }
                return null;
            }
           
        }

        public double GetLeaveBalance()
        {
            if (this.LeaveTypeId == 1)
            {
                return this.LeaveEligibility - this.LeavesAvailed - this.NumberOfDays;
            }
            else
            {
                return 0;
            }
        }
    }

    /*
* Constant for Leave Request Status
*/
    public enum LeaveRequestStatus
    {
        PENDING,
        APPROVED,
        REJECTED,
        CANCELED
    }

    /*
* Constant for Leave Request Actions
*/
    public enum LeaveRequestActions
    {
        SAVED,
        SUBMITTED,
        REMINDER,
        FINAL_REMINDER,
        APPROVED,
        REJECTED,
        CANCELED
    }
}