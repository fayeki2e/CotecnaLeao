using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Services;
using Microsoft.Extensions.Options;
using TechParvaLEAO.Notification;
using Hangfire;
using Postal;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Services;

namespace TechParvaLEAO.Notification
{
    public class HangfireEmailSender
    {
        private readonly IApplicationRepository repository;
        private readonly PaymentRequestSequenceService paymentRequestSequenceService;
        private readonly PaymentRequestService paymentRequestService;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IEmailSenderEnhance emailSender;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly EmailSenderOptions emailSenderOptions;
        private readonly IEmployeeServices employeeService;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly IAuditLogServices _auditlog;

        public HangfireEmailSender(IApplicationRepository repository, IMapper mapper,
            PaymentRequestSequenceService paymentRequestSequenceService,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IEmailSenderEnhance emailSender,
            IEmployeeServices employeeService,
            UserManager<ApplicationUser> userManager,
            IOptions<EmailSenderOptions> options,
            LeaveRequestServices leaveRequestServices, IAuditLogServices auditlog)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.paymentRequestService = paymentRequestService;
            this.paymentRequestSequenceService = paymentRequestSequenceService;
            this.dbContext = dbContext;
            this.emailSender = emailSender;
            this.userManager = userManager;
            this.emailSenderOptions = options.Value;
            this.employeeService = employeeService;
            this.leaveRequestServices = leaveRequestServices;
            _auditlog = auditlog;
        }

        public async Task SendEmailNotification(NotificationEventModel notification)
        {
            if (notification == null) return;
            var obj = null as Object;
            var objPayments = null as List<PaymentRequest>;
            var objLeaves = null as List<LeaveRequest>;
            var objTimesheets = null as List<TimeSheet>;
            var objEmployee = null as List<Employee>;

            var recipientObj = null as object;

            if (notification.ObjectId != 0)
            {
                if (notification.ModelType == typeof(PaymentRequest))
                {
                    obj = await repository.GetByIdAsync<PaymentRequest>(notification.ObjectId);
                }
                else if (notification.ModelType == typeof(LeaveRequest))
                {
                    obj = await repository.GetByIdAsync<LeaveRequest>(notification.ObjectId);
                }
                else if (notification.ModelType == typeof(TimeSheet))
                {
                    obj = await repository.GetByIdAsync<TimeSheet>(notification.ObjectId);
                }
                else if (notification.ModelType == typeof(Employee))
                {
                    obj = await repository.GetByIdAsync<Employee>(notification.ObjectId);
                }


            }
            if (obj != null) recipientObj = obj;
            else if (notification.ObjectIds?.Count() > 0)
            {
                if (notification.ModelType == typeof(PaymentRequest))
                {
                    objPayments = dbContext.PaymentRequests
                        .Where(p => notification.ObjectIds.Contains(p.Id)).ToList();
                    recipientObj = objPayments.First();
                }
                else if (notification.ModelType == typeof(LeaveRequest))
                {
                    objLeaves = dbContext.LeaveRequests
                        .Where(p => notification.ObjectIds.Contains(p.Id)).ToList();
                    recipientObj = objLeaves.First();
                }
                else if (notification.ModelType == typeof(TimeSheet))
                {
                    objTimesheets = dbContext.TimeSheets
                        .Where(p => notification.ObjectIds.Contains(p.Id)).ToList();
                    recipientObj = objTimesheets.First();
                }
                else if (notification.ModelType == typeof(Employee))
                {
                     

                    objEmployee= dbContext.Employees.Where(e => e.Id == notification.EmployeeId).ToList();

                    recipientObj = objEmployee.First();
                }


            }

            var configuration = dbContext.EmailNotificationConfiguration.
                    Where(n => n.Type == notification.Type).
                    Where(n => n.Name == notification.Event).ToList();

            foreach (var emailConfig in configuration)
            {
                var subjectLine = emailConfig.SubjectLine;
                //var htmlTemplate = emailConfig.TemplatePathHtml;
                var htmlTemplateName = emailConfig.TemplatePathHtml;
                var recipient = EmailRecipientUtils.GetRecipient(dbContext, recipientObj, emailConfig.Receiver);
                if (string.IsNullOrEmpty(recipient)) continue;
                var paymentRequestVm = null as PaymentRequestEmailModel;
                var leaveRequestVm = null as LeaveRequestEmailModel;
                var timesheetVm = null as TimeSheetEmailModel;

                var paymentRequestVms = new List<PaymentRequestEmailModel>();
                var leaveRequestVms = new List<LeaveRequestEmailModel>();
                var timesheetVms = new List<TimeSheetEmailModel>();

                var acceptUrl = "" as string;
                var rejectUrl = "" as string;
                var detailUrl = "" as string;
                int employeeId = 0;
                var actionBy = null as Employee;
                var nextActionBy = null as Employee;

                if (notification.ActionById != null)
                {
                    actionBy = await repository.GetByIdAsync<Employee>(notification.ActionById);
                }
                if (notification.NextActionById != null)
                {
                    nextActionBy = await repository.GetByIdAsync<Employee>(notification.NextActionById);
                }

                if (typeof(PaymentRequest).IsInstanceOfType(obj))
                {
                    paymentRequestVm = CreatePaymentRequestViewModel((PaymentRequest)obj);
                    paymentRequestVm.ActionByEmployee = actionBy?.Name;
                    paymentRequestVm.NextActionByEmployee = nextActionBy?.Name;
                    employeeId = ((PaymentRequest)obj).PaymentRequestActionedById.Value;
                    detailUrl = await GenerateDetailUrl(notification, employeeId, null);
                }
                else if (typeof(LeaveRequest).IsInstanceOfType(obj))
                {
                    leaveRequestVm = CreateLeaveViewModel((LeaveRequest)obj);
                    employeeId = ((LeaveRequest)obj).LeaveRequestApprovedById.Value;
                    detailUrl = await GenerateDetailUrl(notification, employeeId, ((LeaveRequest)obj).Employee.EmployeeCode);
                }
                else if (typeof(TimeSheet).IsInstanceOfType(obj))
                {
                    timesheetVm = CreateTimesheetViewModel((TimeSheet)obj);
                    employeeId = ((TimeSheet)obj).TimesheetApprovedById.Value;
                }else if (objPayments!=null)
                {
                    foreach(var objl in objPayments)
                    {
                        paymentRequestVms.Add(CreatePaymentRequestViewModel(objl)); ; ;
                    }
                }else if (objLeaves!=null)
                {
                    foreach (var objl in objLeaves)
                    {
                        leaveRequestVms.Add(CreateLeaveViewModel(objl)); ; ;
                    }
                }else if (objTimesheets!=null)
                {
                    foreach (var objl in objTimesheets)
                    {
                        timesheetVms.Add(CreateTimesheetViewModel(objl)); ; ;
                    }
                }

                if (employeeId != 0)
                {
                    acceptUrl = await GenerateAcceptUrl(notification, employeeId);
                    rejectUrl = await GenerateRejectUrl(notification, employeeId);
                }

                var approver = null as Employee;
                if (notification.EmployeeId!=null)
                {
                    //approver = repository.GetById<Employee>(notification.EmployeeId);
                    approver = dbContext.Employees.Where(e => e.Id == notification.EmployeeId).FirstOrDefault();
                }
     
                var email = new NotificationEmailViewModel
                {
                    ViewName = htmlTemplateName,
                    To = recipient,
                    Subject = subjectLine,

                    PaymentRequestData = paymentRequestVm,
                    LeaveRequestData = leaveRequestVm,
                    TimesheetData = timesheetVm,

                    PaymentRequestDataList = paymentRequestVms,
                    LeaveRequestDataList = leaveRequestVms,
                    TimesheetDataList = timesheetVms,

                    AcceptButtonUrl = acceptUrl,
                    RejectButtonUrl = rejectUrl,
                    DetailButtonUrl = detailUrl,
                    FromDate = notification.FromDate,
                    ToDate = notification.ToDate,
                    Receiver = approver
                };
                try
                {
                    //try
                    //{
                    //    var al = new Auditlog_DM();
                    //    al.module = "HanfireEmailsender.cs";
                    //    al.url = "";
                    //    al.comment = "htmlTemplateName:" + htmlTemplateName + "PaymentRequestData:" + email.PaymentRequestData;
                    //    al.userid = "";
                    //    al.line = "Before Sending";
                    //    al.path = "";
                    //    al.exception = "";
                    //    al.reportingto = "";
                    //    al.details = "Email to:" + email.To.ToString();
                    //    al.status = "";
                    //    _auditlog.InsertLog(al);
                    //}catch(Exception ex)
                    //{

                    //}

                    if (email.To != "" || email.To != null)
                    {
                        await emailSender.SendEmailAsync(email);
                    }

                    //try
                    //{
                    //    var al = new Auditlog_DM();
                    //    al.module = "HanfireEmailsender.cs";
                    //    al.url = "";
                    //    al.comment = "htmlTemplateName:" + htmlTemplateName + "PaymentRequestData:" + email.PaymentRequestData;
                    //    al.userid = "";
                    //    al.line = "After Sending";
                    //    al.path = "";
                    //    al.exception = "";
                    //    al.reportingto = "";
                    //    al.details = "Email to:" + email.To.ToString();
                    //    al.status = "";
                    //    _auditlog.InsertLog(al);
                    //}
                    //catch (Exception ex)
                    //{

                    //}
                }
                catch (Exception ex)
                {
                    //var al = new Auditlog_DM();
                    //al.module = "HanfireEmailsender.cs";
                    //al.url = "";
                    //al.comment = "htmlTemplateName:" + htmlTemplateName + "PaymentRequestData:" + email.PaymentRequestData;
                    //al.userid = "";
                    //al.line = "";
                    //al.path = "";
                    //al.exception = ex.Message;
                    //al.reportingto = "";
                    //al.details = "Email to:"+ email.To.ToString();
                    //al.status = "";
                    //_auditlog.InsertLog(al);

                    //LogWriter.WriteLog(" HangfireEmailSender - SendEmailNotification", ex.Message);
                }
            }
        }

        private async Task<string> GenerateAcceptUrl(NotificationEventModel notification, int employeeId)
        {

            var user = dbContext.Users.FirstOrDefault(u => u.EmployeeProfileId == employeeId);
            var token = await userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
            return
                string.Format(
                    "{0}/Home/LoginCallback?token={1}&userId={2}&type={3}&targetaction={4}&objectId={5}&area={6}",
                    emailSenderOptions.ButtonURL,
                    token, user.Id, GetControllerName(notification.Type), "Approve", notification.ObjectId, GetArea(notification.Type));
            //return _urlHelper.Action("LoginCallback", "Home", new { token = token, userid = user.Id, controller="PaymentRequests", action="Accept", id=5 });            
        }

        private async Task<string> GenerateRejectUrl(NotificationEventModel notification, int employeeId)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.EmployeeProfileId == employeeId);
            var token = await userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
            return
                string.Format(
                    "{0}/Home/LoginCallback?token={1}&userId={2}&type={3}&targetaction={4}&objectId={5}&area={6}",
                    emailSenderOptions.ButtonURL,
                    token, user.Id, GetControllerName(notification.Type), "Reject", notification.ObjectId, GetArea(notification.Type));
            //return _urlHelper.Action("LoginCallback", "Account", new { token = token, userid = user.Id, controller = "PaymentRequests", action = "Reject", id = 5 });
        }

        private async Task<string> GenerateDetailUrl(NotificationEventModel notification, int employeeId, string employeeCode)
        {
            var user = dbContext.Users.FirstOrDefault(u => u.EmployeeProfileId == employeeId);
            var token = await userManager.GenerateUserTokenAsync(user, "Default", "passwordless-auth");
            var detailCode = "ExpenseDetails";
            if ("Advance".Equals(notification.Type)){
                detailCode = "AdvanceDetails";
            }
            else if ("Leave".Equals(notification.Type))
            {
                detailCode = "LeavesPendingApprovalList";
            }
            return
                string.Format(
                    "{0}/Home/LoginCallback?token={1}&userId={2}&type={3}&targetaction={4}&objectId={5}&area={6}&employeeCode={7}",
                    emailSenderOptions.ButtonURL,
                    token, user.Id, GetControllerName(notification.Type), detailCode, notification.ObjectId, GetArea(notification.Type), employeeCode);
            //return _urlHelper.Action("LoginCallback", "Home", new { token = token, userid = user.Id, controller="PaymentRequests", action="Accept", id=5 });            
        }

        private string GetControllerName(string type)
        {
            if (string.Equals(EmailNotification.TYPE_ADVANCE, type) || string.Equals(EmailNotification.TYPE_EXPENSE, type))
            {
                return "PaymentRequests";
            }
            else if (string.Equals(EmailNotification.TYPE_LEAVE, type))
            {
                return "LeaveRequests";
            }
            return "";
        }

        private string GetArea(string type)
        {
            if (string.Equals(EmailNotification.TYPE_ADVANCE, type) || string.Equals(EmailNotification.TYPE_EXPENSE, type))
            {
                return "Expense";
            }
            else if (string.Equals(EmailNotification.TYPE_LEAVE, type))
            {
                return "Leave";
            }
            return "";

        }


        private PaymentRequestEmailModel CreatePaymentRequestViewModel(PaymentRequest paymentRequest)
        {
            PaymentRequestEmailModel paymentRequestEmailModel = new PaymentRequestEmailModel();
            paymentRequestEmailModel.FromDate = paymentRequest.FromDate.HasValue ? paymentRequest.FromDate.Value : DateTime.Today;
            paymentRequestEmailModel.ToDate = paymentRequest.ToDate.HasValue ? paymentRequest.ToDate.Value : DateTime.Today;
            paymentRequestEmailModel.Id = paymentRequest.Id;
            paymentRequestEmailModel.Employee = paymentRequest.Employee.Name;
            paymentRequestEmailModel.Currency = paymentRequest.Currency?.Name;
            paymentRequestEmailModel.RequestNumber = paymentRequest.RequestNumber;
            paymentRequestEmailModel.Amount = paymentRequest.Amount;
            paymentRequestEmailModel.Comment = paymentRequest.Comment;
            paymentRequestEmailModel.CreatedDate = paymentRequest.PaymentRequestCreatedDate;
            paymentRequestEmailModel.PostedOn = paymentRequest.PostedOn.HasValue ? paymentRequest.PostedOn.Value : DateTime.Today;
            paymentRequestEmailModel.RejectionReason = string.Concat(paymentRequest.RejectionReasons?.Reason, paymentRequest.RejectionReasonOther == null ? "" : "-" + paymentRequest.RejectionReasonOther);
            paymentRequestEmailModel.ReportingManager = paymentRequest.Employee.ReportingTo?.Name;
            paymentRequestEmailModel.EmployeeCode = paymentRequest.Employee.EmployeeCode;
            paymentRequestEmailModel.UnsettledAmount = paymentRequestService.GetUnsettledAdvanceAmount(paymentRequest.Employee, paymentRequest.Currency?.Id);
            paymentRequestEmailModel.UnsettledCurrency = paymentRequest.Currency?.Name;
            paymentRequestEmailModel.ActionByEmployee = "";
            paymentRequestEmailModel.NextActionByEmployee = "";
            return paymentRequestEmailModel;
        }

        private LeaveRequestEmailModel CreateLeaveViewModel(LeaveRequest leaveRequest)
        {
            LeaveRequestEmailModel leaveRequestEmailModel = new LeaveRequestEmailModel();
            //dbContext.Entry(leaveRequest.Employee).Reload();
            //dbContext.Entry(leaveRequest).Reload();
            //var reportingTo = dbContext.Employees.Find(leaveRequest.Employee.ReportingToId);
            leaveRequestEmailModel.Id = leaveRequest.Id;
            leaveRequestEmailModel.CreatedDate = leaveRequest.LeaveRequestCreatedDate;
            leaveRequestEmailModel.Employee = leaveRequest.Employee.Name;
            leaveRequestEmailModel.EmployeeCode = leaveRequest.Employee.EmployeeCode;
            leaveRequestEmailModel.FromDate = leaveRequest.StartDate;
            leaveRequestEmailModel.ToDate = leaveRequest.EndDate;
            leaveRequestEmailModel.ReportingManager = leaveRequest.Employee.ReportingTo?.Name;
            //leaveRequestEmailModel.ReportingManager = reportingTo?.Name;
            leaveRequestEmailModel.CoordinatorName = leaveRequest.CreatedByEmployee.Name;
            leaveRequestEmailModel.LeaveEligibility = leaveRequest.LeaveEligibility;
            leaveRequestEmailModel.LeavesOpeningBalance = leaveRequest.LeavesOpeningBalance;
            leaveRequestEmailModel.LeaveCarriedForward = leaveRequest.LeavesCarriedForward;
            leaveRequestEmailModel.LeavesPending = leaveRequest.LeavesPending;
            if (leaveRequest.LeaveTypeId == 1)
            {
                leaveRequestEmailModel.LeavesBalance = leaveRequestEmailModel.LeavesOpeningBalance - (leaveRequest.LeavesAvailed + leaveRequest.NumberOfDays);
                leaveRequestEmailModel.LeavesAvailed = leaveRequest.LeavesAvailed + leaveRequest.NumberOfDays;
                leaveRequestEmailModel.LeavesUtilized = leaveRequest.LeavesAvailed + leaveRequest.NumberOfDays;
            }else
            {
                leaveRequestEmailModel.LeavesBalance = leaveRequestEmailModel.LeavesOpeningBalance - (leaveRequest.LeavesAvailed);
                leaveRequestEmailModel.LeavesAvailed = leaveRequest.LeavesAvailed;
                leaveRequestEmailModel.LeavesUtilized = leaveRequest.LeavesAvailed;
            }

            leaveRequestEmailModel.LWPDays = leaveRequestServices.GetLWPDays(leaveRequest.Employee);
            leaveRequestEmailModel.HR = "HR";//leaveRequest.Employee.ReportingTo.Name;
            leaveRequestEmailModel.RejectionReason = string.Concat(leaveRequest.RejectionReason?.Text, leaveRequest.RejectionReasonOther == null ? "" : "-" + leaveRequest.RejectionReasonOther);
            leaveRequestEmailModel.LeaveYear = leaveRequest.LeaveAccountingPeriod?.Name;
            leaveRequestEmailModel.LeaveNature = leaveRequest.LeaveNature;
            leaveRequestEmailModel.NumberOfDays = leaveRequest.NumberOfDays;
            leaveRequestEmailModel.CancelReason = leaveRequest.CancellationReason;
            leaveRequestEmailModel.HalfDayStart = leaveRequest.HalfDayStart;
            leaveRequestEmailModel.HalfDayEnd = leaveRequest.HalfDayEnd;
            leaveRequestEmailModel.LeaveType = leaveRequest.LeaveSubCategory.Text;
            leaveRequestEmailModel.DocumentPath = leaveRequest.documentsPath;
            return leaveRequestEmailModel;
        }

        private TimeSheetEmailModel CreateTimesheetViewModel(TimeSheet timeSheet)
        {
            TimeSheetEmailModel timeSheetEmailModel = new TimeSheetEmailModel();
            timeSheetEmailModel.ReportingManager = timeSheet.Employee.ReportingTo.Name;
            timeSheetEmailModel.FromDate = timeSheet.StartDate;
            timeSheetEmailModel.ToDate = timeSheet.EndDate;
            timeSheetEmailModel.Employee = timeSheet.Employee.Name;
            timeSheetEmailModel.Location = timeSheet.Employee.Location.Name;
            timeSheetEmailModel.Id = timeSheet.Id;
            timeSheetEmailModel.EmployeeCode = timeSheet.Employee.EmployeeCode;
            timeSheetEmailModel.CreatdeDate = timeSheet.TimesheetCreatedOn;
            timeSheetEmailModel.CoordinatorName = timeSheet.TimesheetCreatedBy.Name;
            //TODO HR EMail
            timeSheetEmailModel.HR = timeSheet.Employee.ReportingTo.Name;
            timeSheetEmailModel.NumberOfDays = timeSheet.NumberOfDays;
            return timeSheetEmailModel;
        }
    }
}
