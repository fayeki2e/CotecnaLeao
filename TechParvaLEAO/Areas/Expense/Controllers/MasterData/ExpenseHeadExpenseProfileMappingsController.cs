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
    public class EHEPSearchVO
    {
        public int? ExpenseHeadId { get; set; }
        public int? ExpenseProfileId { get; set; }
    }

    /*
     * Controller for Expense Head Expense Profile Mappings Master data
     */
    [Area("Expense")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class ExpenseHeadExpenseProfileMappingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ExpenseHeadExpenseProfileMappingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Expense Head Expense Profile Mappings
         */
        // GET: Expense/ExpenseHeadExpenseProfileMappings
        public async Task<IActionResult> Index([Bind]EHEPSearchVO vo)
        {
            ViewData["ExpenseHeadId"] = new SelectList(_context.ExpenseHeads, "Id", "ExpenseHeadDesc");
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name");
            var applicationDbContext = _context.ExpenseHeadExpenseProfileMappings.AsQueryable();
            if (vo!=null && vo.ExpenseHeadId != null)
            {
                applicationDbContext = applicationDbContext.Where(m => m.ExpenseHeadId == vo.ExpenseHeadId);
            }
            if (vo != null && vo.ExpenseProfileId != null)
            {
                applicationDbContext = applicationDbContext.Where(m => m.ExpenseProfileId == vo.ExpenseProfileId);
            }
            return View(await applicationDbContext.Include(e => e.ExpenseHead).Include(e => e.ExpenseProfile).ToListAsync());
        }

        /*
         * Expense Head Expense Profile Mapping Details
         */
        // GET: Expense/ExpenseHeadExpenseProfileMappings/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHeadExpenseProfileMapping = await _context.ExpenseHeadExpenseProfileMappings
                .Include(e => e.ExpenseHead)
                .Include(e => e.ExpenseProfile)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (expenseHeadExpenseProfileMapping == null)
            {
                return NotFound();
            }

            return View(expenseHeadExpenseProfileMapping);
        }

        /*
         * Show form to creates a new Expense Head Expense Profile Mapping
         */
        // GET: Expense/ExpenseHeadExpenseProfileMappings/Create
        public IActionResult Create()
        {
            ViewData["ExpenseHeadId"] = new SelectList(_context.ExpenseHeads, "Id", "ExpenseHeadDesc");
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Expense Head Expense Profile Mappings from submitted form data
         */
        // POST: Expense/ExpenseHeadExpenseProfileMappings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ExpenseHeadId,ExpenseProfileId,Deactivated,Id")] ExpenseHeadExpenseProfileMapping expenseHeadExpenseProfileMapping)
        {
            if (ModelState.IsValid)
            {
                _context.Add(expenseHeadExpenseProfileMapping);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ExpenseHeadId"] = new SelectList(_context.ExpenseHeads, "Id", "ExpenseHeadDesc", expenseHeadExpenseProfileMapping.ExpenseHeadId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", expenseHeadExpenseProfileMapping.ExpenseProfileId);
            return View(expenseHeadExpenseProfileMapping);
        }

        /*
         * Show form to edit Expense Head Expense Profile Mapping
         */
        // GET: Expense/ExpenseHeadExpenseProfileMappings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHeadExpenseProfileMapping = await _context.ExpenseHeadExpenseProfileMappings.FindAsync(id);
            if (expenseHeadExpenseProfileMapping == null)
            {
                return NotFound();
            }
            ViewData["ExpenseHeadId"] = new SelectList(_context.ExpenseHeads, "Id", "ExpenseHeadDesc", expenseHeadExpenseProfileMapping.ExpenseHeadId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", expenseHeadExpenseProfileMapping.ExpenseProfileId);
            return View(expenseHeadExpenseProfileMapping);
        }

        /*
         * Updates Expense Head Expense Profile Mapping from submitted form data
         */
        // POST: Expense/ExpenseHeadExpenseProfileMappings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ExpenseHeadId,ExpenseProfileId,Deactivated,Id")] ExpenseHeadExpenseProfileMapping expenseHeadExpenseProfileMapping)
        {
            if (id != expenseHeadExpenseProfileMapping.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(expenseHeadExpenseProfileMapping);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExpenseHeadExpenseProfileMappingExists(expenseHeadExpenseProfileMapping.Id))
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
            ViewData["ExpenseHeadId"] = new SelectList(_context.ExpenseHeads, "Id", "ExpenseHeadDesc", expenseHeadExpenseProfileMapping.ExpenseHeadId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", expenseHeadExpenseProfileMapping.ExpenseProfileId);
            return View(expenseHeadExpenseProfileMapping);
        }

        /*
         * Confirmation deactivation of the Expense Head Expense Profile Mapping
         */
        // GET: Expense/ExpenseHeadExpenseProfileMappings/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var expenseHeadExpenseProfileMapping = await _context.ExpenseHeadExpenseProfileMappings
                .Include(e => e.ExpenseHead)
                .Include(e => e.ExpenseProfile)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (expenseHeadExpenseProfileMapping == null)
            {
                return RedirectToAction("Index");
            }

            return View(expenseHeadExpenseProfileMapping);
        }

        /*
         * Deactivate a Expense Head Expense Profile Mapping
         */
        // POST: Expense/ExpenseHeadExpenseProfileMappings/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var expenseHeadExpenseProfileMapping = await _context.ExpenseHeadExpenseProfileMappings.FindAsync(id);
            expenseHeadExpenseProfileMapping.Deactivated = true;
            _context.ExpenseHeadExpenseProfileMappings.Update(expenseHeadExpenseProfileMapping);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExpenseHeadExpenseProfileMappingExists(int id)
        {
            return _context.ExpenseHeadExpenseProfileMappings.Any(e => e.Id == id);
        }
    }
}
