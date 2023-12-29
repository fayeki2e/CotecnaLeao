using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
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
 * Handler class for Finance Expense Reminder.
 */
    public class FinanceExpenseReminderHandler : BaseNotificationHandler, IRequestHandler<FinanceExpenseReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public FinanceExpenseReminderHandler(IApplicationRepository repository, IMapper mapper,
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


        public async Task<bool> Handle(FinanceExpenseReminderViewModel ExpenseFinalReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var reminderExpenses = _repository.
                            Get<PaymentRequest>(p => (p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                                p.Status == PaymentRequestStatus.APPROVED.ToString()));
            IList<int> Ids = new List<int>();
            foreach(var expense in reminderExpenses)
            {
                Ids.Add(expense.Id);
            }
            if (Ids.Count > 0)
            {
                await SendNotification(EmailNotification.TYPE_EXPENSE,
                    EmailNotification.STATUS_EXPENSE_REMINDER_FINANCE,
                    typeof(PaymentRequest),
                    Ids);
            }
            return true;
        }
    }
}
