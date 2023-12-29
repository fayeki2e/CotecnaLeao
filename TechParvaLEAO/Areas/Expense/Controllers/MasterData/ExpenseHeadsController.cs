using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Expense.Controllers.MasterData
{
    /*
     * Controller for Expense Heads Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class ExpenseHeadsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseHeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Expense Heads
         */
        // GET: Expense/ExpenseHeads
        public async Task<IActionResult> Index(ExpenseHeadSearchVM expenseHeadSearchVM)
        {
            
            var expenseHeads = _context.ExpenseHeads.AsQueryable<ExpenseHead>();
            if (expenseHeadSearchVM.AccountNumber != null)
            {
                expenseHeads = expenseHeads.Where(e => EF.Functions.Like(e.AccountNumber, "%"+expenseHeadSearchVM.AccountNumber+"%"));
            }
            if (expenseHeadSearchVM.ExpenseHeadDesc != null)
            {
                expenseHeads = expenseHeads.Where(e => EF.Functions.Like(e.ExpenseHeadDesc, expenseHeadSearchVM.ExpenseHeadDesc));
            }       
            return View(await expenseHeads.ToListAsync());
        }

        /*
         * Expense Head Details
         */
        // GET: Expense/ExpenseHeads/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHead = await _context.ExpenseHeads
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenseHead == null)
            {
                return NotFound();
            }

            return View(expenseHead);
        }

        /*
         * Show form to creates a new Expense Head
         */
        // GET: Expense/ExpenseHeads/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Expense Head from submitted form data
         */
        // POST: Expense/ExpenseHeads/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AccountNumber,ExpenseHeadDesc,Deactivated,Id")] ExpenseHead expenseHead)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expenseHead);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(expenseHead);
        }

        /*
         * Show form to edit Expense Head
         */
        // GET: Expense/ExpenseHeads/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHead = await _context.ExpenseHeads.FindAsync(id);
            if (expenseHead == null)
            {
                return NotFound();
            }
            return View(expenseHead);
        }

        /*
         * Updates Expense Head from submitted form data
         */
        // POST: Expense/ExpenseHeads/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AccountNumber,ExpenseHeadDesc,Deactivated,Id")] ExpenseHead expenseHead)
        {
            if (id != expenseHead.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expenseHead);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseHeadExists(expenseHead.Id))
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
            return View(expenseHead);
        }

        /*
         * Confirmation deactivation of the Expense Head
         */
        // GET: Expense/ExpenseHeads/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHead = await _context.ExpenseHeads
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (expenseHead == null)
            {
                return RedirectToAction("Index");
            }

            return View(expenseHead);
        }

        /*
         * Deactivate a Expense Head
         */
        // POST: Expense/ExpenseHeads/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var expenseHead = await _context.ExpenseHeads.FindAsync(id);
            expenseHead.Deactivated = true;
            _context.ExpenseHeads.Update(expenseHead);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseHeadExists(int id)
        {
            return _context.ExpenseHeads.Any(e => e.Id == id);
        }
    }
}
