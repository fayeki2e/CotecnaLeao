using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Expense.Controllers.MasterData
{
    public enum PaymentRequestTypes
    {
        REIMBURSEMENT,
        ADVANCE
    }

    /*
     * Controller for Payment Request Rejection Reasons Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class PaymentRequestRejectionReasonsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PaymentRequestRejectionReasonsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Payment Request Rejection Reasons
         */
        // GET: Expense/PaymentRequestRejectionReasons
        public async Task<IActionResult> Index()
        {
            return View(await _context.PaymentRequestRejectionReasons.ToListAsync());
        }

        /*
         * Payment Request Rejection Reason Details
         */
        // GET: Expense/PaymentRequestRejectionReasons/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequestRejectionReason = await _context.PaymentRequestRejectionReasons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequestRejectionReason == null)
            {
                return NotFound();
            }

            return View(paymentRequestRejectionReason);
        }

        /*
         * Show form to creates a new Payment Request Rejection Reason
         */
        // GET: Expense/PaymentRequestRejectionReasons/Create
        public IActionResult Create()
        {
            ViewData["PaymentRequestType"] = new SelectList(Enum.GetValues(typeof(PaymentRequestTypes)));
            return View();
        }

        /*
         * Creates a new Payment Request Rejection Reason from submitted form data
         */
        // POST: Expense/PaymentRequestRejectionReasons/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Reason,Type,Id")] PaymentRequestRejectionReason paymentRequestRejectionReason)
        {
            if (ModelState.IsValid)
            {
                _context.Add(paymentRequestRejectionReason);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PaymentRequestType"] = new SelectList(Enum.GetValues(typeof(PaymentRequestTypes)));
            return View(paymentRequestRejectionReason);
        }

        /*
         * Show form to edit Payment Request Rejection Reason
         */
        // GET: Expense/PaymentRequestRejectionReasons/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequestRejectionReason = await _context.PaymentRequestRejectionReasons.FindAsync(id);
            if (paymentRequestRejectionReason == null)
            {
                return NotFound();
            }
            ViewData["PaymentRequestType"] = new SelectList(Enum.GetValues(typeof(PaymentRequestTypes)));
            return View(paymentRequestRejectionReason);
        }

        /*
         * Updates Payment Request Rejection Reason from submitted form data
         */
        // POST: Expense/PaymentRequestRejectionReasons/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Reason,Type,Id")] PaymentRequestRejectionReason paymentRequestRejectionReason)
        {
            if (id != paymentRequestRejectionReason.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(paymentRequestRejectionReason);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PaymentRequestRejectionReasonExists(paymentRequestRejectionReason.Id))
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
            ViewData["PaymentRequestType"] = new SelectList(Enum.GetValues(typeof(PaymentRequestTypes)));
            return View(paymentRequestRejectionReason);
        }

        /*
         * Confirmation deactivation of the Payment Request Rejection Reason
         */
        // GET: Expense/PaymentRequestRejectionReasons/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequestRejectionReason = await _context.PaymentRequestRejectionReasons
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequestRejectionReason == null)
            {
                return RedirectToAction("Index");
            }

            return View(paymentRequestRejectionReason);
        }

        /*
         * Deactivate a Payment Request Rejection Reason
         */
        // POST: Expense/PaymentRequestRejectionReasons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paymentRequestRejectionReason = await _context.PaymentRequestRejectionReasons.FindAsync(id);
            _context.PaymentRequestRejectionReasons.Remove(paymentRequestRejectionReason);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PaymentRequestRejectionReasonExists(int id)
        {
            return _context.PaymentRequestRejectionReasons.Any(e => e.Id == id);
        }
    }
}
