using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    /*
     * Controller for Employee Claim Series Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class EmployeeClaimSeriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeClaimSeriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Employee Claim Series
         */
        // GET: Organization/EmployeeClaimSeries
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.EmployeeClaimSeries.Include(e => e.Employee).Include(e => e.FinancialYear);
            return View(await applicationDbContext.ToListAsync());
        }

        /*
         * Employee Claim Series Details
         */
        // GET: Organization/EmployeeClaimSeries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeClaimSeries = await _context.EmployeeClaimSeries
                .Include(e => e.Employee)
                .Include(e => e.FinancialYear)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeClaimSeries == null)
            {
                return NotFound();
            }

            return View(employeeClaimSeries);
        }

        /*
         * Show form to creates a new Employee Claim Series
         */
        // GET: Organization/EmployeeClaimSeries/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");
            ViewData["FinancialYearId"] = new SelectList(_context.FinancialYears, "Id", "Code");
            return View();
        }

        /*
        * Creates a new Employee Claim Series from submitted form data
        */
        // POST: Organization/EmployeeClaimSeries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FinancialYearId,SerialNumber,Deactivated,Id")] EmployeeClaimSeries employeeClaimSeries)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeClaimSeries);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeClaimSeries.EmployeeId);
            ViewData["FinancialYearId"] = new SelectList(_context.FinancialYears, "Id", "Code", employeeClaimSeries.FinancialYearId);
            return View(employeeClaimSeries);
        }

        /*
         * Show form to edit Employee Claim Series
         */
        // GET: Organization/EmployeeClaimSeries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeClaimSeries = await _context.EmployeeClaimSeries.FindAsync(id);
            if (employeeClaimSeries == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeClaimSeries.EmployeeId);
            ViewData["FinancialYearId"] = new SelectList(_context.FinancialYears, "Id", "Code", employeeClaimSeries.FinancialYearId);
            return View(employeeClaimSeries);
        }

        /*
         * Updates Employee Claim Series from submitted form data
         */
        // POST: Organization/EmployeeClaimSeries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FinancialYearId,SerialNumber,Deactivated,Id")] EmployeeClaimSeries employeeClaimSeries)
        {
            if (id != employeeClaimSeries.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeClaimSeries);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeClaimSeriesExists(employeeClaimSeries.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeClaimSeries.EmployeeId);
            ViewData["FinancialYearId"] = new SelectList(_context.FinancialYears, "Id", "Code", employeeClaimSeries.FinancialYearId);
            return View(employeeClaimSeries);
        }

        /*
         * Confirmation deactivation of the Employee Claim Series
         */
        // GET: Organization/EmployeeClaimSeries/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeClaimSeries = await _context.EmployeeClaimSeries
                .Include(e => e.Employee)
                .Include(e => e.FinancialYear)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (employeeClaimSeries == null)
            {
                return RedirectToAction("Index");
            }

            return View(employeeClaimSeries);
        }

        /*
         * Deactivate a Employee Claim Series
         */
        // POST: Organization/EmployeeClaimSeries/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var employeeClaimSeries = await _context.EmployeeClaimSeries.FindAsync(id);
            employeeClaimSeries.Deactivated = true;
            _context.EmployeeClaimSeries.Update(employeeClaimSeries);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeClaimSeriesExists(int id)
        {
            return _context.EmployeeClaimSeries.Any(e => e.Id == id);
        }
    }
}
