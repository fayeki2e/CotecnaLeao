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
using TechParvaLEAO.Notification;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;

namespace TechParvaLEAO.Areas.Leave.Handler
{
    /*
    * Handler class to approve and reject leave request.
    */
    public class LeaveRequestApproveRejectHandler : IRequestHandler<LeaveRequestApproveRejectViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly LeaveRequestServices _leaveRequestService;
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public LeaveRequestApproveRejectHandler(IApplicationRepository repository, IMapper mapper,
            LeaveRequestServices leaveRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator
            ){
            _repository = repository;
            _mapper = mapper;
            _leaveRequestService = leaveRequestService;
            _dbContext = dbContext;
            _mediator = mediator;
        }

        public async Task<bool> Handle(LeaveRequestApproveRejectViewModel leaveRequestVm, CancellationToken cancellationToken)
        {
            var leaveRequest = _dbContext.LeaveRequests.Single(p => p.Id == leaveRequestVm.LeaveRequestId);
            var leaveRequestType = leaveRequest.LeaveType;   
            var actionByEmployee = _repository.GetById<Employee>(leaveRequestVm.ActionById);

            LeaveRequestAction action = new LeaveRequestAction
            {
                ActionById = actionByEmployee.Id,
                LeaveRequest = leaveRequest,
                Timestamp = DateTime.Now,
            };

            if (leaveRequestVm.RejectionReasonId == 0)
            {
                //Approval Case
                leaveRequest.Status = LeaveRequestStatus.APPROVED.ToString();
                leaveRequest.LeaveRequestApprovedById= actionByEmployee.Id;
                action.Action = LeaveRequestActions.APPROVED.ToString();
            }
            else
            {
                leaveRequest.Status = LeaveRequestStatus.REJECTED.ToString();
                leaveRequest.LeaveRequestApprovedById = actionByEmployee.Id;
                leaveRequest.RejectionReasonId = leaveRequestVm.RejectionReasonId;
                action.Action = LeaveRequestActions.REJECTED.ToString();
                leaveRequest.RejectionReasonOther = leaveRequestVm.RejectionReasonOther;
                //In case of partial utilization, carry forward is updated.
                if (leaveRequest.CompOffAgainstDate != null)
                {
                    //Half day leave
                    if (leaveRequest.NumberOfDays < 1 && leaveRequest.CompOffAgainstDate.CarryForward == 0)
                    {
                        leaveRequest.CompOffAgainstDate.CarryForward = 0.5F;

                    }else if (leaveRequest.NumberOfDays < 1 && leaveRequest.CompOffAgainstDate.CarryForward > 0)
                    {
                        leaveRequest.CompOffAgainstDate.CarryForward = 0;
                    }

                    leaveRequest.CompOffAgainstDate.AddedUtilized = 1;
                }

            }
            _dbContext.Entry(leaveRequest).State = EntityState.Modified;
            _dbContext.LeaveRequestActions.Add(action);
            _dbContext.SaveChanges();

            var notification = new NotificationEventModel
            {
                ActionById = actionByEmployee.Id,
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

            if (string.Equals(LeaveRequestStatus.APPROVED.ToString(), leaveRequest.Status))
            {
                if (string.Equals("Leave Without Pay", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_APPROVED_LWP;
                }
                else if (string.Equals("Maternity Leave", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_APPROVED_MATERNITY;
                }
                else if (string.Equals("Mission", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_APPROVED_MISSION;
                }
                else
                {
                    return EmailNotification.STATUS_LEAVE_APPROVED;
                }

            }
            else if (string.Equals(LeaveRequestStatus.REJECTED.ToString(), leaveRequest.Status))
            {
                if (string.Equals("Leave Without Pay", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_REJECTED_LWP;
                }
                else if (string.Equals("Maternity Leave", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_REJECTED;
                }
                else if (string.Equals("Mission", leaveRequest.LeaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_REJECTED_MISSION;
                }
                else
                {
                    return EmailNotification.STATUS_LEAVE_REJECTED;
                }


            }
            return "";
        }
    }
}
