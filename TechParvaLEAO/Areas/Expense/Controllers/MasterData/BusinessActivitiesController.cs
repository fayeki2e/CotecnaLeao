using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Expense.Controllers.MasterData
{
    /*
     * Controller for Business Activities Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class BusinessActivitiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessActivitiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Business Activities
         */
        // GET: Expense/BusinessActivities
        public async Task<IActionResult> Index(BusinessActivitySearchVM businessActivitySearchVM)
        {
            ViewData["BusinessUnitid"] = new SelectList(_context.BusinessUnits, "Id", "Name");
           
            var businessActivities = _context.BusinessActivities.AsQueryable<BusinessActivity>();
            if (businessActivitySearchVM.Name != null)
            {
                businessActivities = businessActivities.Where(b => EF.Functions.Like(b.Name, "%"+businessActivitySearchVM.Name+"%"));
            }
            if (businessActivitySearchVM.Code != null)
            {
                businessActivities = businessActivities.Where(b => EF.Functions.Like(b.Code, "%"+businessActivitySearchVM.Code+"%"));
            }
            if (businessActivitySearchVM.BusinessUnitid != null)
            {
                businessActivities = businessActivities.Where(b => b.BusinessUnitid == businessActivitySearchVM.BusinessUnitid);
            }

            return View(await businessActivities.ToListAsync());
        }

        /*
         * Business Activity Details
         */
        // GET: Expense/BusinessActivities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivity = await _context.BusinessActivities
                .Include(b => b.BusinessUnit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessActivity == null)
            {
                return NotFound();
            }

            return View(businessActivity);
        }

        /*
         * Show form to creates a new Business Activity
         */
        // GET: Expense/BusinessActivities/Create
        public IActionResult Create()
        {
            ViewData["BusinessUnitid"] = new SelectList(_context.BusinessUnits, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Business Activity from submitted form data
         */
        // POST: Expense/BusinessActivities/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,IsVOC,BusinessUnitid,Deactivated,Id")] BusinessActivity businessActivity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(businessActivity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessUnitid"] = new SelectList(_context.BusinessUnits, "Id", "Name", businessActivity.BusinessUnitid);
            return View(businessActivity);
        }

        /*
         * Show form to edit Business Activity
         */
        // GET: Expense/BusinessActivities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivity = await _context.BusinessActivities.FindAsync(id);
            if (businessActivity == null)
            {
                return NotFound();
            }
            ViewData["BusinessUnitid"] = new SelectList(_context.BusinessUnits, "Id", "Name", businessActivity.BusinessUnitid);
            return View(businessActivity);
        }

        /*
         * Updates Business Activity from submitted form data
         */
        // POST: Expense/BusinessActivities/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,IsVOC,BusinessUnitid,Deactivated,Id")] BusinessActivity businessActivity)
        {
            if (id != businessActivity.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessActivity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessActivityExists(businessActivity.Id))
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
            ViewData["BusinessUnitid"] = new SelectList(_context.BusinessUnits, "Id", "Name", businessActivity.BusinessUnitid);
            return View(businessActivity);
        }

        /*
         * Confirmation deactivation of the Business Activity
         */
        // GET: Expense/BusinessActivities/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivity = await _context.BusinessActivities
                .Include(b => b.BusinessUnit)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (businessActivity == null)
            {
                return RedirectToAction("Index");
            }

            return View(businessActivity);
        }

        /*
         * Deactivate a Business Activity
         */
        // POST: Expense/BusinessActivities/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var businessActivity = await _context.BusinessActivities.FindAsync(id);
            businessActivity.Deactivated = true;
            _context.BusinessActivities.Update(businessActivity);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessActivityExists(int id)
        {
            return _context.BusinessActivities.Any(e => e.Id == id);
        }
    }
}
