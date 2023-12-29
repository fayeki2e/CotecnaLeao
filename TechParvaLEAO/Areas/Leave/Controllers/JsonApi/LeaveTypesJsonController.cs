using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
   * Json Api Controller for Leave Types
   */
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveTypesJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext context;
        private readonly LeaveRequestServices leaveRequestServices;
        public LeaveTypesJsonController(ApplicationDbContext context, LeaveRequestServices leaveRequestServices)
        {
            this.context = context;
            this.leaveRequestServices = leaveRequestServices;
        }

        // GET: api/LeaveTypesJson
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IList<LeaveType>>> GetLeaveTypes(int employeeId)
        {
            return await leaveRequestServices.GetLeaveTypesForEmployee(employeeId);
        }
    }
}
