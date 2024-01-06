using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Services;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
  * Handler class for Payment Request Approve Reject.
  */
    public class PaymentRequestApproveRejectHandler : IRequestHandler<PaymentRequestApproveRejectViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IAuditLogServices _auditlog;

        public PaymentRequestApproveRejectHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator, IAuditLogServices auditlog)
        {
            _repository = repository;
            _mapper = mapper;
            _paymentRequestService = paymentRequestService;
            _dbContext = dbContext;
            _mediator = mediator;
            _auditlog = auditlog;
        }

        private async Task ApproveReject(PaymentRequestApproveRejectViewModel paymentRequestVm, int id)
        {
            var paymentRequest = _dbContext.PaymentRequests.Single(p => p.Id == id);
            var paymentRequestType = paymentRequest.Type;
            var actionByEmployee = await _repository.GetByIdAsync<Employee>(paymentRequestVm.ActionById);

            if (paymentRequest.PaymentRequestActionedById != actionByEmployee.Id){
                //try
                //{
                //    var al = new Auditlog_DM();
                //    al.module = "PaymentRequestApproveRejectHandler";
                //    al.url = "ApproveReject - PaymentRequestActionedById != actionByEmployee.Id";
                //    al.comment = "ApproveReject End";
                //    al.userid = paymentRequest.Employee.EmployeeCode;
                //    al.line = paymentRequest.Id.ToString();
                //    al.path = "";
                //    al.exception = "";
                //    al.reportingto = paymentRequest.PaymentRequestActionedById.ToString();
                //    al.details = "";
                //    al.status = "";
                //    _auditlog.InsertLog(al);
                //}
                //catch (Exception ex) { }

                return;
            }else if(!(paymentRequest.Status == PaymentRequestStatus.PENDING.ToString()|| 
                        paymentRequest.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()))
            {
                //try
                //{
                //    var al = new Auditlog_DM();
                //    al.module = "PaymentRequestApproveRejectHandler";
                //    al.url = "ApproveReject - PENDING | APPROVED_ESCALATED";
                //    al.comment = "ApproveReject End";
                //    al.userid = paymentRequest.Employee.EmployeeCode;
                //    al.line = paymentRequest.Id.ToString();
                //    al.path = "";
                //    al.exception = "";
                //    al.reportingto = paymentRequest.PaymentRequestActionedById.ToString();
                //    al.details = "";
                //    al.status = "";
                //    _auditlog.InsertLog(al);
                //}
                //catch (Exception ex) { }

                return;
            }

            PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
            {
                PaymentRequestId = id,
                ActionById = paymentRequestVm.ActionById,
                Timestamp = DateTime.Now,
                Type = paymentRequestType
            };

            if (paymentRequestVm.RejectionReasonId == 0)
            {
                //Approval Case
                double limit = _paymentRequestService.GetApprovalLimit(paymentRequestVm.ActionById);
                if (paymentRequest.Amount > limit)
                {
                    paymentRequest.Status = PaymentRequestStatus.APPROVED_ESCALATED.ToString();
                    paymentRequest.PaymentRequestActionedById = (await GetNextApprover(paymentRequest, actionByEmployee)).Id;

                  //  paymentRequest.PaymentRequestActionedById = paymentRequestVm.ActionById;
                    paymentRequest.ActionDate = null;
                    approvalActions.Action = PaymentRequestActions.APPROVED.ToString();
                }
                else
                {
                    paymentRequest.Status = PaymentRequestStatus.APPROVED.ToString();
                    paymentRequest.PaymentRequestActionedById = paymentRequestVm.ActionById;
                    paymentRequest.ActionDate = DateTime.Now;
                    approvalActions.Action = PaymentRequestActions.APPROVED.ToString();
                }
            }
            else
            {
                //Rejection Case
                paymentRequest.RejectionReasonsId = paymentRequestVm.RejectionReasonId;
                paymentRequest.Status = PaymentRequestStatus.REJECTED.ToString();
                paymentRequest.RejectionReasonOther = paymentRequestVm.RejectionReasonOther;
                paymentRequest.PaymentRequestActionedById = paymentRequestVm.ActionById;
                paymentRequest.ActionDate = DateTime.Now;
                approvalActions.Action = PaymentRequestActions.REJECTED.ToString();
                approvalActions.Note = paymentRequestVm.RejectionReasonOther;
            }
           
            _dbContext.Entry(paymentRequest).State = EntityState.Modified;
            _dbContext.PaymentRequestApprovalActions.Add(approvalActions);
            _dbContext.SaveChanges();
            
            var notification = new NotificationEventModel
            {
                Type = GetNotificationType(paymentRequest),
                Event = GetNotificationEvent(paymentRequest),
                ModelType = typeof(PaymentRequest),
                ObjectId = paymentRequest.Id,
                ActionById = paymentRequestVm.ActionById,
                NextActionById = paymentRequest.PaymentRequestActionedById
             };

            //try
            //{
            //    var al = new Auditlog_DM();
            //    al.module = "PaymentRequestApproveRejectHandler";
            //    al.url = "ApproveReject";
            //    al.comment = "ApproveReject End";
            //    al.userid = paymentRequest.Employee.EmployeeCode;
            //    al.line = paymentRequest.Id.ToString();
            //    al.path = "";
            //    al.exception = "";
            //    al.reportingto = paymentRequest.PaymentRequestActionedById.ToString();
            //    al.details = "";
            //    al.status = "";
            //    _auditlog.InsertLog(al);
            //}
            //catch(Exception ex) { }


            await _mediator.Publish(notification);
        }

        private async Task<Employee> GetNextApprover(PaymentRequest paymentRequest, Employee actionByEmployee)
        {
            var bu = null as BusinessUnit;
            if (PaymentRequestType.ADVANCE.ToString().Equals(paymentRequest.Type))
            {
                //In advance, there is no need to check line items
                if(paymentRequest.BusinessActivity.IsVOC && paymentRequest.CustomerMarket.IsVOC)
                {
                    //Should go to VOC Head
                    bu = await _repository.GetOneAsync<BusinessUnit>(b => b.Name == "VOC");
                }
                else if (paymentRequest.BusinessActivity.BusinessUnit != null)
                {
                    //Should go to business head
                    bu = paymentRequest.BusinessActivity.BusinessUnit;
                }
            }
            else
            {
                bu = await GetExpenseBusinessUnit(paymentRequest);
            }

            if(actionByEmployee.AuthorizationProfile.Approval_Limit<50000  )
            {
                //var al = new Auditlog_DM();
                //al.module = "PaymentRequestApproveRejectHandler";
                //al.url = "GetNextApprover";
                //al.comment = "actionByEmployee.AuthorizationProfile.Approval_Limit<50000 ";
                //al.userid = paymentRequest.Employee.EmployeeCode;
                //al.line = paymentRequest.Id.ToString();
                //al.path = "";
                //al.exception = "";
                //al.reportingto = actionByEmployee.ReportingTo.ToString();
                //al.details = "";
                //al.status = "";
                //_auditlog.InsertLog(al);

                return actionByEmployee.ReportingTo;
            }
            else
            {
                if (paymentRequest.Employee.EmployeeCode == bu.BUHead.EmployeeCode)
                {
                    //var al = new Auditlog_DM();
                    //al.module = "PaymentRequestApproveRejectHandler";
                    //al.url = "GetNextApprover";
                    //al.comment = "paymentRequest.Employee.EmployeeCode == bu.BUHead.EmployeeCode ";
                    //al.userid = paymentRequest.Employee.EmployeeCode;
                    //al.line = paymentRequest.Id.ToString();
                    //al.path = "";
                    //al.exception = "";
                    //al.reportingto = actionByEmployee.ReportingTo.ToString();
                    //al.details = "";
                    //al.status = "";
                    //_auditlog.InsertLog(al);

                    return actionByEmployee.ReportingTo;
                }
                else
                {
                    //var al = new Auditlog_DM();
                    //al.module = "PaymentRequestApproveRejectHandler";
                    //al.url = "GetNextApprover";
                    //al.comment = "BUHead";
                    //al.userid = paymentRequest.Employee.EmployeeCode;
                    //al.line = paymentRequest.Id.ToString();
                    //al.path = "";
                    //al.exception = "";
                    //al.reportingto = actionByEmployee.ReportingTo.ToString();
                    //al.details = "";
                    //al.status = "";
                    //_auditlog.InsertLog(al);


                    return bu.BUHead;
                }
            }
        }

        public async Task<BusinessUnit> GetExpenseBusinessUnit(PaymentRequest paymentRequest)
        {
            var vocBu = await _repository.GetOneAsync<BusinessUnit>(bu => bu.Name == "VOC");
            Dictionary<BusinessUnit, double> expenses = new Dictionary<BusinessUnit, double>();
            var otherAmount = 0.0;
            foreach (var lineItem in paymentRequest.LineItems)
            {
                if(lineItem.BusinessActivity.IsVOC && lineItem.CustomerMarket.IsVOC)
                {
                    var amount = 0.0;
                    expenses.TryGetValue(vocBu, out amount);
                    if (expenses.ContainsKey(vocBu))
                    {
                        expenses[vocBu] += lineItem.Amount;
                    }
                    else
                    {
                        expenses[vocBu] = lineItem.Amount;
                    }
                }
                else if (lineItem.BusinessActivity.BusinessUnit != null)
                {
                    var amount = 0.0;
                    expenses.TryGetValue(lineItem.BusinessActivity.BusinessUnit, out amount);
                    if (expenses.ContainsKey(lineItem.BusinessActivity.BusinessUnit))
                    {
                        expenses[lineItem.BusinessActivity.BusinessUnit] += lineItem.Amount;
                    }
                    else
                    {
                        expenses[lineItem.BusinessActivity.BusinessUnit] = lineItem.Amount;
                    }
                }
                else
                {
                    otherAmount += lineItem.Amount;
                }

            }
            var highestBu = null as BusinessUnit;
            var highestAmount = 0.0;
            foreach(KeyValuePair<BusinessUnit, double> keyValue in expenses)
            {
                if (highestAmount < keyValue.Value)
                {
                    highestBu = keyValue.Key;
                    highestAmount = keyValue.Value;
                }
            }
            if (highestAmount>otherAmount)
            {
                return highestBu;
            }
            else
            {
                return null;
            }
        }




        public async Task<bool> Handle(PaymentRequestApproveRejectViewModel paymentRequestVm, CancellationToken cancellationToken)
        {
            if (paymentRequestVm.PaymentRequestId != 0)
            {
                await ApproveReject(paymentRequestVm, paymentRequestVm.PaymentRequestId);
            }
            if (paymentRequestVm.PaymentRequestsId != null)
            {
                foreach(int id in paymentRequestVm.PaymentRequestsId)
                {
                    await ApproveReject(paymentRequestVm, id);
                }
            }
            return true;
        }

        private string GetNotificationType(PaymentRequest paymentRequest)
        {
            if(string.Equals(PaymentRequestType.ADVANCE.ToString(), paymentRequest.Type)) {
                return EmailNotification.TYPE_ADVANCE;
            }
            else
            {
                return EmailNotification.TYPE_EXPENSE;
            }
        }

        private string GetNotificationEvent(PaymentRequest paymentRequest)
        {

            if (string.Equals(PaymentRequestStatus.APPROVED.ToString(), paymentRequest.Status))
            {
                if(string.Equals(PaymentRequestType.ADVANCE.ToString(), paymentRequest.Type))
                {
                    return EmailNotification.STATUS_ADVANCE_APPROVED;
                }
                else
                {
                    return EmailNotification.STATUS_EXPENSE_APPROVED;
                }

            }
            else if(string.Equals(PaymentRequestStatus.APPROVED_ESCALATED.ToString(), paymentRequest.Status))
            {
                if (string.Equals(PaymentRequestType.ADVANCE.ToString(), paymentRequest.Type))
                {
                    return EmailNotification.STATUS_ADVANCE_APPROVED_ESCALATED;
                }
                else
                {
                    return EmailNotification.STATUS_EXPENSE_APPROVED_ESCALATED;
                }
            }
            else if (string.Equals(PaymentRequestStatus.REJECTED.ToString(), paymentRequest.Status))
            {
                if (string.Equals(PaymentRequestType.ADVANCE.ToString(), paymentRequest.Type))
                {
                    return EmailNotification.STATUS_ADVANCE_REJECTED;
                }
                else
                {
                    return EmailNotification.STATUS_EXPENSE_REJECTED;
                }
            }
            return "";
        }

    }
}
