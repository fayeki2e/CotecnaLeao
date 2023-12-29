using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.IO;
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
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Attendance.Services;

namespace TechParvaLEAO.Areas.Leave.Handler
{
    /*
    * Handler class to create new leave request.
    */
    public class NewLeaveRequestHandler : IRequestHandler<NewLeaveViewModel, Result>
    {
        private readonly IApplicationRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly LeaveRequestServices _leaveRequestService;
        private readonly PaymentRequestService _paymentRequestService;
        private readonly TimeSheetServices _timesheetServices;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public NewLeaveRequestHandler(IApplicationRepository repository, IMapper mapper,
            ApplicationDbContext dbContext,
            LeaveRequestServices leaveRequestService,
            PaymentRequestService paymentRequestService,
            TimeSheetServices timeSheetServices,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
            _leaveRequestService = leaveRequestService;
            _paymentRequestService = paymentRequestService;
            _timesheetServices = timeSheetServices;
            _mediator = mediator;
        }

        public async Task<Result> Handle(NewLeaveViewModel leaveRequestVm, CancellationToken cancellationToken)
        {
            var canApply = CanApplyForThisLeave(leaveRequestVm);
            var errList = new List<string>();
            if (!canApply)
            {
                errList.Add("You are not allowed to apply this leave");
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };

            }
            var employee = _repository.GetOne<Employee>(e => e.Id == leaveRequestVm.EmployeeId);
            var leaveRequest = _mapper.Map<NewLeaveViewModel, LeaveRequest>(leaveRequestVm);
            var leaveType = _repository.GetById<LeaveType>(leaveRequestVm.LeaveTypeId);
            if (leaveRequest.LeaveTypeId == 4)
            {
                leaveRequest.EndDate = leaveRequest.StartDate + TimeSpan.FromDays(180);
            }
            leaveRequest.CreatedByEmployeeId = leaveRequestVm.CreatedByEmployeeId;
            leaveRequest.Status = LeaveRequestStatus.PENDING.ToString();
            leaveRequest.LeaveRequestApprovedById = employee.ReportingToId.Value;

            var leave_year = leaveRequest.StartDate.Year;
            var curr_year = DateTime.Today.Year;

            if(leave_year == curr_year)
            {
                leaveRequest.LeaveEligibility = _leaveRequestService.GetAnnaulLeaveEligibility(employee);
                leaveRequest.LeavesAvailed = _leaveRequestService.GetAnnaulLeaveUtilized(employee);
                leaveRequest.LeavesCarriedForward = _leaveRequestService.GetAnnaulLeavesCarryForward(employee);
            }
            else
            {
                leaveRequest.LeaveEligibility = _leaveRequestService.GetAnnaulLeaveEligibility(employee, leaveRequest.StartDate);
                leaveRequest.LeavesAvailed = _leaveRequestService.GetAnnaulLeaveUtilized(employee, leaveRequest.StartDate);
                leaveRequest.LeavesCarriedForward = _leaveRequestService.GetAnnaulLeavesCarryForward(employee, leaveRequest.StartDate);
            }



    
            leaveRequest.LeavesOpeningBalance = leaveRequest.LeaveEligibility + leaveRequest.LeavesCarriedForward;

        
            leaveRequest.NumberOfDays = _leaveRequestService.GetNumberOfLeaveDays(
                    employee,
                    leaveRequest.StartDate,
                    leaveRequest.EndDate,
                    leaveRequestVm.HalfDayStart,
                    leaveRequestVm.HalfDayEnd,
                    leaveRequestVm.LeaveTypeId.HasValue? leaveRequestVm.LeaveTypeId.Value: 0
                    );
            if (leaveRequest.NumberOfDays <= 0)
            {
                errList.Add("You cannot apply for Leave on a Holiday.");
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }
            if (leaveType.Limit && leaveRequest.LeavesOpeningBalance < leaveRequest.LeavesAvailed + leaveRequest.NumberOfDays)
            {
                errList.Add("You do not have sufficient balance to apply for this leave");
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }
            // comp off must have start date and end date same
            if (leaveType.Id == 2 && ((leaveRequestVm.StartDate - leaveRequestVm.EndDate).Value.TotalSeconds != 0))
            {
                errList.Add("Comp off cannot be applied for more than 1 day at a time.");
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }
            // comp off must have start date and end date same
            if (leaveType.Id == 2 )
            {
                var compOff = _repository.GetById<LeaveCreditAndUtilization>(leaveRequest.CompOffAgainstDateId);
                if (compOff.CarryForward > 0 && compOff.CarryForward < leaveRequest.NumberOfDays)
                {
                    errList.Add("Comp off cannot be applied for more than 0.5 for this comp off.");
                    return new Result
                    {
                        Success = false,
                        ErrorMessage = errList
                    };
                }

            }
            var expenses = _paymentRequestService.FindExpenseByDate(employee, leaveRequest.StartDate, leaveRequest.EndDate);
            if (expenses.Count() > 0)
            {
                var payments = new System.Text.StringBuilder();
                foreach (var item in expenses)
                {
                    payments.Append(item.PaymentRequest.RequestNumber);
                    payments.Append(",");
                }
                errList.Add("You have an expense from the given leave dates. Expense claim number(s): " + payments.ToString());
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }

            var timesheets = _timesheetServices.GetTimeSheetsForEmployee(employee, leaveRequest.StartDate, leaveRequest.EndDate);
            if (timesheets.Count() > 0)
            {
                foreach (var record in timesheets)
                {
                    errList.Add("Timesheet entry is submitted for date: " + record.WorkDate.ToString("dd-MM-yyyy"));
                }
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }

            // Check if overlapping leave exists
            var existingLeaves = _repository.Get<LeaveRequest>(l => l.EmployeeId==leaveRequest.EmployeeId &&
                    (l.StartDate <= leaveRequest.EndDate && l.EndDate >= leaveRequest.StartDate) &&
                   (l.Status == LeaveRequestStatus.PENDING.ToString() || l.Status == LeaveRequestStatus.APPROVED.ToString())
                   );
            if (existingLeaves.Count() > 0){
                errList.Add("Another leave exists for your leave application date. You cannot apply for overlapping leaves.");
                return new Result
                {
                    Success = false,
                    ErrorMessage = errList
                };
            }



            leaveRequest.LeavesPending = leaveRequest.LeaveEligibility + leaveRequest.LeavesCarriedForward -leaveRequest.LeavesAvailed;
            leaveRequest.LWPDays = _leaveRequestService.GetLWPDays(employee);
            leaveRequest.LeaveAccountingPeriod = _leaveRequestService.GetAccountingPeriod(leaveRequest.StartDate);
            //In case of partial utilization, carry forward is updated.
            if (leaveRequest.CompOffAgainstDateId != null)
            {
                var compOff = _repository.GetById<LeaveCreditAndUtilization>(leaveRequest.CompOffAgainstDateId);

                compOff.CarryForward = 1 - compOff.CarryForward -
                        Convert.ToSingle(leaveRequest.NumberOfDays);
                if (compOff.CarryForward <= 0) { compOff.AddedUtilized = 0; };
            }


            LeaveRequestAction approvalActions = new LeaveRequestAction
            {
                ActionById = employee.ReportingToId.Value,
                LeaveRequest = leaveRequest,
                Timestamp = DateTime.Now,
                Action = LeaveRequestActions.SUBMITTED.ToString()
            };
            _dbContext.Entry<LeaveRequest>(leaveRequest).State=EntityState.Added;
            _dbContext.Entry<LeaveRequestAction>(approvalActions).State = EntityState.Added;

            if (leaveRequestVm.DraftId != null)
            {
                var draft = _dbContext.LeaveDrafts.
                    Where(p => p.UniqueId == leaveRequestVm.DraftId).FirstOrDefault();
                if (draft != null)
                {
                    _dbContext.LeaveDrafts.Remove(draft);
                }
            }

            long size = leaveRequestVm.Documents.Sum(f => f.Length);
            string filePathData = "";
            foreach (var formFile in leaveRequestVm.Documents)
            {
                if (formFile.Length > 0)
                {
                    string filePath = Path.Combine("LeaveUploads", leaveRequest.Id + "_" + formFile.FileName);
                    filePathData = filePathData + filePath;
                    filePathData = filePathData + "*";
                    // full path to file in temp location
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await formFile.CopyToAsync(stream);
                    }
                }
            }

            if (leaveRequestVm.Documents.Count > 0)
            {
                leaveRequest.documentsPath = filePathData;
            }
            _dbContext.SaveChanges();

            var notification = new NotificationEventModel
            {
                Type = EmailNotification.TYPE_LEAVE,
                Event = GetNotificationEvent(leaveRequest),
                ModelType = typeof(LeaveRequest),
                ObjectId = leaveRequest.Id
            };

            await _mediator.Publish<NotificationEventModel>(notification);

            return new Result { Success=true};
        }

        /*
         * Method for Can apply for this leave
         */
        private bool CanApplyForThisLeave(NewLeaveViewModel leaveRequestVm)
        {
            var leaveType = _repository.GetById<LeaveType>(leaveRequestVm.LeaveTypeId);
            if (leaveType.Id == 4)//Maternity Leave
            {
                var employee = _repository.GetById<Employee>(leaveRequestVm.EmployeeId);
                if (!employee.CanCreateMaternityLeaves()) return false;
            }
            return true;
        }

        /*
         * Method to return Notification Event
         */
        private string GetNotificationEvent(LeaveRequest leaveRequest)
        {
            LeaveType leaveType = _dbContext.LeaveTypes.Where(l => l.Id == leaveRequest.LeaveTypeId).FirstOrDefault();

            if (string.Equals(LeaveRequestStatus.PENDING.ToString(), leaveRequest.Status))
            {
                if (string.Equals("Leave Without Pay", leaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_SUBMITTED_LWP;
                }
                else if (string.Equals("Maternity Leave", leaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_SUBMITTED_MATERNITY;
                }
                else if (string.Equals("Mission", leaveType.Name))
                {
                    return EmailNotification.STATUS_LEAVE_SUBMITTED_MISSION;
                }
                else
                {
                    return EmailNotification.STATUS_LEAVE_SUBMITTED;
                }

            }
            return "";
        }
    }
}
