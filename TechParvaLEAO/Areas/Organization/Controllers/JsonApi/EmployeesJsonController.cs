using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Organization.Controllers
{
    /*
     * Json Api Controller for Employees Json
     */
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesJsonController : BaseDefaultController
    {
        private readonly ApplicationDbContext context;
        private readonly IEmployeeServices employeeServices;
        private readonly PaymentRequestService paymentRequestService;
        public class EmployeeJson
        {
            public int Id { get; set; }
            public string EmployeeCode { get; set; }
            public string Name { get; set; }
            public string Designation { get; set; }
            public string Location { get; set; }
            public double BasicSalary { get; set; }
            public double INRAdvanceReceived { get; set; }
            public double INRReimbursementApproved { get; set; }
            public double INRPendingAmount { get; set; }
            public bool CanHoldCreditCard { get; set; }
            public string CurrencyCode { get; set; }

            public static implicit operator EmployeeJson(ActionResult<EmployeeJson> v)
            {
                throw new NotImplementedException();
            }
        }

        public EmployeesJsonController(ApplicationDbContext context, IEmployeeServices employeeServices, PaymentRequestService paymentRequestService)
        {
            this.context = context;
            this.employeeServices = employeeServices;
            this.paymentRequestService = paymentRequestService;
        }

        // GET: api/EmployeesJson/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeJson>> GetEmployee(int? id, int? currencyId)
        {            
            var employee = await context.Employees
                .Include(e=>e.Designation)
                .Include(e => e.Location)
                .Where(e=> e.Id== id).FirstOrDefaultAsync<Employee>();
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            var currencyObj = await context.Currencies.Where(c=>c.Id==currency).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            var paidAdvaces = paymentRequestService.GetPaidAdvances(employee, currencyId);
            var approvedReimbursements  = paymentRequestService.GetApprovedReimbursements(employee, currencyId);
            var paidAmount = 0.0d;
            var reimbusedAmount = 0.0d;
            foreach(var advance in paidAdvaces)
            {
                paidAmount += advance.PaidAmount;
            }
            foreach (var reimb in approvedReimbursements)
            {
                reimbusedAmount += (reimb.Amount - reimb.PaidAmount);
            }
            EmployeeJson employeeJson = new EmployeeJson
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                Name = employee.Name,
                Designation = employee.Designation.Name,
                Location = employee.Location.Name,
                BasicSalary = await employeeServices.GetBasicSalary(employee.Id, DateTime.Today),
                INRAdvanceReceived = paidAmount,
                INRReimbursementApproved = reimbusedAmount,
                INRPendingAmount = paidAmount - reimbusedAmount,
                CanHoldCreditCard = employee.CanHoldCreditCard,
                CurrencyCode = currencyObj.Name
            };
            return employeeJson;
        }           
        }
    }

