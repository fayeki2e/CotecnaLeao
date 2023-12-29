using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Controllers;

namespace TechParvaLEAO.Areas.Expense.Controllers
{
    /*
    * Json Api Controller for Expense Heads
    */
    [Route("api/[controller]")]
    [ApiController]
    public class ExpenseHeadsJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeServices _employeeServices;

        public class ExpenseHeadJson
        {
            public int Id { get; set; }
            public string ExpenseHeadDesc { get; set; }
        }

        public ExpenseHeadsJsonController(ApplicationDbContext context, IEmployeeServices employeeServices)
        {
            _context = context;
            _employeeServices = employeeServices;
        }

        // GET: api/ExpenseHeadsJson
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IEnumerable<ExpenseHead>>> GetExpenseHeads(int? employeeId)
        {
            if (employeeId == null) return null;
            var employee = await _employeeServices.GetEmployee(employeeId.Value);
            if (employee == null) return null;

            var expenseProfilesId = employee.ExpenseProfileId;
            var ehefm =  await _context.ExpenseHeadExpenseProfileMappings.Include(e=> e.ExpenseHead).Where(e => e.Deactivated==false && e.ExpenseProfileId == expenseProfilesId).ToListAsync();
            var result = new List<ExpenseHead>();
            foreach (var item in ehefm)
            {
                if (item.ExpenseHead.Deactivated == false)
                {
                    result.Add(item.ExpenseHead);
                }
            }
            return result;
        }
    }
}
