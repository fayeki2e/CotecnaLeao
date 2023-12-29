using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Notification
{
    public class EmailNotification
    { 
        public static readonly string TYPE_ADVANCE = "Advance";
        public static readonly string TYPE_EXPENSE = "Expense";
        public static readonly string TYPE_LEAVE = "Leave";
        public static readonly string TYPE_TIMESHEET = "Timesheet";

        private static readonly string _SUBMITTED = "Submitted";
        private static readonly string _REMINDER = "Reminder";
        private static readonly string _REMINDER_FINAL = "Reminder Final";
        private static readonly string _REJECTED = "Rejected";
        private static readonly string _APPROVED_ESCALATED = "Approved Escalated";
        private static readonly string _REMINDER_ADVANCE_FINANCE = "Finance Advance Reminder";
        private static readonly string _REMINDER_EXPENSE_FINANCE = "Finance Expense Reminder";
        private static readonly string _REMINDER_DOCUEMENT_SUBMISSION = "Expense Document Submission Reminder";
        private static readonly string _CANCELLED = "Cancelled";

        public static readonly string _APPROVED = "Approved";
        private static readonly string _PAID = "Paid";
        private static readonly string _MATERNITY = "_MATERNITY";
        private static readonly string _LWP = "_LWP";
        private static readonly string _LWP_MONTHLY = "_LWP_MONTHLY";
        private static readonly string _MISSION = "_MISSION";

        public static readonly string STATUS_ADVANCE_SUBMITTED = _SUBMITTED;
        public static readonly string STATUS_ADVANCE_REMINDER = _REMINDER;
        public static readonly string STATUS_ADVANCE_REMINDER_FINAL = _REMINDER_FINAL;
        public static readonly string STATUS_ADVANCE_REJECTED = _REJECTED;
        public static readonly string STATUS_ADVANCE_APPROVED_ESCALATED = _APPROVED_ESCALATED;
        public static readonly string STATUS_ADVANCE_APPROVED = _APPROVED;
        public static readonly string STATUS_ADVANCE_PAID = _PAID;
        public static readonly string STATUS_ADVANCE_REMINDER_FINANCE = _REMINDER_ADVANCE_FINANCE;

        public static readonly string STATUS_EXPENSE_SUBMITTED = _SUBMITTED;
        public static readonly string STATUS_EXPENSE_REMINDER = _REMINDER;
        public static readonly string STATUS_EXPENSE_REMINDER_FINAL = _REMINDER_FINAL;
        public static readonly string STATUS_EXPENSE_REJECTED = _REJECTED;
        public static readonly string STATUS_EXPENSE_APPROVED_ESCALATED = _APPROVED_ESCALATED;
        public static readonly string STATUS_EXPENSE_APPROVED = _APPROVED;
        public static readonly string STATUS_EXPENSE_PAID = _PAID;
        public static readonly string STATUS_EXPENSE_REMINDER_FINANCE = _REMINDER_EXPENSE_FINANCE;
        public static readonly string STATUS_EXPENSE_REMINDER_DOCUMENT_SUBMISSION = _REMINDER_DOCUEMENT_SUBMISSION;

        public static readonly string STATUS_LEAVE_SUBMITTED = _SUBMITTED;
        public static readonly string STATUS_LEAVE_SUBMITTED_MATERNITY = _SUBMITTED+_MATERNITY;
        public static readonly string STATUS_LEAVE_SUBMITTED_LWP = _SUBMITTED + _LWP;
        public static readonly string STATUS_LEAVE_SUBMITTED_LWP_MONTHLY = _SUBMITTED + _LWP_MONTHLY;
        public static readonly string STATUS_LEAVE_REMINDER = _REMINDER;
        public static readonly string STATUS_LEAVE_REMINDER_FINAL = _REMINDER_FINAL;
        public static readonly string STATUS_LEAVE_REJECTED = _REJECTED;
        public static readonly string STATUS_LEAVE_REJECTED_LWP = _REJECTED + _LWP;
        public static readonly string STATUS_LEAVE_APPROVED = _APPROVED;
        public static readonly string STATUS_LEAVE_APPROVED_LWP = _APPROVED + _LWP;
        public static readonly string STATUS_LEAVE_APPROVED_MATERNITY = _APPROVED + _MATERNITY;
        public static readonly string STATUS_LEAVE_APPROVED_LWP_MONTHLY = _APPROVED + _LWP_MONTHLY;
        public static readonly string STATUS_LEAVE_APPROVED_MISSION = _APPROVED + _MISSION;
        public static readonly string STATUS_LEAVE_REJECTED_MISSION = _REJECTED + _MISSION;
        public static readonly string STATUS_LEAVE_SUBMITTED_MISSION = _SUBMITTED + _MISSION;
        public static readonly string STATUS_LEAVE_CANCELLED = _CANCELLED;

        public static readonly string STATUS_TIMESHEET_SUBMITTED = _SUBMITTED;
        public static readonly string STATUS_TIMESHEET_REMINDER = _REMINDER;
        public static readonly string STATUS_TIMESHEET_REMINDER_FINAL = _REMINDER_FINAL;
        public static readonly string STATUS_TIMESHEET_REJECTED = _REJECTED;
        public static readonly string STATUS_TIMESHEET_APPROVED = _APPROVED;

        public static readonly string RECEIVER_EMPLOYEE = "Employee";
        public static readonly string RECEIVER_MANAGER = "Manager";
        public static readonly string RECEIVER_HIGHER_MANAGER = "Next Level Manager";
        public static readonly string RECEIVER_HR = _APPROVED;
        public static readonly string RECEIVER_FINANCE_MANAGER = "Finance Manager";
        public static readonly string RECEIVER_COORDINATOR = "Co-Ordinator";
    }
}
