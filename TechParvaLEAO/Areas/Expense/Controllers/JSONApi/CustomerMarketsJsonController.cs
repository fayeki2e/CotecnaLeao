using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Expense.Controllers
{
    /*
    * Json Api Controller for Customer Markets
    */
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerMarketsJsonController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomerMarketsJsonController(ApplicationDbContext context)
        {
            _context = context;
        }

        public class CustomerMarketJson
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }


        // GET: api/CustomerMarketsJson
        [HttpGet("{employeeId}")]
        public async Task<ActionResult<IEnumerable<CustomerMarket>>> GetCustomerMarkets(int employeeId, int businessActivity)
        {
            var employee = await _context.Employees.Where(e => e.Id == employeeId).FirstAsync();
            var businessActivityCustomerMarketMapping = null as List<BusinessActivityCustomerMarketMapping>;


            string sql_query = "";

            // sql_query = " select * from Employees where DesignationId in (select Id from Designations where name like '%manager%')";

            sql_query = "select E.* ";
            sql_query = sql_query + " from Employees E inner join Designations D on E.DesignationId = D.Id where AuthorizationProfileId = 3 and E.Deactivated = 0 ";
            sql_query = sql_query + " and(D.Name like '%head%' or  D.Name like '%director%') ";
            sql_query = sql_query + " and E.Id='" + employeeId +"'";
           
            var is_business_director_or_head = _context.Employees.FromSql(sql_query).ToList();



            if (employee.AuthorizationProfileId==1 || employee.AuthorizationProfileId == 2)
            {
                businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping.Include(e => e.CustomerMarket).Where(e => e.Deactivated==false && e.BusinessActivityId == businessActivity).ToListAsync();
            }
            else
            {
                if(is_business_director_or_head.Count() >0)
                {

                    businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping.Include(e => e.CustomerMarket).Where(e => e.BusinessActivityId == businessActivity && (e.Deactivated == false )).ToListAsync();

                }
                else
                {
                    businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping.Include(e => e.CustomerMarket).Where(e => e.BusinessActivityId == businessActivity && (e.Deactivated == false && (e.CustomerMarket.LocationId == employee.LocationId || e.CustomerMarket.LocationId == null))).ToListAsync();
                }

                
            }

            var result = new List<CustomerMarket>();
            foreach (var item in businessActivityCustomerMarketMapping)
            {
                if (item.CustomerMarket.Deactivated == false)
                {
                    result.Add(item.CustomerMarket);
                }
            }
            return result;
        }
    }
}
