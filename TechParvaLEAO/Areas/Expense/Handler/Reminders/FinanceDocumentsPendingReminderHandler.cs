﻿using AutoMapper;
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
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
 * Handler class for Finance Documents Pending Reminder.
 */
    public class FinanceDocumentsPendingReminderHandler : BaseNotificationHandler, IRequestHandler<DocumentSubmissionReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly PaymentRequestSequenceService _paymentRequestSequenceService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;

        public FinanceDocumentsPendingReminderHandler(IApplicationRepository repository, IMapper mapper,
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

        public async Task<bool> Handle(DocumentSubmissionReminderViewModel ExpenseFinalReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var reminderExpenses = _repository.
                            Get<PaymentRequest>(p => (p.Type == PaymentRequestType.REIMBURSEMENT.ToString() &&
                                p.Status == PaymentRequestStatus.PAID.ToString())&&
                                p.SupportingDocumentsReceivedDate == null);
            Dictionary<Employee, List<PaymentRequest>> reminders = new Dictionary<Employee, List<PaymentRequest>>();
            foreach (var paymentRequest in reminderExpenses)
            {
                var paymentLists = reminders.GetOrCreate(paymentRequest.Employee);
                paymentLists.Add(paymentRequest);
            }

            foreach (KeyValuePair<Employee, List<PaymentRequest>> item in reminders)
            {
                await SendNotification(EmailNotification.TYPE_EXPENSE,
                EmailNotification.STATUS_EXPENSE_REMINDER_DOCUMENT_SUBMISSION,
                typeof(PaymentRequest),
                                 item.Value.Select((p) => p.Id).ToList());
            }
            return true;
        }
    }
}
