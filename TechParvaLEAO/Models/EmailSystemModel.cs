using Postal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Models
{
    public class NotificationEmailViewModel:Postal.Email
    {
        public string To { get; set; }
        public string From { get; set; } = "uat@cotecna.test";
        public string Subject { get; set; } = "No Subject Specified";
        public PaymentRequestEmailModel PaymentRequestData { get; set; }
        public List<PaymentRequestEmailModel> PaymentRequestDataList { get; set; } = new List<PaymentRequestEmailModel>();
        public TimeSheetEmailModel TimesheetData { get; set; }
        public List<TimeSheetEmailModel> TimesheetDataList { get; set; } = new List<TimeSheetEmailModel>();
        public LeaveRequestEmailModel LeaveRequestData { get; set; }
        public List<LeaveRequestEmailModel> LeaveRequestDataList { get; set; } = new List<LeaveRequestEmailModel>();

        public List<Employee> EmployeeDataList { get; set; } = new List<Employee>();
        public List<NewLeaveViewModel> obj_leaverequest { get; set; } = new List<NewLeaveViewModel>();

        public List<LeaveRequestModal> obj_LeaveRequestModal { get; set; } = new List<LeaveRequestModal>();

        public string AcceptButtonUrl { get; set; }
        public string RejectButtonUrl { get; set; }
        public string DetailButtonUrl { get; set; }
        public Employee Receiver { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
 
    }
}
