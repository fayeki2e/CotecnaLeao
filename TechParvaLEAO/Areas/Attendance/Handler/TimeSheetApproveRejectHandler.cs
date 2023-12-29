using AutoMapper;
using MediatR;
using System;
using System.Threading.Tasks;
using System.Threading;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Attendance.Handler
{
    /*
     * Handler class for TimeSheet Approve and Reject.
     */
    public class TimeSheetApproveRejectHandler : IRequestHandler<TimeSheetApproveRejectViewModel, bool>
    {
        private readonly IApplicationRepository repository;
        private readonly TimeSheetServices timeSheetServices;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly ApplicationDbContext context;
        private readonly IMediator mediator;
        //
        public TimeSheetApproveRejectHandler(IApplicationRepository repository, IMapper mapper,
            TimeSheetServices timeSheetServices,
            ApplicationDbContext dbContext,            
            ApplicationDbContext applicationDbContext,
            IMediator mediator,
            LeaveRequestServices leaveRequestServices
            )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.timeSheetServices = timeSheetServices;
            this.dbContext = dbContext;
            this.leaveRequestServices = leaveRequestServices;
            this.context = applicationDbContext;
            this.mediator = mediator;
        }

        public async Task<bool> Handle(TimeSheetApproveRejectViewModel timesheetApproveRejectVm, CancellationToken cancellationToken)
        {
            var timeSheet = repository.GetById<TimeSheet>(timesheetApproveRejectVm.Id);
            timeSheet.TimesheetApprovedById = timesheetApproveRejectVm.ApprovedById;
            if (string.Equals("Approve", timesheetApproveRejectVm.ApproveReject))
            {
                Approve(timeSheet);
            }
            else
            {
                Reject(timeSheet, timesheetApproveRejectVm.RejecttionReason);
            }
            var notification = new NotificationEventModel
            {
                Type = GetNotificationType(timeSheet),
                Event = GetNotificationEvent(timesheetApproveRejectVm),
                ModelType = typeof(TimeSheet),
                ObjectId = timeSheet.Id
            };
            await mediator.Publish(notification);
            return true;
        }

        private string GetNotificationEvent(TimeSheetApproveRejectViewModel timesheetApproveRejectVm)
        {
            if(string.Equals(timesheetApproveRejectVm.ApproveReject, "Approve"))
            {
                return EmailNotification.STATUS_TIMESHEET_APPROVED;
            }
            else if (string.Equals(timesheetApproveRejectVm.ApproveReject, "Reject"))
            {
                return EmailNotification.STATUS_TIMESHEET_REJECTED;
            }
            return "";
        }

        private void Approve(TimeSheet timeSheet)
        {
            var compOffs = timeSheet.TimesheetCompOffs;
            int compoffTypeId = 2;
            var leaveAccountingPeriod = leaveRequestServices.GetAccountingPeriod(DateTime.Today);
            timeSheet.Status = TimesheetStatus.APPROVED.ToString();
            timeSheet.ApprovedOn = DateTime.Now;
            foreach (var compOff in compOffs)
            {
                var compOffCredit = new LeaveCreditAndUtilization
                {
                    LeaveTypeId = compoffTypeId,
                    EmployeeId = timeSheet.EmployeeId.Value,
                    CreatedById = timeSheet.TimesheetCreatedById.Value,
                    NumberOfDays = 1,
                    AddedUtilized = 1,
                    AccrualDate = compOff.CompOffDate,
                    ApprovedDate = DateTime.Now,
                    ExpiryDate = DateTime.Now + TimeSpan.FromDays(30),
                    LeaveAccountingPeriodId = leaveAccountingPeriod.Id
                };
                context.Add(compOffCredit);
            }
            context.Entry(timeSheet).State = EntityState.Modified;
            context.SaveChanges();

        }
        private void Reject(TimeSheet timeSheet, int RejectionReason)
        {
            timeSheet.Status = TimesheetStatus.REJECTED.ToString();
            context.Entry(timeSheet).State = EntityState.Modified;
            context.SaveChanges();
        }

        private string GetNotificationType(TimeSheet timeSheet)
        {
            return EmailNotification.TYPE_TIMESHEET;
        }


    }
}
