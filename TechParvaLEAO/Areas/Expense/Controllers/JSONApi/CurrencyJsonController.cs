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
    * Json Api Controller for Currency
    */
    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext context;
        private readonly IEmployeeServices employeeServices;

        public class CurrencyDTO
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public CurrencyJsonController(ApplicationDbContext context, IEmployeeServices employeeServices)
        {
            this.context = context;
            this.employeeServices = employeeServices;
        }

        // GET: api/GetCurrencyOptions
        [HttpGet("{employeeId}")]
        public  ActionResult<IList<CurrencyDTO>> GetCurrencyOptions(int? employeeId)
        {
            if (employeeId == null) return Ok();
            var employee = context.Employees.FirstOrDefault(e=>e.Id==employeeId.Value);
            var forexAllowed= employee.CanCreateForexRequests;
            var currencyOptions = forexAllowed?
                 context.Currencies.Where(c=> c.Deactivated==false).Select(b=>new CurrencyDTO { Id=b.Id, Name=b.Name}): 
                 context.Currencies.Where(c=> c.IsForex==false).Select(b => new CurrencyDTO { Id = b.Id, Name = b.Name });
            return Ok(currencyOptions);
        }

    }
}
