using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers
{
    /*
     * Controller for Employee Weekly Offs
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class EmployeeWeeklyOffsController : Controller
    {
        private readonly ApplicationDbContext context;
        protected readonly IApplicationRepository repo;

        public EmployeeWeeklyOffsController(ApplicationDbContext context, IApplicationRepository repo)
        {
            this.context = context;
            this.repo = repo;
        }

        /*
         * Show list of Employee Weekly Offs
         */
        // GET: Organization/EmployeeWeeklyOffs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = context.EmployeeWeeklyOff.Include(e => e.Employee);
            return View(await applicationDbContext.ToListAsync());
        }

        /*
         * Employee Weekly Off Details
         */
        // GET: Organization/EmployeeWeeklyOffs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeWeeklyOff = await context.EmployeeWeeklyOff
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeWeeklyOff == null)
            {
                return NotFound();
            }
    
            return View(employeeWeeklyOff);
        }

        /*
         * Show form to creates a new Employee Weekly Off
         */
        // GET: Organization/EmployeeWeeklyOffs/Create
        public async Task<ActionResult> Create(int id)
        {
            var employee = await repo.GetByIdAsync<Employee>(id);
            var employeeWeeklyOff = new EmployeeWeeklyOffViewModel();
            employeeWeeklyOff.EmployeeId = employee.Id;
            employeeWeeklyOff.EmployeeName = employee.Name;
            return View(employeeWeeklyOff);
        }

        /*
         * Creates a new Employee Weekly Off from submitted form data
         */
        // POST: Organization/EmployeeWeeklyOffs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind] EmployeeWeeklyOffViewModel employeeWeeklyOffViewModel)
        {
            if (ModelState.IsValid)
            {
                var endDate = new DateTime(2099, 12, 31);
                var previous = repo.Get<EmployeeWeeklyOff>(o =>
                    o.EmployeeId == employeeWeeklyOffViewModel.EmployeeId && o.ToDate == endDate);
                foreach (var prevRecord in previous)
                {
                    prevRecord.ToDate = employeeWeeklyOffViewModel.FromDate-TimeSpan.FromDays(1);
                    context.Entry(prevRecord).State = EntityState.Modified;
                }
                var employeeWeeklyOff = new EmployeeWeeklyOff
                {
                    EmployeeId = employeeWeeklyOffViewModel.EmployeeId,
                    FormDate = employeeWeeklyOffViewModel.FromDate,
                    ToDate = endDate,
                    WeeklyOffDay = employeeWeeklyOffViewModel.WeeklyOffDay,
                    OtherWeeklyOffDay = employeeWeeklyOffViewModel.OtherWeeklyOffDay,
                    OtherWeeklyOffRule = employeeWeeklyOffViewModel.OtherWeeklyOffRule,
                };
                
                context.Add(employeeWeeklyOff);
                await context.SaveChangesAsync();
                var employee = context.Employees.Find(employeeWeeklyOffViewModel.EmployeeId);
                employee.SpecificWeeklyOff = true;
                context.Entry(employee).State = EntityState.Modified;

                await context.SaveChangesAsync();                
                return RedirectToAction("Details", "Employees", new {Id = employee.Id});
            }
            ViewData["EmployeeId"] = new SelectList(context.Employees, "Id", "Id", employeeWeeklyOffViewModel.EmployeeId);
            ViewData["WeeklyOffDay"] = new SelectList(Enum.GetValues(typeof(DayOfWeek)));
            return View(employeeWeeklyOffViewModel);
        }
    }
}
