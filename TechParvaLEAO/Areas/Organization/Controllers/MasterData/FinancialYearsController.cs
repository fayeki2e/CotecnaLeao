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
     * Controller for Financial Years Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class FinancialYearsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinancialYearsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Financial Years
         */
        // GET: Organization/FinancialYears
        public async Task<IActionResult> Index()
        {
            return View(await _context.FinancialYears.ToListAsync());
        }

        /*
         * Financial Year Details
         */
        // GET: Organization/FinancialYears/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialYear = await _context.FinancialYears
                .FirstOrDefaultAsync(m => m.Id == id);
            if (financialYear == null)
            {
                return NotFound();
            }

            return View(financialYear);
        }

        /*
         * Show form to creates a new Financial Year
         */
        // GET: Organization/FinancialYears/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Financial Year from submitted form data
         */
        // POST: Organization/FinancialYears/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Code,StartDate,EndDate,Deactivated,Id")] FinancialYear financialYear)
        {
            if (ModelState.IsValid)
            {
                _context.Add(financialYear);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(financialYear);
        }

        /*
         * Show form to edit Financial Year
         */
        // GET: Organization/FinancialYears/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialYear = await _context.FinancialYears.FindAsync(id);
            if (financialYear == null)
            {
                return NotFound();
            }
            return View(financialYear);
        }

        /*
         * Updates Financial Year from submitted form data
         */
        // POST: Organization/FinancialYears/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Code,StartDate,EndDate,Deactivated,Id")] FinancialYear financialYear)
        {
            if (id != financialYear.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(financialYear);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FinancialYearExists(financialYear.Id))
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
            return View(financialYear);
        }

        /*
         * Confirmation deactivation of the Financial Year
         */
        // GET: Organization/FinancialYears/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var financialYear = await _context.FinancialYears
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (financialYear == null)
            {
                return RedirectToAction("Index");
            }

            return View(financialYear);
        }

        /*
         * Deactivate a Financial Year
         */
        // POST: Organization/FinancialYears/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var financialYear = await _context.FinancialYears.FindAsync(id);
            financialYear.Deactivated = true;
            _context.FinancialYears.Update(financialYear);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FinancialYearExists(int id)
        {
            return _context.FinancialYears.Any(e => e.Id == id);
        }
    }
}
