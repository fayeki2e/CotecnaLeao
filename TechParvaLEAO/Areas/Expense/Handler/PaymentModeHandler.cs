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
using TechParvaLEAO.Data;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
   * Handler class for Payment Mode.
   */
    public class PaymentModeHandler : IRequestHandler<PaymentModeViewModel, bool>
    {
        private readonly IApplicationRepository repository;
        private readonly PaymentRequestService paymentRequestService;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public PaymentModeHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.paymentRequestService = paymentRequestService;
            this.dbContext = dbContext;
            this.mediator = mediator;
        }


        public async Task<bool> Handle(PaymentModeViewModel paymentRequestVm, CancellationToken cancellationToken)
        {
            var paymentRequest = await dbContext.PaymentRequests.SingleAsync(p=> p.Id==paymentRequestVm.PaymentRequestId);
            var paymentRequestType = paymentRequest.Type;
            var actionByEmployee = await repository.GetByIdAsync<Employee>(paymentRequestVm.ActionById);

            PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
            {
                PaymentRequestId = paymentRequestVm.PaymentRequestId,
                ActionById = paymentRequestVm.ActionById,
                Timestamp = DateTime.Now,
                Type = paymentRequestType,
                Action = PaymentRequestActions.PAYMENT_MODE_UPDATE.ToString()
            };
            paymentRequest.SettlementMode= paymentRequestVm.PaymentMode;

            dbContext.Entry(paymentRequest).State = EntityState.Modified;
            dbContext.PaymentRequestApprovalActions.Add(approvalActions);
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
