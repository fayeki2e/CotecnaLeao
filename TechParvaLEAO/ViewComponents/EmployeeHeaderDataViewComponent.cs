using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.ViewComponents
{
    public class EmployeeHeaderViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        private readonly IEmployeeServices _employeeServices;

        public EmployeeHeaderViewComponent(ApplicationDbContext context, 
            IEmployeeServices employeeServices)
        {
            _db = context;
            _employeeServices = employeeServices;
        }
        public async Task<IViewComponentResult> InvokeAsync(ClaimsPrincipal user)
        {
            var employee = await _employeeServices.GetEmployee(user);
            return View(employee);
        }
    }
}
