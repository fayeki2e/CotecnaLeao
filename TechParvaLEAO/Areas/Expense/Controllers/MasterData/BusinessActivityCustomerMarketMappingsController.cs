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
     * Controller for Business Activity Customer Market Mappings Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class BusinessActivityCustomerMarketMappingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BusinessActivityCustomerMarketMappingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Business Activity Customer Market Mappings
         */
        // GET: Expense/BusinessActivityCustomerMarketMappings
        public async Task<IActionResult> Index(BusinessActivityCustomerMarketMappingSearchVM mappingSearchVM)
        {
            ViewData["BusinessActivityId"] = new SelectList(_context.BusinessActivities, "Id", "Name");
            ViewData["CustomerMarketId"] = new SelectList(_context.CustomerMarkets, "Id", "Name");

            var bACMList = _context.BusinessActivityCustomerMarketMapping.AsQueryable();
            if (mappingSearchVM.BusinessActivityId != null)
            {
                bACMList = bACMList.Where(c => c.BusinessActivityId == mappingSearchVM.BusinessActivityId);
            }
            if (mappingSearchVM.CustomerMarketId != null)
            {
                bACMList = bACMList.Where(c => c.CustomerMarketId == mappingSearchVM.CustomerMarketId);
            }

            return View(await bACMList.ToListAsync());
        }

        /*
         * Business Activity Customer Market Mappings Details
         */
        // GET: Expense/BusinessActivityCustomerMarketMappings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping
                .Include(b => b.BusinessActivity)
                .Include(b => b.CustomerMarket)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (businessActivityCustomerMarketMapping == null)
            {
                return NotFound();
            }

            return View(businessActivityCustomerMarketMapping);
        }

        /*
         * Show form to creates a new Business Activity Customer Market Mapping
         */
        // GET: Expense/BusinessActivityCustomerMarketMappings/Create
        public IActionResult Create()
        {
            ViewData["BusinessActivityId"] = new SelectList(_context.BusinessActivities, "Id", "Name");
            ViewData["CustomerMarketId"] = new SelectList(_context.CustomerMarkets, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Business Activity Custome rMarket Mapping from submitted form data
         */
        // POST: Expense/BusinessActivityCustomerMarketMappings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusinessActivityId,CustomerMarketId,Deactivated,Id")] BusinessActivityCustomerMarketMapping businessActivityCustomerMarketMapping)
        {
            if (ModelState.IsValid)
            {
                _context.Add(businessActivityCustomerMarketMapping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BusinessActivityId"] = new SelectList(_context.BusinessActivities, "Id", "Name", businessActivityCustomerMarketMapping.BusinessActivityId);
            ViewData["CustomerMarketId"] = new SelectList(_context.CustomerMarkets, "Id", "Name", businessActivityCustomerMarketMapping.CustomerMarketId);
            return View(businessActivityCustomerMarketMapping);
        }

        /*
         * Show form to edit Business Activity Customer Market Mapping
         */
        // GET: Expense/BusinessActivityCustomerMarketMappings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping.FindAsync(id);
            if (businessActivityCustomerMarketMapping == null)
            {
                return NotFound();
            }
            ViewData["BusinessActivityId"] = new SelectList(_context.BusinessActivities, "Id", "Name", businessActivityCustomerMarketMapping.BusinessActivityId);
            ViewData["CustomerMarketId"] = new SelectList(_context.CustomerMarkets, "Id", "Name", businessActivityCustomerMarketMapping.CustomerMarketId);
            return View(businessActivityCustomerMarketMapping);
        }

        /*
         * Updates Business Activity Customer Market Mapping from submitted form data
         */
        // POST: Expense/BusinessActivityCustomerMarketMappings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusinessActivityId,CustomerMarketId,Deactivated,Id")] BusinessActivityCustomerMarketMapping businessActivityCustomerMarketMapping)
        {
            if (id != businessActivityCustomerMarketMapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(businessActivityCustomerMarketMapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusinessActivityCustomerMarketMappingExists(businessActivityCustomerMarketMapping.Id))
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
            ViewData["BusinessActivityId"] = new SelectList(_context.BusinessActivities, "Id", "Name", businessActivityCustomerMarketMapping.BusinessActivityId);
            ViewData["CustomerMarketId"] = new SelectList(_context.CustomerMarkets, "Id", "Name", businessActivityCustomerMarketMapping.CustomerMarketId);
            return View(businessActivityCustomerMarketMapping);
        }

        /*
         * Confirmation deactivation of the Business Activity Customer Market Mapping
         */
        // GET: Expense/BusinessActivityCustomerMarketMappings/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping
                .Include(b => b.BusinessActivity)
                .Include(b => b.CustomerMarket)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (businessActivityCustomerMarketMapping == null)
            {
                return RedirectToAction("Index");
            }

            return View(businessActivityCustomerMarketMapping);
        }

        /*
         * Deactivate a Business Activity Customer Market Mapping
         */
        // POST: Expense/BusinessActivityCustomerMarketMappings/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var businessActivityCustomerMarketMapping = await _context.BusinessActivityCustomerMarketMapping.FindAsync(id);
            businessActivityCustomerMarketMapping.Deactivated = true;
            _context.BusinessActivityCustomerMarketMapping.Update(businessActivityCustomerMarketMapping);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BusinessActivityCustomerMarketMappingExists(int id)
        {
            return _context.BusinessActivityCustomerMarketMapping.Any(e => e.Id == id);
        }
    }
}
