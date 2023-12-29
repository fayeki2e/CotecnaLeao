using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Data;
using TechParvaLEAO.Authorization;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    /*
     * Controller for Employee Basic Salaries Master data
     */
    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class EmployeeBasicSalariesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeBasicSalariesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Employee Basic Salaries
         */
        // GET: Organization/EmployeeBasicSalaries
        public async Task<IActionResult> Index(EmployeeBasicSalarieSearchVM salarieSearchVM)
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name");

            var employeeBasicSalaries = _context.EmployeeBasicSalaries.AsQueryable<EmployeeBasicSalary>();

            if (salarieSearchVM.FromDate != null && salarieSearchVM.FromDate > new DateTime(2000, 01, 01))
            {
                employeeBasicSalaries = employeeBasicSalaries.Where(e => e.FromDate >= salarieSearchVM.FromDate);
            }
            if (salarieSearchVM.ToDate != null && salarieSearchVM.ToDate > new DateTime(2000, 01, 01))
            {
                employeeBasicSalaries = employeeBasicSalaries.Where(e => e.ToDate <= salarieSearchVM.ToDate);
            }
            if (salarieSearchVM.EmployeeId != null)
            {
                employeeBasicSalaries = employeeBasicSalaries.Where(e => e.EmployeeId == salarieSearchVM.EmployeeId);
            }
            if (salarieSearchVM.EmployeeCode != null)
            {
                employeeBasicSalaries = employeeBasicSalaries.Where(e => e.Employee.EmployeeCode == salarieSearchVM.EmployeeCode);
            }

            if (salarieSearchVM.BaseSalary > 0)
            {
                employeeBasicSalaries = employeeBasicSalaries.Where(e => e.BaseSalary == salarieSearchVM.BaseSalary);
            }

            return View(await employeeBasicSalaries.OrderBy(b =>b.EmployeeId).ToListAsync());
        }

        /*
         * Employee Basic Salaries Details
         */
        // GET: Organization/EmployeeBasicSalaries/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeBasicSalary = await _context.EmployeeBasicSalaries
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employeeBasicSalary == null)
            {
                return NotFound();
            }

            return View(employeeBasicSalary);
        }

        /*
         * Show form to creates a new Employee Basic Salaries
         */
        // GET: Organization/EmployeeBasicSalaries/Create
        [HttpGet]
        public IActionResult Create(int? EmployeeId)
        {
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", EmployeeId);
            ViewData["SelectedEmployeeId"] = EmployeeId;
            var employee = new EmployeeBasicSalary();
            employee.FromDate = DateTime.Today;
            employee.ToDate = new DateTime(2099, 12, 31);
            return View(employee);
        }

        /*
         * Creates a new Employee Basic Salaries from submitted form data
         */
        // POST: Organization/EmployeeBasicSalaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,FromDate,ToDate,BaseSalary,Deactivated")] EmployeeBasicSalary employeeBasicSalary)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employeeBasicSalary);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Employees", new { Id = employeeBasicSalary.EmployeeId });
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeBasicSalary.EmployeeId);
            return View(employeeBasicSalary);
        }

        /*
         * Show form to edit Employee Basic Salaries
         */
        // GET: Organization/EmployeeBasicSalaries/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeBasicSalary = await _context.EmployeeBasicSalaries.FindAsync(id);
            if (employeeBasicSalary == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeBasicSalary.EmployeeId);
            return View(employeeBasicSalary);
        }

        /*
         * Updates Employee Basic Salaries from submitted form data
         */
        // POST: Organization/EmployeeBasicSalaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeId,FromDate,ToDate,BaseSalary,Deactivated,Id")] EmployeeBasicSalary employeeBasicSalary)
        {
            if (id != employeeBasicSalary.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employeeBasicSalary);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeBasicSalaryExists(employeeBasicSalary.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Employees", new { Id = employeeBasicSalary.EmployeeId });
            }
            ViewData["EmployeeId"] = new SelectList(_context.Employees, "Id", "Name", employeeBasicSalary.EmployeeId);
            return View(employeeBasicSalary);
        }

        /*
         * Confirmation deactivation of the Employee Basic Salaries
         */
        // GET: Organization/EmployeeBasicSalaries/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employeeBasicSalary = await _context.EmployeeBasicSalaries
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (employeeBasicSalary == null)
            {
                return RedirectToAction("Index");
            }

            return View(employeeBasicSalary);
        }

        /*
         * Deactivate a Employee Basic Salaries
         */
        // POST: Organization/EmployeeBasicSalaries/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var employeeBasicSalary = await _context.EmployeeBasicSalaries.FindAsync(id);
            employeeBasicSalary.Deactivated = true;
            _context.EmployeeBasicSalaries.Update(employeeBasicSalary);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [HttpGet]
        public IActionResult UploadBasicSalariesFile()
        {
            return View("UploadBasicSalariesFile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadBasicSalariesFile(IFormFile file, string operation)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            CsvReader csvReader = new CsvReader(new StreamReader(file.OpenReadStream()));
            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;
            var options = new TypeConverterOptions { Formats = new[] { "dd/MM/yyyy", "dd-MM-yyyy", "yyyy-MM-dd" } };
            csvReader.Configuration.TypeConverterOptionsCache.AddOptions<DateTime>(options);

            var records = csvReader.GetRecords<BasicSalaryVm>().ToList();
            var result = new List<BasicSalaryDataImportResult>();
            foreach (var record in records)
            {
                var toDate = new DateTime(2099, 1, 1);
                if (record.ToDate.HasValue)
                {
                    toDate = record.ToDate.Value;
                }
                else
                {
                    record.ToDate = toDate;
                }
                var procResult = new BasicSalaryDataImportResult(record);
                var previous = _context.EmployeeWeeklyOff.Where(o =>
                    o.Employee.EmployeeCode == record.EmployeeCode && o.ToDate == toDate);
                foreach (var prevRecord in previous)
                {
                    prevRecord.ToDate = record.FromDate - TimeSpan.FromDays(1);
                    _context.Entry(prevRecord).State = EntityState.Modified;
                }
                var employee = await _context.Employees.Where(
                        e => e.EmployeeCode == record.EmployeeCode).FirstOrDefaultAsync();
                if (employee != null)
                {
                    var employeeSalary = new EmployeeBasicSalary
                    {
                        EmployeeId = employee.Id,
                        FromDate = record.FromDate,
                        ToDate = toDate,
                        BaseSalary = record.Salary
                    };
                    _context.Add(employeeSalary);
                    procResult.Result = "SUCCESS";
                }
                else
                {
                    procResult.Result = "NOT FOUND";
                }
                result.Add(procResult);
            }
            await _context.SaveChangesAsync();
            return View("UploadBasicSalariesFileResult", result);
        }

        private bool EmployeeBasicSalaryExists(int id)
        {
            return _context.EmployeeBasicSalaries.Any(e => e.Id == id);
        }
    }
}
