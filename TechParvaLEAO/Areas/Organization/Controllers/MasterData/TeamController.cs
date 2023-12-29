using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class TeamController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeamController(ApplicationDbContext context)
        {
            _context = context;
        }


        public async Task<IActionResult> Index()
        {
            var Teamlist = _context.Team.AsQueryable<Team>();
            //if (designationSearchVM.Name != null)
            //{
            //    designations = designations.Where(c => c.Name == designationSearchVM.Name);
            //}

            return View(await Teamlist.ToListAsync());
        }



        // GET: TeamController
        //public ActionResult Index()
        //{
              

        //    return View(teamList);
        //}

        // GET: TeamController/Details/5
      

      

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Team = await _context.Team.FindAsync(id);
            if (Team == null)
            {
                return NotFound();
            }
            return View(Team);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TeamName,Id")] Team team)
        {
            if (id != team.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                  
                }
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TeamName,Id")] Team team)
        {
            if (ModelState.IsValid)
            {
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(team);
        }


        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Team
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (team == null)
            {
                return RedirectToAction("Index");
            }

            return View(team);
        }


        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var team = await _context.Team.FindAsync(id);
            team.Deactivated = true;
            _context.Team.Update(team);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        // GET: TeamController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

         
    }
}
