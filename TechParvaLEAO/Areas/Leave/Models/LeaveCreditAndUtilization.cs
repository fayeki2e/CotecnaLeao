using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Leave.Models;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
  * Model for Leave Credit And Utilization
  */
    public class LeaveCreditAndUtilization: Entity<int>
    {
        public LeaveCreditAndUtilization()
        { }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Display(Name = "Created By")]
        [ForeignKey("CreatedBy")]
        public virtual int? CreatedById { get; set; }
        public virtual Employee CreatedByEmployee { get; set; }

        [Display(Name = "Type")]
        [ForeignKey("LeaveType")]
        public virtual int LeaveTypeId { get; set; }
        public virtual LeaveType LeaveType { get; set; }

        [Display(Name = "Number of Days")]
        public double NumberOfDays { get; set; }//leaves taken
            
        [Display(Name = "Added/ Utilizaed")]
        public int AddedUtilized { get; set; }

        [Display(Name = "Carry Forward Leaves")]
        public double CarryForward { get; set; }

        [Display(Name = "Annual Leaves")]
        public double AnnualLeaves { get; set; }                  

        [Display(Name = "Accrual Date")]
        public DateTime? AccrualDate { get; set; }

        [Display(Name = "Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [Display(Name = "Expiry Date")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Leave Accounting Period")]
        [ForeignKey("LeaveAccountingPeriod")]
        public virtual int? LeaveAccountingPeriodId { get; set; }
        public virtual LeaveAccountingPeriod LeaveAccountingPeriod { get; set; }
    }
}
