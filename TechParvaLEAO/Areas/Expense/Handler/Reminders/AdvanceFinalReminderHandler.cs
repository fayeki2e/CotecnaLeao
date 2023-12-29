using AutoMapper;
using MediatR;
using System;
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
  * Handler class for Advance Final Reminder.
  */
    public class AdvanceFinalReminderHandler : BaseNotificationHandler, IRequestHandler<AdvanceFianlReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public AdvanceFinalReminderHandler(IApplicationRepository repository, IMapper mapper,
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

        public async Task<bool> Handle(AdvanceFianlReminderViewModel AdvanceFinalReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var days_before = AdvanceFinalReminderVm.ForDate - TimeSpan.FromDays(3);
            var reminderAdvances = _repository.
                            Get<PaymentRequest>(p => (p.PaymentRequestCreatedDate.Date <= days_before
                                                        && p.Status == PaymentRequestStatus.PENDING.ToString()
                                                        && p.Type== "ADVANCE"));
            foreach (var paymentRequest in reminderAdvances)
            {
                await SendNotification(EmailNotification.TYPE_ADVANCE,
                    EmailNotification.STATUS_ADVANCE_REMINDER_FINAL,
                    typeof(PaymentRequest),
                    paymentRequest.Id); ;
            }
            return true;
        }
    }
}
