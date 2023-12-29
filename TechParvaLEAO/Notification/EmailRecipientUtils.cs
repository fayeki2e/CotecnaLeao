using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Notification
{
    public class EmailRecipientUtils
    {
        public const string EMPLOYEE = "Employee";
        public const string FOR_ACTION_OF = "ForActionOf";
        public const string MANAGER = "Manager";
        public const string SUBMITTED_BY = "SubmittedBy";
        public const string HR = "HR";
        public const string FINANCE = "Finance";

        public static string GetRecipient(ApplicationDbContext dbContext, object obj, String recipientType)
        {
            switch (recipientType)
            {
                case HR:
                    return dbContext.ApplicationConfiguration.FirstOrDefault(c=> c.Key== "HR_EMAIL").Value;
                case FINANCE:
                    return dbContext.ApplicationConfiguration.FirstOrDefault(c => c.Key == "ACCOUNTS_EMAIL").Value;
            }

            if (obj is PaymentRequest)
            {
                return GetRecipient((PaymentRequest)obj, recipientType);
            }
            else if (obj is LeaveRequest)
            {
                return GetRecipient((LeaveRequest)obj, recipientType);
            }
            else if (obj is TimeSheet)
            {
                return GetRecipient((TimeSheet)obj, recipientType);
            }
            return null;
        }
        private static string GetRecipient(PaymentRequest paymentRequest, String recipientType)
        {
            switch (recipientType)
            {
                case EMPLOYEE:
                    return paymentRequest.PaymentRequestCreatedBy.Email;
                case SUBMITTED_BY:
                    return paymentRequest.PaymentRequestCreatedBy.Email;
                case FOR_ACTION_OF:
                    return paymentRequest.PaymentRequestActionedBy.Email;
                case MANAGER:
                    return paymentRequest.Employee.ReportingTo.Email;
            }
            return null;
        }

        private static string GetRecipient(LeaveRequest leaveRequest, String recipientType)
        {
            switch (recipientType)
            {
                case EMPLOYEE:
                    return leaveRequest.CreatedByEmployee.Email;
                case SUBMITTED_BY:
                    return leaveRequest.CreatedByEmployee.Email;
                case FOR_ACTION_OF:
                    return leaveRequest.Employee.ReportingTo.Email;
                case MANAGER:
                    return leaveRequest.Employee.ReportingTo.Email;
            }
            return null;
        }
        private static string GetRecipient(TimeSheet timesheet, String recipientType)
        {
            switch (recipientType)
            {
                case EMPLOYEE:
                    return timesheet.TimesheetCreatedBy.Email;
                case SUBMITTED_BY:
                    return timesheet.TimesheetCreatedBy.Email;
                case FOR_ACTION_OF:
                    return timesheet.Employee.ReportingTo.Email;
                case MANAGER:
                    return timesheet.Employee.ReportingTo.Email;
            }
            return null;
        }
    }
}
