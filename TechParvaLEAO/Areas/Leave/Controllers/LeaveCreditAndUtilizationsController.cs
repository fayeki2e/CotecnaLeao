using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Organization.Models;


namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
     * Controller for Leave Credit And Utilizations
     */
    [Area("Leave")]
    public class LeaveCreditAndUtilizationsController : BaseViewController
    {
        private readonly LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices;
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeServices employeeServices;

        public LeaveCreditAndUtilizationsController(ApplicationDbContext context,
            LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices,
            IEmployeeServices employeeServices)
        {
            _context = context;
            this.leaveCreditAndUtilizationServices = leaveCreditAndUtilizationServices;
            this.employeeServices = employeeServices;
        }

        /*
         * Show list of Own Comp Off Leaves
         */
        // GET: Leave/LeaveCreditAndUtilizations/LeaveListOwn
        [Authorize]
        public ActionResult CompOffsListOwn()
        {
            ViewData["LeaveTypeId"] = 2;
            return View("CompOffsList", leaveCreditAndUtilizationServices.GetOwnLeaveCompOffs(GetEmployee()));
        }

        /*
         * Show list of On behalf of Comp Off Leaves
         */
        // GET: Leave/LeaveCreditAndUtilizations
        public async Task<ActionResult> Index()
        {
            ViewData["LeaveTypeId"] = 2;
            return View("CompOffsList", await leaveCreditAndUtilizationServices.GetOnBehalfLeaveCompOffs(GetEmployee()));
        }
    }
}
