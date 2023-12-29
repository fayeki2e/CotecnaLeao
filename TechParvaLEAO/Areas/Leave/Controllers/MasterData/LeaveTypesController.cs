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
    * Controller for Leave Type Master data
    */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LeaveTypesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveTypesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Leave Types
         */
        // GET: Leave/LeaveTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveTypes.ToListAsync());
        }

        /*
         * Leave Type Details
         */
        // GET: Leave/LeaveTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveType == null)
            {
                return NotFound();
            }

            return View(leaveType);
        }

        /*
         * Show form to creates a new Leave Type
         */
        // GET: Leave/LeaveTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Leave Type from submitted form data
         */
        // POST: Leave/LeaveTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Paid,Limit,Order,IsMaternity,IsMission,IsCommon,Deactivated,Id")] LeaveType leaveType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveType);
        }

        /*
         * Show form to edit Leave Type
         */
        // GET: Leave/LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes.FindAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            return View(leaveType);
        }

        /*
         * Updates Leave Type from submitted form data
         */
        // POST: Leave/LeaveTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Paid,Limit,Order,IsMaternity,IsMission,IsCommon,Deactivated,Id")] LeaveType leaveType)
        {
            if (id != leaveType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveTypeExists(leaveType.Id))
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
            return View(leaveType);
        }

        /*
         * Confirmation deactivation of the Leave Type
         */
        // GET: Leave/LeaveTypes/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveType = await _context.LeaveTypes
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (leaveType == null)
            {
                return RedirectToAction("Index");
            }

            return View(leaveType);
        }

        /*
         * Deactivate a Leave Type
         */
        // POST: Leave/LeaveTypes/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var leaveType = await _context.LeaveTypes.FindAsync(id);
            leaveType.Deactivated = true;
            _context.LeaveTypes.Update(leaveType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveTypeExists(int id)
        {
            return _context.LeaveTypes.Any(e => e.Id == id);
        }
    }
}
