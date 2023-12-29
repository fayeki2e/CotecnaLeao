using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    /*
     * Controller for Designations Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class DesignationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DesignationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Designations
         */
        // GET: Organization/Designations
        public async Task<IActionResult> Index(DesignationSearchVM designationSearchVM)
        {
            var designations = _context.Designations.AsQueryable<Designation>();
            if (designationSearchVM.Name != null)
            {
                designations = designations.Where(c => c.Name == designationSearchVM.Name);
            }

            return View(await designations.ToListAsync());
        }

        /*
         * Designation Details
         */
        // GET: Organization/Designations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (designation == null)
            {
                return NotFound();
            }

            return View(designation);
        }

        /*
         * Show form to creates a new Designation
         */
        // GET: Organization/Designations/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Designation from submitted form data
         */
        // POST: Organization/Designations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Deactivated,Id")] Designation designation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(designation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(designation);
        }

        /*
         * Show form to edit Designation
         */
        // GET: Organization/Designations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designations.FindAsync(id);
            if (designation == null)
            {
                return NotFound();
            }
            return View(designation);
        }

        /*
         * Updates Designation from submitted form data
         */
        // POST: Organization/Designations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Deactivated,Id")] Designation designation)
        {
            if (id != designation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(designation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DesignationExists(designation.Id))
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
            return View(designation);
        }

        /*
         * Confirmation deactivation of the Designation
         */
        // GET: Organization/Designations/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var designation = await _context.Designations
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated==false);
            if (designation == null)
            {
                return RedirectToAction("Index");
            }

            return View(designation);
        }

        /*
         * Deactivate a Designation
         */
        // POST: Organization/Designations/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var designation = await _context.Designations.FindAsync(id);
            designation.Deactivated = true;
            _context.Designations.Update(designation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DesignationExists(int id)
        {
            return _context.Designations.Any(e => e.Id == id);
        }
    }
}
