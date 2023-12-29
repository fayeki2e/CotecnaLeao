using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Leave.Handler
{
    /*
    * Handler class form Final reminder.
    */
    public class LeaveFinalReminderHandler : BaseNotificationHandler, IRequestHandler<LeaveFinalReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly LeaveRequestServices _leaveRequestService;
        private readonly IMapper _mapper;

        public LeaveFinalReminderHandler(IApplicationRepository repository, IMapper mapper,
            ApplicationDbContext dbContext,
            LeaveRequestServices leaveRequestService,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
            _leaveRequestService = leaveRequestService;
            _mediator = mediator;
        }


        public async Task<bool> Handle(LeaveFinalReminderViewModel LeaveReminderVm, CancellationToken cancellationToken)
        {
            //Get all payment requests which were created one or two days ago
            //And are not approved or rejected
            var third_day_before = LeaveReminderVm.ForDate - TimeSpan.FromDays(3);
            var reminderLeaves = _repository.
                            Get<LeaveRequest>(r => 
                            r.LeaveRequestCreatedDate.Date == third_day_before.Date &&
                            r.Status == LeaveRequestStatus.PENDING.ToString() &&
                            r.LeaveTypeId == 1);
            Dictionary<Employee, List<LeaveRequest>> reminders = new Dictionary<Employee, List<LeaveRequest>>();
            foreach (var leaveRequest in reminderLeaves)
            {
                var leaveRequests = reminders.GetOrCreate(leaveRequest.LeaveRequestApprovedBy);
                leaveRequests.Add(leaveRequest);
            }

            foreach (KeyValuePair<Employee, List<LeaveRequest>> item in reminders)
            {
                await SendNotification(EmailNotification.TYPE_LEAVE,
                    EmailNotification.STATUS_LEAVE_REMINDER_FINAL,
                    typeof(LeaveRequest),
                    item.Value.Select((p) => p.Id).ToList()); ;
            }
            return true;
        }
    }
}
