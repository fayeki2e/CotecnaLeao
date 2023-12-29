using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Expense.Controllers
{
    /*
   * Json Api Controller for Payment Requests
   */
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentRequestsJsonController : BaseDefaultController
    {
        private readonly IEmployeeServices _employeeServices;
        protected readonly IApplicationRepository _repo;
        protected readonly PaymentRequestService _paymentRequestService;

        public class PaymentRequestStatusJson
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public PaymentRequestsJsonController(IEmployeeServices employeeServices, IApplicationRepository repo, PaymentRequestService paymentRequestService)
        {
            _employeeServices = employeeServices;
            _paymentRequestService = paymentRequestService;
            _repo = repo;
        }

        //[HttpGet("{employeeId},{currencyId}")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentRequestStatusJson>>> GetPendingAdvances(int? employeeId, int? currencyId)
        {
            if (employeeId == null) return NotFound(404);
            var employee = await _repo.GetByIdAsync<Employee>(employeeId);
            return Ok(_paymentRequestService.GetPendingForexAdvances(employee, currencyId).Select(p=>new PaymentRequestStatusJson { Id=p.Id, Name=p.RequestNumber}));
        }

    }
}
