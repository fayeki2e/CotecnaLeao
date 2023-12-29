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
    * Controller for Leave Rejection Reasons Master data
    */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LeaveRejectionReasonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveRejectionReasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Leave Rejection Reasons
         */
        // GET: Leave/LeaveRejectionReasons
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveRejectionReasons.ToListAsync());
        }

        /*
         * Leave Rejection Reason Details
         */
        // GET: Leave/LeaveRejectionReasons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRejectionReason = await _context.LeaveRejectionReasons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveRejectionReason == null)
            {
                return NotFound();
            }

            return View(leaveRejectionReason);
        }

        /*
         * Show form to creates a new Leave Rejection Reason
         */
        // GET: Leave/LeaveRejectionReasons/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Leave Rejection Reason from submitted form data
         */
        // POST: Leave/LeaveRejectionReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Text,Deactivated,Id")] LeaveRejectionReason leaveRejectionReason)
        {
            if (ModelState.IsValid)
            {
                _context.Add(leaveRejectionReason);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveRejectionReason);
        }

        /*
         * Show form to edit Leave Rejection Reason
         */
        // GET: Leave/LeaveRejectionReasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRejectionReason = await _context.LeaveRejectionReasons.FindAsync(id);
            if (leaveRejectionReason == null)
            {
                return NotFound();
            }
            return View(leaveRejectionReason);
        }

        /*
         * Updates Leave Rejection Reason from submitted form data
         */
        // POST: Leave/LeaveRejectionReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Text,Deactivated,Id")] LeaveRejectionReason leaveRejectionReason)
        {
            if (id != leaveRejectionReason.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRejectionReason);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRejectionReasonExists(leaveRejectionReason.Id))
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
            return View(leaveRejectionReason);
        }

        /*
         * Confirmation deactivation of the Leave Rejection Reason
         */
        // GET: Leave/LeaveRejectionReasons/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRejectionReason = await _context.LeaveRejectionReasons
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (leaveRejectionReason == null)
            {
                return RedirectToAction("Index");
            }

            return View(leaveRejectionReason);
        }

        /*
         * Deactivate a Leave Rejection Reason
         */
        // POST: Leave/LeaveRejectionReasons/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var leaveRejectionReason = await _context.LeaveRejectionReasons.FindAsync(id);
            leaveRejectionReason.Deactivated = true;
            _context.LeaveRejectionReasons.Update(leaveRejectionReason);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveRejectionReasonExists(int id)
        {
            return _context.LeaveRejectionReasons.Any(e => e.Id == id);
        }
    }
}
