using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
   * Handler class for New Advance.
   */
    public class NewAdvanceHandler : IRequestHandler<AdvanceViewModel, bool>
    {
        private readonly IApplicationRepository repository;
        private readonly PaymentRequestSequenceService paymentRequestSequenceService;
        private readonly PaymentRequestService paymentRequestService;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IMediator mediator;
        public NewAdvanceHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestSequenceService paymentRequestSequenceService,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator
            )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.paymentRequestService = paymentRequestService;
            this.paymentRequestSequenceService = paymentRequestSequenceService;
            this.dbContext = dbContext;
            this.mediator = mediator;
        }


        public async Task<bool> Handle(AdvanceViewModel paymentRequestVm, CancellationToken cancellationToken)
        {
            var employee = await repository.GetOneAsync<Employee>(e => e.Id == Int32.Parse(paymentRequestVm.EmployeeId));
            var paymentRequest = mapper.Map<AdvanceViewModel, PaymentRequest>(paymentRequestVm);
            paymentRequest.PaymentRequestCreatedById = paymentRequestVm.CreatedByEmployeeId;
            paymentRequest.Type = PaymentRequestType.ADVANCE.ToString();
            paymentRequest.Status = PaymentRequestStatus.PENDING.ToString();
            paymentRequest.PaymentRequestActionedById = employee.ReportingToId;
            paymentRequest.LocationId = employee.LocationId.Value;
            paymentRequest.BalanceAmount = paymentRequestVm.Amount.Value;
            var financialYear = paymentRequestService.GetFinancialYear(DateTime.Today);
            paymentRequest.FinancialYearId = financialYear.Id;
            var requestNumber = paymentRequestSequenceService.GetNextSequence(
                PaymentRequestType.ADVANCE.ToString(),
                employee.Location.Code,
                employee.EmployeeCode,
                financialYear.Code
                );
            paymentRequest.RequestNumber = requestNumber;
            paymentRequest.VersionNumber = 1;
            PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
            {
                ActionById = employee.Id,
                PaymentRequest = paymentRequest,
                Timestamp = DateTime.Now,
                Action = PaymentRequestActions.SUBMITTED.ToString(),
                Type = PaymentRequestType.ADVANCE.ToString()
            };
            if (paymentRequestVm.DraftId != null)
            {
                var draft = dbContext.PaymentRequestDraft.
                    Where(p=> p.DraftId == paymentRequestVm.DraftId).FirstOrDefault();
                if (draft != null)
                {
                    dbContext.PaymentRequestDraft.Remove(draft);
                }
            }

            dbContext.Currencies.ToList();
            dbContext.PaymentRequests.Add(paymentRequest);
            dbContext.PaymentRequestApprovalActions.Add(approvalActions);
            dbContext.SaveChanges();

            var notification = new NotificationEventModel
            {
                Type = EmailNotification.TYPE_ADVANCE,
                Event = EmailNotification.STATUS_ADVANCE_SUBMITTED,
                ModelType = typeof(PaymentRequest),
                ObjectId = paymentRequest.Id
            };
            await mediator.Publish<NotificationEventModel>(notification);
            return true;
        }
    }
}
