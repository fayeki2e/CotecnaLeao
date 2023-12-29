using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Leave.Controllers.MasterData
{
    /*
     * Controller for Holidays Master data
     */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class HolidaysController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HolidaysController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Holidays
         */
        // GET: Leave/Holidays
        public async Task<IActionResult> Index(HolidaySearchVM searchVM)
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            var holidays = _context.Holidays.AsQueryable<Holiday>();
            if (searchVM.LocationId != 0)
            {
                holidays = holidays.Where(h => h.LocationId == searchVM.LocationId);
            }
            return View(await holidays.ToListAsync());
        }

        /*
         * Holiday Details
         */
        // GET: Leave/Holidays/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holidays
                .Include(h => h.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (holiday == null)
            {
                return NotFound();
            }

            return View(holiday);
        }

        /*
         * Show form to creates a new Holiday
         */
        // GET: Leave/Holidays/Create
        public IActionResult Create()
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Holiday from submitted form data
         */
        // POST: Leave/Holidays/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HolidayDate,Reason,IsHalfDay,LocationId,Deactivated,Id")] Holiday holiday)
        {
            if (ModelState.IsValid)
            {
                _context.Add(holiday);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", holiday.LocationId);
            return View(holiday);
        }

        /*
         * Show form to edit Holiday
         */
        // GET: Leave/Holidays/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holidays.FindAsync(id);
            if (holiday == null)
            {
                return NotFound();
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", holiday.LocationId);
            return View(holiday);
        }

        /*
         * Updates Holiday from submitted form data
         */
        // POST: Leave/Holidays/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HolidayDate,Reason,IsHalfDay,LocationId,Deactivated,Id")] Holiday holiday)
        {
            if (id != holiday.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(holiday);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HolidayExists(holiday.Id))
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
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", holiday.LocationId);
            return View(holiday);
        }

        /*
         * Confirmation deactivation of the Holiday
         */
        // GET: Leave/Holidays/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var holiday = await _context.Holidays
                .Include(h => h.Location)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (holiday == null)
            {
                return RedirectToAction("Index");
            }

            return View(holiday);
        }

        /*
         * Deactivate a Holiday
         */
        // POST: Leave/Holidays/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var holiday = await _context.Holidays.FindAsync(id);
            holiday.Deactivated = true;
            _context.Holidays.Update(holiday);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HolidayExists(int id)
        {
            return _context.Holidays.Any(e => e.Id == id);
        }
    }
}
