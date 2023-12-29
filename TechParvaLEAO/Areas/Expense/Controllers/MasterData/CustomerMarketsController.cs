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
     * Controller for Customer Markets Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class CustomerMarketsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerMarketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Customer Markets
         */
        // GET: Expense/CustomerMarkets
        public async Task<IActionResult> Index(CustomerMarketSearchVM customerMarketSearchVM)
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");

            var customerMarkets = _context.CustomerMarkets.AsQueryable<CustomerMarket>();
            if (customerMarketSearchVM.Name != null)
            {
                customerMarkets = customerMarkets.Where(c => EF.Functions.Like(c.Name, "%"+customerMarketSearchVM.Name+"%"));
            }
            if (customerMarketSearchVM.Code != null)
            {
                customerMarkets = customerMarkets.Where(c => EF.Functions.Like(c.Code, "%"+customerMarketSearchVM.Code+"%"));
            }
            if (customerMarketSearchVM.LocationId != null)
            {
                customerMarkets = customerMarkets.Where(c => c.LocationId == customerMarketSearchVM.LocationId);
            }

            return View(await customerMarkets.ToListAsync());
        }

        /*
         * Customer Market Details
         */
        // GET: Expense/CustomerMarkets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerMarket = await _context.CustomerMarkets
                .Include(c => c.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (customerMarket == null)
            {
                return NotFound();
            }

            return View(customerMarket);
        }

        /*
         * Show form to creates a new Customer Market
         */
        // GET: Expense/CustomerMarkets/Create
        public IActionResult Create()
        {
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Customer Market from submitted form data
         */
        // POST: Expense/CustomerMarkets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,LocationId,IsVOC,Deactivated,Id")] CustomerMarket customerMarket)
        {
            if (ModelState.IsValid)
            {
                _context.Add(customerMarket);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", customerMarket.LocationId);
            return View(customerMarket);
        }

        /*
         * Show form to edit Customer Market
         */
        // GET: Expense/CustomerMarkets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerMarket = await _context.CustomerMarkets.FindAsync(id);
            if (customerMarket == null)
            {
                return NotFound();
            }
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", customerMarket.LocationId);
            return View(customerMarket);
        }

        /*
         * Updates Customer Market from submitted form data
         */
        // POST: Expense/CustomerMarkets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,LocationId,IsVOC,Deactivated,Id")] CustomerMarket customerMarket)
        {
            if (id != customerMarket.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customerMarket);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerMarketExists(customerMarket.Id))
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
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", customerMarket.LocationId);
            return View(customerMarket);
        }

        /*
         * Confirmation deactivation of the Customer Market
         */
        // GET: Expense/CustomerMarkets/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customerMarket = await _context.CustomerMarkets
                .Include(c => c.Location)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (customerMarket == null)
            {
                return RedirectToAction("Index");
            }

            return View(customerMarket);
        }

        /*
         * Deactivate a Customer Market
         */
        // POST: Expense/CustomerMarkets/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var customerMarket = await _context.CustomerMarkets.FindAsync(id);
            customerMarket.Deactivated = true;
            _context.CustomerMarkets.Update(customerMarket);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerMarketExists(int id)
        {
            return _context.CustomerMarkets.Any(e => e.Id == id);
        }
    }
}
