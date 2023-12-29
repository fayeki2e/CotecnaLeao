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
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Areas.Reports.Models;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    /*
     * Controller for Locations Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LocationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LocationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Locations
         */
        // GET: Organization/Locations
        public async Task<IActionResult> Index()
        {
            return View(await _context.Locations.ToListAsync());
        }

        /*
         * Location Details
         */
        // GET: Organization/Locations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id);
            if (location == null)
            {
                return NotFound();
            }

            return View(location);
        }

        /*
         * Show form to creates a new Location
         */
        // GET: Organization/Locations/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Location from submitted form data
         */
        // POST: Organization/Locations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Code,Deactivated,Id,MondayWorkDayType,TuesdayWorkDayType,WednesdayWorkDayType,ThursdayWorkDayType,FridayWorkDayType,SaturdayWorkDayType,SundayWorkDayType")] LocationViewModel locationVm)
        {
            if (ModelState.IsValid)
            {
                var location = new Location();
                location.Name = locationVm.Name;
                location.Code = locationVm.Code;
                location.Deactivated = locationVm.Deactivated;
                _context.Add(location);
                await _context.SaveChangesAsync();

                var mondayWd = new LocationWorkHours();
                mondayWd.WorkDayType = locationVm.MondayWorkDayType;
                mondayWd.DayOfWeek = DayOfWeek.Monday;
                mondayWd.Location = location;
                _context.Add(mondayWd);

                var tuesdayWd = new LocationWorkHours();
                tuesdayWd.WorkDayType = locationVm.TuesdayWorkDayType;
                tuesdayWd.DayOfWeek = DayOfWeek.Tuesday;
                tuesdayWd.Location = location;
                _context.Update(tuesdayWd);

                var wednesdayWd = new LocationWorkHours();
                wednesdayWd.WorkDayType = locationVm.WednesdayWorkDayType;
                wednesdayWd.DayOfWeek = DayOfWeek.Wednesday;
                wednesdayWd.Location = location;
                _context.Update(wednesdayWd);

                var thursdayWd = new LocationWorkHours();
                thursdayWd.WorkDayType = locationVm.ThursdayWorkDayType;
                thursdayWd.DayOfWeek = DayOfWeek.Thursday;
                thursdayWd.Location = location;
                _context.Update(thursdayWd);

                var fridayWd = new LocationWorkHours();
                fridayWd.WorkDayType = locationVm.FridayWorkDayType;
                fridayWd.DayOfWeek = DayOfWeek.Friday;
                fridayWd.Location = location;
                _context.Update(fridayWd);

                var saturdayWd = new LocationWorkHours();
                saturdayWd.WorkDayType = locationVm.SaturdayWorkDayType;
                saturdayWd.DayOfWeek = DayOfWeek.Saturday;
                saturdayWd.Location = location;
                _context.Update(saturdayWd);

                var sundayWd = new LocationWorkHours();
                sundayWd.WorkDayType = locationVm.SundayWorkDayType;
                sundayWd.Location = location;
                sundayWd.DayOfWeek = DayOfWeek.Sunday;
                _context.Update(sundayWd);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(locationVm);
        }

        /*
         * Show form to edit Location
         */
        // GET: Organization/Locations/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations.FindAsync(id);
            if (location == null)
            {
                return NotFound();
            }
            var locationVm = new LocationViewModel();
            locationVm.Id = location.Id;
            locationVm.Name = location.Name;
            locationVm.Code = location.Code;
            locationVm.Deactivated = location.Deactivated;
            locationVm.MondayWorkDayType= location.GetWorkday(DayOfWeek.Monday).WorkDayType;
            locationVm.TuesdayWorkDayType = location.GetWorkday(DayOfWeek.Tuesday).WorkDayType;
            locationVm.WednesdayWorkDayType = location.GetWorkday(DayOfWeek.Wednesday).WorkDayType;
            locationVm.ThursdayWorkDayType = location.GetWorkday(DayOfWeek.Thursday).WorkDayType;
            locationVm.FridayWorkDayType = location.GetWorkday(DayOfWeek.Friday).WorkDayType;
            locationVm.SaturdayWorkDayType = location.GetWorkday(DayOfWeek.Saturday).WorkDayType;
            locationVm.SundayWorkDayType = location.GetWorkday(DayOfWeek.Sunday).WorkDayType;

            return View(locationVm);
        }

        /*
         * Updates Location from submitted form data
         */
        // POST: Organization/Locations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Code,Deactivated,Id,MondayWorkDayType,TuesdayWorkDayType,WednesdayWorkDayType,ThursdayWorkDayType,FridayWorkDayType,SaturdayWorkDayType,SundayWorkDayType")] LocationViewModel locationVm)
        {
            if (id != locationVm.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var location = GetLocation(locationVm.Id);
                    location.Id = locationVm.Id;
                    location.Name = locationVm.Name;
                    location.Code = locationVm.Code;
                    location.Deactivated = locationVm.Deactivated;
                    _context.Update(location);

                    var mondayWd = location.GetWorkday(DayOfWeek.Monday);
                    mondayWd.WorkDayType = locationVm.MondayWorkDayType;
                    _context.Update(mondayWd);

                    var tuesdayWd = location.GetWorkday(DayOfWeek.Tuesday);
                    tuesdayWd.WorkDayType = locationVm.TuesdayWorkDayType;
                    _context.Update(tuesdayWd);

                    var wednesdayWd = location.GetWorkday(DayOfWeek.Wednesday);
                    wednesdayWd.WorkDayType = locationVm.WednesdayWorkDayType;
                    _context.Update(wednesdayWd);

                    var thursdayWd = location.GetWorkday(DayOfWeek.Thursday);
                    thursdayWd.WorkDayType = locationVm.ThursdayWorkDayType;
                    _context.Update(thursdayWd);

                    var fridayWd = location.GetWorkday(DayOfWeek.Friday);
                    fridayWd.WorkDayType = locationVm.FridayWorkDayType;
                    _context.Update(fridayWd);

                    var saturdayWd = location.GetWorkday(DayOfWeek.Saturday);
                    saturdayWd.WorkDayType = locationVm.SaturdayWorkDayType;
                    _context.Update(saturdayWd);

                    var sundayWd = location.GetWorkday(DayOfWeek.Sunday);
                    sundayWd.WorkDayType = locationVm.SundayWorkDayType;
                    _context.Update(sundayWd);

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LocationExists(locationVm.Id))
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
            return View(locationVm);
        }

        /*
         * Confirmation deactivation of the Location
         */
        // GET: Organization/Locations/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var location = await _context.Locations
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (location == null)
            {
                return RedirectToAction("Index");
            }

            return View(location);
        }

        /*
         * Deactivate a Location
         */
        // POST: Organization/Locations/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var location = await _context.Locations.FindAsync(id);
            location.Deactivated = true;
            _context.Locations.Update(location);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LocationExists(int id)
        {
            return _context.Locations.Any(e => e.Id == id);
        }

        private Location GetLocation(int id)
        {
            return _context.Locations.Find(id);
        }

    }
}
