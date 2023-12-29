using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;

namespace TechParvaLEAO.Notification
{
    public class AutoCancelLeaves
    {
        private readonly ApplicationDbContext _context;
        private readonly IApplicationRepository _repository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediatR;
        private readonly LeaveRequestServices _leaveRequestServices;

        public AutoCancelLeaves(ApplicationDbContext context, IMapper mapper,
            LeaveRequestServices leaveRequestServices,
            IApplicationRepository repository,
            UserManager<ApplicationUser> userManager,
            IMediator mediatR)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
            this._mediatR = mediatR;
            this._repository = repository;
            this._leaveRequestServices = leaveRequestServices;
        }

        public async Task ExecuteAutoCancelLeaves(String operation)
        {
            IEnumerable<LeaveRequest> leaveRequests = await _repository.GetAsync<LeaveRequest>(
                l=>l.Status == "PENDING" && l.LeaveRequestCreatedDate < DateTime.Today);
            foreach(var leaveRequest in leaveRequests)
            {
                var workingDays = _leaveRequestServices.GetBusinessDays(leaveRequest.Employee, 
                    leaveRequest.LeaveRequestCreatedDate, DateTime.Today, false, false);
                if (workingDays > 4) //Working days considers day of application as one day, which is not the case
                {
                    Console.WriteLine("Leave Request", leaveRequest);
                    LeaveRequestCancelViewModel vm = new LeaveRequestCancelViewModel();
                    vm.CancellationReason = "Other/ Auto Cancelled";
                    vm.LeaveRequestId = leaveRequest.Id;
                    vm.ActionById = leaveRequest.LeaveRequestApprovedById.Value;
                    leaveRequest.CancellationReason = "Other/ Auto Cancelled";
                    var result = await _mediatR.Send(vm);
                }
            }
        }
    }
}

