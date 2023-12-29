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
    * Controller for Leave Sub Categories Master data
    */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LeaveSubCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveSubCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Leave Sub Category
         */
        // GET: Leave/LeaveSubCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveSubCategories.ToListAsync());
        }

        /*
         * Leave Sub Category Details
         */
        // GET: Leave/LeaveSubCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveSubCategory = await _context.LeaveSubCategories
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveSubCategory == null)
            {
                return NotFound();
            }

            return View(leaveSubCategory);
        }

        /*
         * Show form to creates a new Leave Sub Category
         */
        // GET: Leave/LeaveSubCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Leave Sub Category from submitted form data
         */
        // POST: Leave/LeaveSubCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,Id")] LeaveSubCategory leaveSubCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveSubCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveSubCategory);
        }

        /*
         * Show form to edit Leave Sub Category
         */
        // GET: Leave/LeaveSubCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveSubCategory = await _context.LeaveSubCategories.FindAsync(id);
            if (leaveSubCategory == null)
            {
                return NotFound();
            }
            return View(leaveSubCategory);
        }

        /*
         * Updates Leave Sub Category from submitted form data
         */
        // POST: Leave/LeaveSubCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Text,Id")] LeaveSubCategory leaveSubCategory)
        {
            if (id != leaveSubCategory.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveSubCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveSubCategoryExists(leaveSubCategory.Id))
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
            return View(leaveSubCategory);
        }

        /*
         * Confirmation deactivation of the Leave SubCategory
         */
        // GET: Leave/LeaveSubCategories/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveSubCategory = await _context.LeaveSubCategories
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (leaveSubCategory == null)
            {
                return RedirectToAction("Index");
            }

            return View(leaveSubCategory);
        }

        /*
         * Deactivate a Leave Sub Category
         */
        // POST: Leave/LeaveSubCategories/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var leaveSubCategory = await _context.LeaveSubCategories.FindAsync(id);
            leaveSubCategory.Deactivated = true;
            _context.LeaveSubCategories.Update(leaveSubCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveSubCategoryExists(int id)
        {
            return _context.LeaveSubCategories.Any(e => e.Id == id);
        }
    }
}
