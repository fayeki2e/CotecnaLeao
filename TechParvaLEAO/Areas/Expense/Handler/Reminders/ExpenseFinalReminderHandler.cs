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
using TechParvaLEAO.Data;
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
 * Handler class for Expense Final Reminder.
 */
    public class ExpenseFinalReminderHandler : BaseNotificationHandler, IRequestHandler<ExpenseFinalReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public ExpenseFinalReminderHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestSequenceService paymentRequestSequenceService,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _paymentRequestService = paymentRequestService;
            _paymentRequestSequenceService = paymentRequestSequenceService;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<bool> Handle(ExpenseFinalReminderViewModel ExpenseReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var days_before = ExpenseReminderVm.ForDate - TimeSpan.FromDays(3);
            var reminderExpenses = _repository.
                            Get<PaymentRequest>(p => (p.PaymentRequestCreatedDate <= days_before && p.Status == PaymentRequestStatus.PENDING.ToString()));
            foreach (var paymentRequest in reminderExpenses)
            {

                await SendNotification(EmailNotification.TYPE_EXPENSE,
                    EmailNotification.STATUS_EXPENSE_REMINDER_FINAL,
                    typeof(PaymentRequest),
                    paymentRequest.Id);
            }
            //DO that same for escalated

            return true;
        }

    }
}
