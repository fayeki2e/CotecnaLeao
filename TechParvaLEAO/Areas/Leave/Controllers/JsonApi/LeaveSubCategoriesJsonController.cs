using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
   * Json Api Controller for Leave Sub Categories
   */
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveSubCategoriesJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext context;
        private readonly LeaveRequestServices leaveRequestServices;


        public LeaveSubCategoriesJsonController(ApplicationDbContext context, LeaveRequestServices leaveRequestServices)
        {
            this.context = context;
            this.leaveRequestServices = leaveRequestServices;
        }

        // GET: api/LeaveSubCategoriesJson/5
        [HttpGet]
        public async Task<ActionResult<IList<DropDownVO>>> GetLeaveSubCategories(int leaveCategory, int leaveType)
        {
            return await leaveRequestServices.GetLeaveSubCategories(leaveCategory, leaveType);
        }
    }

}
