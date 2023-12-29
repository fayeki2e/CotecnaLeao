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
using TechParvaLEAO.Areas.Leave.Models.ViewModels;


namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
    * Json Api Controller for Leave Categories
    */
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveCategoriesJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext _context;
        private readonly LeaveRequestServices leaveRequestServices;

        public LeaveCategoriesJsonController(ApplicationDbContext context, LeaveRequestServices leaveRequestServices)
        {
            _context = context;
            this.leaveRequestServices = leaveRequestServices;

        }


        // GET: api/LeaveCategoriesJson
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DropDownVO>>> GetLeaveCategories(int leaveType)
        {
            return await leaveRequestServices.GetLeaveCategories(leaveType);
        }
    }

}
