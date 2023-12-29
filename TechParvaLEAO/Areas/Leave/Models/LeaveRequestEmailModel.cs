using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Leave.Models
{
    /*
 * Model for Leave Request Email Model
 */
    public class LeaveRequestEmailModel
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Employee { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ReportingManager { get; set; }
        public DateTime CreatdeDate { get; set; }
        public string CoordinatorName { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeavesAvailed { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeavesPending { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double NumberOfDays { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public string LeaveNature { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public string CategoryType { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]

        public double LeaveEligibility { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeaveCarriedForward { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeavesOpeningBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeavesUtilized { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LeavesBalance { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]
        public double LWPDays { get; set; }
        [DisplayFormat(DataFormatString = "{0:#.##}")]

        public bool IsHalfDay { get; set; }
        public bool HalfDayStart { get; set; }
        public bool HalfDayEnd { get; set; }
        public string HR { get; set; }
        public string RejectionReason { get; set; }
        public string CancelReason { get; set; }
        public string LeaveYear { get; set; }
        public string LeaveType { get; set; }
        public string DocumentPath { get; set; }
        public IEnumerable<LeaveRequestDetails> LeaveRequestDetail { get; set; }
    }

    /*
 * Model for Leave Request Details
 */
    public class LeaveRequestDetails
    {
        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Employee { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CreatdeDate { get; set; }

    }
}
