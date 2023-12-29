using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;
using System.Collections;
using TechParvaLEAO.Areas.Leave.Models;

namespace TechParvaLEAO.Areas.Leave.Services
{
    public class LeaveCreditAndUtilizationServices
    {
        private readonly IApplicationRepository context;
        private readonly LocationWorkdaysService locationWorkdaysService;
        private readonly IEmployeeServices employeeServices;

        public LeaveCreditAndUtilizationServices(IApplicationRepository context,
            LocationWorkdaysService locationWorkdaysService,
            IEmployeeServices employeeServices)
        {
            this.context = context;
            this.locationWorkdaysService = locationWorkdaysService;
            this.employeeServices = employeeServices;
        }

        public IEnumerable<LeaveCreditAndUtilization> GetOwnLeaveCompOffs(Employee employee)
        {
            return context.Get<LeaveCreditAndUtilization>(t => t.LeaveTypeId==2 && 
                t.EmployeeId == employee.Id, 
                q => q.OrderByDescending(s => s.AccrualDate));
        }
        public IEnumerable<LeaveCreditAndUtilization> GetOwnAvailableCompOffs(Employee employee)
        {
            return context.Get<LeaveCreditAndUtilization>(t => t.LeaveTypeId == 2 &&
                t.EmployeeId == employee.Id && t.AddedUtilized==0,
                q => q.OrderByDescending(s => s.AccrualDate));
        }
        
        public async Task<IEnumerable<LeaveCreditAndUtilization>> GetOnBehalfLeaveCompOffs(Employee employee)
        {
            var onBehalfEmployees = await employeeServices.GetOnFieldEmployeeForTimesheet(employee);

            return context.Get<LeaveCreditAndUtilization>(t => t.LeaveTypeId == 2 && 
                onBehalfEmployees.Any(e => t.EmployeeId == e.Id), 
                q => q.OrderByDescending(s => s.AccrualDate));
        }

    }
}


