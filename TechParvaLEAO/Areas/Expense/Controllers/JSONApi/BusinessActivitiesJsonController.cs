using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Expense.Controllers
{
    /*
    * Json Api Controller for Business Activities
    */
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessActivitiesJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeServices _employeeServices;
        protected readonly IApplicationRepository _repo;

        public class BusinessActivityJson
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public BusinessActivitiesJsonController(ApplicationDbContext context, IEmployeeServices employeeServices, IApplicationRepository repo)
        {
            _context = context;
            _employeeServices = employeeServices;
            _repo = repo;
        }

        // GET: api/ExpenseHeadsJson
        [HttpGet("{employeeId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BusinessActivity>>> GetBusinessActivities()
        {
            //await _context.BusinessActivities.ToListAsync()            
            return Ok(await _repo.GetAsync<BusinessActivity>(b=>b.Id>=1));
        }


        private bool BusinessActivityExists(int id)
        {
            return _context.BusinessActivities.Any(e => e.Id == id);
        }
    }
}
