using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Leave.Handler
{
    /*
    * Handler class to cancel leave request.
    */
    public class LeaveRequestCancelHandler : IRequestHandler<LeaveRequestCancelViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly LeaveRequestServices _leaveRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public LeaveRequestCancelHandler(IApplicationRepository repository, IMapper mapper,
            LeaveRequestServices leaveRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _leaveRequestService = leaveRequestService;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<bool> Handle(LeaveRequestCancelViewModel leaveRequestVm, CancellationToken cancellationToken)
        {
            var leaveRequest = _dbContext.LeaveRequests.Single(p => p.Id == leaveRequestVm.LeaveRequestId);
            var leaveRequestType = leaveRequest.LeaveType;
            var actionByEmployee = _repository.GetById<Employee>(leaveRequestVm.ActionById);
            var cancelReason = leaveRequestVm.CancellationReason;

            LeaveRequestAction action = new LeaveRequestAction
            {
                ActionById = actionByEmployee.Id,
                LeaveRequest = leaveRequest,
                Timestamp = DateTime.Now,
            };
         
            leaveRequest.Status = LeaveRequestStatus.CANCELED.ToString();
            leaveRequest.LeaveRequestApprovedById = actionByEmployee.Id;
            leaveRequest.CancellationReason = cancelReason;
            action.Action = LeaveRequestActions.CANCELED.ToString();
            //In case of partial utilization, carry forward is updated.
            if (leaveRequest.CompOffAgainstDate != null)
            {
                //Half day leave
                if (leaveRequest.NumberOfDays < 1 && leaveRequest.CompOffAgainstDate.CarryForward == 0)
                {
                    leaveRequest.CompOffAgainstDate.CarryForward = 0.5F;

                }
                else if (leaveRequest.NumberOfDays < 1 && leaveRequest.CompOffAgainstDate.CarryForward > 0)
                {
                    leaveRequest.CompOffAgainstDate.CarryForward = 0;
                }

                leaveRequest.CompOffAgainstDate.AddedUtilized = 1;
            }

            _dbContext.Entry(leaveRequest).State = EntityState.Modified;
            _dbContext.LeaveRequestActions.Add(action);
            _dbContext.SaveChanges();

            var notification = new NotificationEventModel
            {
                Type = GetNotificationType(leaveRequest),
                Event = GetNotificationEvent(leaveRequest),
                ModelType = typeof(LeaveRequest),
                ObjectId = leaveRequest.Id
            };
            await _mediator.Publish(notification);
            return true;


        }


        /*
    * Method to return notification type.
    */
        private string GetNotificationType(LeaveRequest leaveRequest)
        {
            return EmailNotification.TYPE_LEAVE;
        }


        /*
    * Method to return notification event.
    */
        private string GetNotificationEvent(LeaveRequest leaveRequest)
        {
            if (string.Equals(LeaveRequestStatus.CANCELED.ToString(), leaveRequest.Status))
            {          
                    return EmailNotification.STATUS_LEAVE_CANCELLED;             
            }
            return "";
        }
    }
}
