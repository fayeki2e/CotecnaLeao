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
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Services;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
 * Handler class for Expense Reminder.
 */
    public class ExpenseReminderHandler : BaseNotificationHandler, IRequestHandler<ExpenseReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public ExpenseReminderHandler(IApplicationRepository repository, IMapper mapper,
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

        public async Task<bool> Handle(ExpenseReminderViewModel ExpenseReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            //var first_day_before = ExpenseReminderVm.ForDate - TimeSpan.FromDays(1);
            //var second_day_before = ExpenseReminderVm.ForDate - TimeSpan.FromDays(2);
            /*
            var reminderExpenses = _repository.
                            Get<PaymentRequest>(p => (p.PaymentRequestCreatedDate.Date == first_day_before.Date ||
                                                        p.PaymentRequestCreatedDate.Date == second_day_before.Date)
                                                        && p.Status == PaymentRequestStatus.PENDING.ToString()
                                                        && p.Type==PaymentRequestType.REIMBURSEMENT.ToString());
                                                        */
            var reminderExpenses = _repository.
                            Get<PaymentRequest>(p => p.Status == PaymentRequestStatus.PENDING.ToString()||
                                                    p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()
                                                        && p.Type == PaymentRequestType.REIMBURSEMENT.ToString()
                                                        && p.Comment != "Advance entry from upload file to settle expenses");
            //Addsimilar condition for all advance and expense type handlers
            Dictionary<int, List<PaymentRequest>> reminders = new Dictionary<int, List<PaymentRequest>>();
            foreach (var paymentRequest in reminderExpenses)
            {
                if (paymentRequest.PaymentRequestActionedById != null)
                {
                    var paymentLists = reminders.GetOrCreate(paymentRequest.PaymentRequestActionedBy.Id);
                    paymentLists.Add(paymentRequest);
                }
            }

            foreach (KeyValuePair<int, List<PaymentRequest>> item in reminders)
            {
                try
                {

                    await SendNotification(EmailNotification.TYPE_EXPENSE,
                        EmailNotification.STATUS_EXPENSE_REMINDER,
                        typeof(PaymentRequest),
                        item.Value.Select((p) => p.Id).ToList());
                }
                catch (Exception ex)
                {

                    LogWriter.WriteLog(" ExpenseReminderHandler - Handle(ExpenseReminderViewModel", ex.Message);
                }
            }
            return true;
        }
    }
}
