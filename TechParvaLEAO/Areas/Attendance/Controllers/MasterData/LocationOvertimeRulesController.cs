using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Attendance.Controllers.MasterData
{
    /*
     * Controller for Location Overtime Rule Master data
     */
    [Area("Attendance")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LocationOvertimeRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationOvertimeRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Location Overtime Rules
         */
        // GET: Attendance/LocationOvertimeRules
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.LocationOvertimeRule.Include(l => l.Location);
            return View(await applicationDbContext.ToListAsync());
        }

        /*
         * Location Overtime Rule Details
         */
        // GET: Attendance/LocationOvertimeRules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationOvertimeRule = await _context.LocationOvertimeRule
                .Include(l => l.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (locationOvertimeRule == null)
            {
                return NotFound();
            }

            return View(locationOvertimeRule);
        }

        /*
         * Show form to creates a new Location Overtime Rule
         */
        // GET: Attendance/LocationOvertimeRules/Create
        public IActionResult Create()
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Location Overtime Rule from submitted form data
         */
        // POST: Attendance/LocationOvertimeRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LocationId,OvertimeMultiplier,Deactivated,Id")] LocationOvertimeRule locationOvertimeRule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(locationOvertimeRule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", locationOvertimeRule.LocationId);
            return View(locationOvertimeRule);
        }

        /*
         * Show form to edit Location Overtime Rule
         */
        // GET: Attendance/LocationOvertimeRules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationOvertimeRule = await _context.LocationOvertimeRule.FindAsync(id);
            if (locationOvertimeRule == null)
            {
                return NotFound();
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", locationOvertimeRule.LocationId);
            return View(locationOvertimeRule);
        }

        /*
         * Updates Location Overtime Rule from submitted form data
         */
        // POST: Attendance/LocationOvertimeRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,OvertimeMultiplier,Deactivated,Id")] LocationOvertimeRule locationOvertimeRule)
        {
            if (id != locationOvertimeRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(locationOvertimeRule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationOvertimeRuleExists(locationOvertimeRule.Id))
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
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", locationOvertimeRule.LocationId);
            return View(locationOvertimeRule);
        }

        /*
         * Confirmation deactivation of the Location Overtime Rule
         */
        // GET: Attendance/LocationOvertimeRules/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var locationOvertimeRule = await _context.LocationOvertimeRule
                .Include(l => l.Location)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (locationOvertimeRule == null)
            {
                return RedirectToAction("Index");
            }

            return View(locationOvertimeRule);
        }

        /*
         * Deactivate a Location Overtime Rule
         */
        // POST: Attendance/LocationOvertimeRules/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var locationOvertimeRule = await _context.LocationOvertimeRule.FindAsync(id);
            locationOvertimeRule.Deactivated = true;
            _context.LocationOvertimeRule.Update(locationOvertimeRule);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationOvertimeRuleExists(int id)
        {
            return _context.LocationOvertimeRule.Any(e => e.Id == id);
        }
    }
}
