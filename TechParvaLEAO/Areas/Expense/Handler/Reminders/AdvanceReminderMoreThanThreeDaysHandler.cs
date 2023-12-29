using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
 * Handler class for Advance Reminder More Than Three Days.
 */
    public class AdvanceReminderMoreThanThreeDaysHandler : BaseNotificationHandler, IRequestHandler<AdvanceReminderMoreThanThreeDaysViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly LeaveRequestServices _leaveRequestServices;

        public AdvanceReminderMoreThanThreeDaysHandler(IApplicationRepository repository, IMapper mapper,
            PaymentRequestSequenceService paymentRequestSequenceService,
            PaymentRequestService paymentRequestService,
            ApplicationDbContext dbContext,
            LeaveRequestServices leaveRequestServices,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _paymentRequestService = paymentRequestService;
            _paymentRequestSequenceService = paymentRequestSequenceService;
            _dbContext = dbContext;
            _mediator = mediator;
            _leaveRequestServices = leaveRequestServices;
        }

        public async Task<bool> Handle(AdvanceReminderMoreThanThreeDaysViewModel AdvanceFinalReminderVm, 
            CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var days_before = AdvanceFinalReminderVm.ForDate - TimeSpan.FromDays(4);
            var reminderAdvances = await _repository.
                GetAsync<PaymentRequest>(p => p.Type == PaymentRequestType.ADVANCE.ToString() &&  
                                                p.PaymentRequestCreatedDate.Date <= days_before
                                            && (p.Status == PaymentRequestStatus.PENDING.ToString()
                                            || p.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString()));
            foreach (var paymentRequest in reminderAdvances)
            {
                var effectiveDate = null as DateTime?;
                if (paymentRequest.Status == PaymentRequestStatus.APPROVED_ESCALATED.ToString() && paymentRequest.ActionDate!=null)
                {
                    effectiveDate = paymentRequest.ActionDate;
                }
                else
                {
                    effectiveDate = paymentRequest.PaymentRequestCreatedDate;
                }
                var workingDays = _leaveRequestServices.GetBusinessDays(paymentRequest.Employee,
                    effectiveDate.Value, DateTime.Today, false, false);
                if (workingDays > 4) //Working days considers day of application as one day, which is not the case
                {
                    PaymentRequestApproveRejectViewModel vm = new PaymentRequestApproveRejectViewModel();
                    vm.PaymentRequestId = paymentRequest.Id;
                    vm.RejectionReasonId = 4;
                    vm.ActionById = paymentRequest.PaymentRequestActionedById.Value;
                    vm.RejectionReasonOther = "Auto Cancalled";
                    var result = await _mediator.Send(vm);
                }
            }
            return true;
        }
    }
}
