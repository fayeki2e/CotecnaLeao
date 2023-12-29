using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Leave.Controllers.MasterData
{
    /*
     * Controller for Leave Categories Master data
     */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LeaveCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Leave Category
         */
        // GET: Leave/LeaveCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveCategories.ToListAsync());
        }

        /*
         * Leave Category Details
         */
        // GET: Leave/LeaveCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveCategory = await _context.LeaveCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveCategory == null)
            {
                return NotFound();
            }

            return View(leaveCategory);
        }

        /*
         * Show form to creates a new Leave Category
         */
        // GET: Leave/LeaveCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
        * Creates a new Leave Category from submitted form data
        */
        // POST: Leave/LeaveCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,Deactivated,Id")] LeaveCategory leaveCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveCategory);
        }

        /*
         * Show form to edit Leave Category
         */
        // GET: Leave/LeaveCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveCategory = await _context.LeaveCategories.FindAsync(id);
            if (leaveCategory == null)
            {
                return NotFound();
            }
            return View(leaveCategory);
        }

        /*
         * Updates Leave Category from submitted form data
         */
        // POST: Leave/LeaveCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Text,Deactivated,Id")] LeaveCategory leaveCategory)
        {
            if (id != leaveCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveCategoryExists(leaveCategory.Id))
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
            return View(leaveCategory);
        }

        /*
         * Confirmation deactivation of the Leave Category
         */
        // GET: Leave/LeaveCategories/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveCategory = await _context.LeaveCategories
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (leaveCategory == null)
            {
                return RedirectToAction("Index");
            }

            return View(leaveCategory);
        }

        /*
         * Deactivate a Leave Category
         */
        // POST: Leave/LeaveCategories/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var leaveCategory = await _context.LeaveCategories.FindAsync(id);
            leaveCategory.Deactivated = true;
            _context.LeaveCategories.Update(leaveCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveCategoryExists(int id)
        {
            return _context.LeaveCategories.Any(e => e.Id == id);
        }
    }
}
