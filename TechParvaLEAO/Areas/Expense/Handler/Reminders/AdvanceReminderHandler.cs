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


namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
 * Handler class for Advance Reminder.
 */
    public class AdvanceReminderHandler :BaseNotificationHandler, IRequestHandler<AdvanceReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public AdvanceReminderHandler(IApplicationRepository repository, IMapper mapper,
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

        public async Task<bool> Handle(AdvanceReminderViewModel AdvanceReminderVm , CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var first_days_before = AdvanceReminderVm.ForDate - TimeSpan.FromDays(1);
            var two_days_before = AdvanceReminderVm.ForDate - TimeSpan.FromDays(2);
            var reminderAdvances = _repository.
                            Get<PaymentRequest>(p => (p.PaymentRequestCreatedDate.Date == first_days_before.Date ||
                                        p.PaymentRequestCreatedDate.Date == two_days_before.Date)
                                        && p.Status==PaymentRequestStatus.PENDING.ToString()
                                        && p.Type == PaymentRequestType.ADVANCE.ToString());
            Dictionary<Employee, List<PaymentRequest>> reminders = new Dictionary<Employee, List<PaymentRequest>>();
            foreach (var paymentRequest in reminderAdvances)
            {
                var paymentLists = reminders.GetOrCreate(paymentRequest.PaymentRequestActionedBy);
                paymentLists.Add(paymentRequest);
            }

            foreach(KeyValuePair<Employee, List<PaymentRequest>> item in reminders)
            {
                await SendNotification(EmailNotification.TYPE_ADVANCE,
                    EmailNotification.STATUS_ADVANCE_REMINDER,
                    typeof(PaymentRequest),
                    item.Value.Select((p)=>p.Id).ToList());
            }
            return true;
        }
    }
}
