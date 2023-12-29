using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
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
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    /*
    * Handler class for document reveived.
    */
    public class DocumentReceivedHandler : IRequestHandler<DocumentReceivedViewModel, bool>
    {
        private readonly IApplicationRepository repository;
        private readonly LeaveRequestServices leaveRequestService;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IMediator mediator;

        public DocumentReceivedHandler(IApplicationRepository repository, IMapper mapper,
            LeaveRequestServices leaveRequestService,
            ApplicationDbContext dbContext,
            IMediator mediator)
        {
            this.repository = repository;
            this.mapper = mapper;
            this.leaveRequestService = leaveRequestService;
            this.dbContext = dbContext;
            this.mediator = mediator;
        }


        public async Task<bool> Handle(DocumentReceivedViewModel leaveRequestVm, CancellationToken cancellationToken)
        {
            var leaveRequest = await dbContext.LeaveRequests.SingleAsync(p => p.Id == leaveRequestVm.LeaveRequestId);
            var actionByEmployee = await repository.GetByIdAsync<Employee>(leaveRequestVm.ActionById);
            leaveRequest.DocumentsReceivedDate = leaveRequestVm.Date;
            dbContext.Entry(leaveRequest).State = EntityState.Modified;            
            await dbContext.SaveChangesAsync();
            return true;
        }
    }
}
