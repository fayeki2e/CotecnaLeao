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
     * Controller for Expense Profiles Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class ExpenseProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Expense Profiles
         */
        // GET: Organization/ExpenseProfiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.ExpenseProfiles.ToListAsync());
        }

        /*
         * Expense Profile Details
         */
        // GET: Organization/ExpenseProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseProfile = await _context.ExpenseProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenseProfile == null)
            {
                return NotFound();
            }

            return View(expenseProfile);
        }

        /*
         * Show form to creates a new Expense Profile
         */
        // GET: Organization/ExpenseProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Expense Profile from submitted form data
         */
        // POST: Organization/ExpenseProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Deactivated,Id")] ExpenseProfile expenseProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expenseProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expenseProfile);
        }

        /*
         * Show form to edit Expense Profile
         */
        // GET: Organization/ExpenseProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseProfile = await _context.ExpenseProfiles.FindAsync(id);
            if (expenseProfile == null)
            {
                return NotFound();
            }
            return View(expenseProfile);
        }

        /*
         * Updates Expense Profile from submitted form data
         */
        // POST: Organization/ExpenseProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Deactivated,Id")] ExpenseProfile expenseProfile)
        {
            if (id != expenseProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expenseProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseProfileExists(expenseProfile.Id))
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
            return View(expenseProfile);
        }

        /*
         * Confirmation deactivation of the Expense Profile
         */
        // GET: Organization/ExpenseProfiles/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseProfile = await _context.ExpenseProfiles
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (expenseProfile == null)
            {
                return RedirectToAction("Index");
            }

            return View(expenseProfile);
        }

        /*
         * Deactivate a Expense Profile
         */
        // POST: Organization/ExpenseProfiles/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var expenseProfile = await _context.ExpenseProfiles.FindAsync(id);
            expenseProfile.Deactivated = true;
            _context.ExpenseProfiles.Update(expenseProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseProfileExists(int id)
        {
            return _context.ExpenseProfiles.Any(e => e.Id == id);
        }
    }
}
