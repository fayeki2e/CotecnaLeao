using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class OvertimeRulesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OvertimeRulesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Organization/OvertimeRules
        public async Task<IActionResult> Index()
        {
            return View(await _context.OvertimeRule.ToListAsync());
        }

        // GET: Organization/OvertimeRules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeRule = await _context.OvertimeRule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overtimeRule == null)
            {
                return NotFound();
            }

            return View(overtimeRule);
        }

        // GET: Organization/OvertimeRules/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Organization/OvertimeRules/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,OvertimeMultiplier,Deactivated,Id")] OvertimeRule overtimeRule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(overtimeRule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(overtimeRule);
        }

        // GET: Organization/OvertimeRules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeRule = await _context.OvertimeRule.FindAsync(id);
            if (overtimeRule == null)
            {
                return NotFound();
            }
            return View(overtimeRule);
        }

        // POST: Organization/OvertimeRules/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,OvertimeMultiplier,Deactivated,Id")] OvertimeRule overtimeRule)
        {
            if (id != overtimeRule.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(overtimeRule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OvertimeRuleExists(overtimeRule.Id))
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
            return View(overtimeRule);
        }

        // GET: Organization/OvertimeRules/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var overtimeRule = await _context.OvertimeRule
                .FirstOrDefaultAsync(m => m.Id == id);
            if (overtimeRule == null)
            {
                return RedirectToAction("Index");
            }

            return View(overtimeRule);
        }

        // POST: Organization/OvertimeRules/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var overtimeRule = await _context.OvertimeRule.FindAsync(id);
            overtimeRule.Deactivated = true;
            _context.Entry(overtimeRule).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OvertimeRuleExists(int id)
        {
            return _context.OvertimeRule.Any(e => e.Id == id);
        }
    }
}
