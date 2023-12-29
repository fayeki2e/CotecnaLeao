using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using MediatR;
using AutoMapper;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Organization.Models;
using System.Threading;


namespace TechParvaLEAO.Areas.Organization.Handler
{
    /*
     * Handler class to add New Employee Weekly Off.
     */
    public class NewEmployeeWeeklyOffHandler : IRequestHandler<EmployeeWeeklyOffViewModel, bool>
    {
        private readonly IApplicationRepository repository;
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly IEmployeeServices employeeServices;
        public NewEmployeeWeeklyOffHandler(IApplicationRepository repository, IMapper mapper,
            ApplicationDbContext dbContext,
            IEmployeeServices employeeServices
            )
        {
            this.repository = repository;
            this.mapper = mapper;
            this.dbContext = dbContext;
            this.employeeServices = employeeServices;
        }

        public async Task<bool> Handle(EmployeeWeeklyOffViewModel employeeWeeklyOffVm, CancellationToken cancellationToken)
        {
            EmployeeWeeklyOff employeeWeeklyOff = null;
            employeeWeeklyOff = new EmployeeWeeklyOff
            {
                FormDate = employeeWeeklyOffVm.FromDate,
                ToDate = employeeWeeklyOff.ToDate,
                EmployeeId = employeeWeeklyOff.EmployeeId,
                WeeklyOffDay = employeeWeeklyOff.WeeklyOffDay
            };
            dbContext.EmployeeWeeklyOff.Add(employeeWeeklyOff);
            await dbContext.SaveChangesAsync();           
            return true;
        }
    }
}
