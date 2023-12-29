using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Leave.Controllers.JsonApi
{
    /*
    * Json Api Controller for Comp Off
    */
    [Route("api/[controller]")]
    [ApiController]
    public class CompOffJsonController : BaseDefaultController
    {
        protected readonly IApplicationRepository repo;
        protected readonly LeaveRequestServices leaveRequestServices;

        public class CompOffJson
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public CompOffJsonController(IApplicationRepository repo, LeaveRequestServices leaveRequestServices)
        {
            this.repo = repo;
            this.leaveRequestServices = leaveRequestServices;
        }

        [HttpGet("{employeeId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CompOffJson>>> GetCompOffs(int? employeeId)
        {
            if (employeeId == null) return NotFound(404);
            //await _context.BusinessActivities.ToListAsync()            
            var employee = await repo.GetByIdAsync<Employee>(employeeId);
            var comOffJson = leaveRequestServices.GetAvailableCompOffs(employee).
                Select(p => 
                new CompOffJson { Id = p.Id, Name = string.Concat(p.AccrualDate.Value.ToString("dd/MM/yyyy"), 
                                                        (p.CarryForward > 0)? " (" + p.CarryForward.ToString() +")": "")});
            return Ok(comOffJson);
        }


    }
}