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
    /*
     * Controller for Approval Limit Profiles Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.FINANCE_MASTER)]
    public class ApprovalLimitProfilesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApprovalLimitProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Approval Limit Profiles
         */
        // GET: Organization/ApprovalLimitProfiles
        public async Task<IActionResult> Index()
        {
            return View(await _context.ApprovalLimitProfiles.ToListAsync());
        }

        /*
         * Approval Limit Profile Details
         */
        // GET: Organization/ApprovalLimitProfiles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalLimitProfile = await _context.ApprovalLimitProfiles
                .FirstOrDefaultAsync(m => m.Id == id);
            if (approvalLimitProfile == null)
            {
                return NotFound();
            }

            return View(approvalLimitProfile);
        }

        /*
         * Show form to creates a new Approval Limit Profile
         */
        // GET: Organization/ApprovalLimitProfiles/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Approval Limit Profile from submitted form data
         */
        // POST: Organization/ApprovalLimitProfiles/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Approval_Limit,Deactivated,Id")] ApprovalLimitProfile approvalLimitProfile)
        {
            if (ModelState.IsValid)
            {
                _context.Add(approvalLimitProfile);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(approvalLimitProfile);
        }

        /*
         * Show form to edit Approval Limit Profile
         */
        // GET: Organization/ApprovalLimitProfiles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalLimitProfile = await _context.ApprovalLimitProfiles.FindAsync(id);
            if (approvalLimitProfile == null)
            {
                return NotFound();
            }
            return View(approvalLimitProfile);
        }

        /*
        * Updates Approval Limit Profile from submitted form data
        */
        // POST: Organization/ApprovalLimitProfiles/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Approval_Limit,Deactivated,Id")] ApprovalLimitProfile approvalLimitProfile)
        {
            if (id != approvalLimitProfile.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(approvalLimitProfile);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApprovalLimitProfileExists(approvalLimitProfile.Id))
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
            return View(approvalLimitProfile);
        }

        /*
        * Confirmation deactivation of the Approval Limit Profile
        */
        // GET: Organization/ApprovalLimitProfiles/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var approvalLimitProfile = await _context.ApprovalLimitProfiles
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (approvalLimitProfile == null)
            {
                return RedirectToAction("Index");
            }

            return View(approvalLimitProfile);
        }

        /*
         * Deactivate a Approval Limit Profile
         */
        // POST: Organization/ApprovalLimitProfiles/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var approvalLimitProfile = await _context.ApprovalLimitProfiles.FindAsync(id);
            approvalLimitProfile.Deactivated = true;
            _context.ApprovalLimitProfiles.Update(approvalLimitProfile);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApprovalLimitProfileExists(int id)
        {
            return _context.ApprovalLimitProfiles.Any(e => e.Id == id);
        }
    }
}
