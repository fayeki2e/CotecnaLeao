using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Attendance.Controllers.MasterData
{
    /*
     * Controller for Jobs Master data
     */
    [Area("Attendance")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class JobsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JobsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Jobs
         */
        // GET: Attendance/Jobs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Jobs.Include(j => j.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        /*
         * Job Details
         */
        // GET: Attendance/Jobs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Client)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        /*
         * Show form to creates a new Job
         */
        // GET: Attendance/Jobs/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            return View();
        }

        /*
         * Creates a new Job from submitted form data
         */
        // POST: Attendance/Jobs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,ClientId,Deactivated,Id")] Job job)
        {
            if (ModelState.IsValid)
            {
                _context.Add(job);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", job.ClientId);
            return View(job);
        }

        /*
         * Show form to edit Job
         */
        // GET: Attendance/Jobs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs.FindAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", job.ClientId);
            return View(job);
        }

        /*
         * Updates Job from submitted form data
         */
        // POST: Attendance/Jobs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,ClientId,Deactivated,Id")] Job job)
        {
            if (id != job.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(job);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JobExists(job.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name", job.ClientId);
            return View(job);
        }

        /*
         * Confirmation deactivation of the Job
         */
        // GET: Attendance/Jobs/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var job = await _context.Jobs
                .Include(j => j.Client)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        /*
         * Deactivate a Job
         */
        // POST: Attendance/Jobs/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var job = await _context.Jobs.FindAsync(id);
            job.Deactivated = true;
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool JobExists(int id)
        {
            return _context.Jobs.Any(e => e.Id == id);
        }
    }
}
