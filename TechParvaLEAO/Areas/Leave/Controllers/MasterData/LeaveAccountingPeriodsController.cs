using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using System.ComponentModel.DataAnnotations;
using DocumentFormat.OpenXml.EMMA;
using TechParvaLEAO.Areas.Organization.Models;
using DocumentFormat.OpenXml.Wordprocessing;

namespace TechParvaLEAO.Areas.Leave.Controllers.MasterData
{
    public class LeaveUtilization
    {
        public double Planned { get; set; } = 0;
        public double Unplanned { get; set; } = 0;
        public double Total
        {
            get
            {
                return this.Planned + this.Unplanned;
            }
        }
    }

    public class CarryForwardVm
    {
        public LeaveAccountingPeriod Period { get; set; }
        public IEnumerable<CarryForwardLeavesVm> CarryForwardData { get; set; }
    }



    public class CarryForwardLeavesVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime Date { get; set; }
        [Display(Name = "Location")]
        public string Branch { get; set; }

        [Display(Name = "Annual Leaves Pro Rata")]
        public double AnnualLeaves { get; set; }
        [Display(Name = "Carry Forward From Last Year")]
        public double CarryForwardLastYear { get; set; }
        [Display(Name = "Total Leaves")]
        public double TotalLeaves { get; set; }
        [Display(Name = "Leaves Utilized")]
        public double LeavesUtilized { get; set; }
        [Display(Name = "Planned Leaves")]
        public double PlannedLeaves { get; set; }
        [Display(Name = "Unplanned Leaves")]
        public double UnplannedLeaves { get; set; }
        [Display(Name = "Leaves Pending")]
        public double LeavesBalance { get; set; }
        [Display(Name = "Carry Forward To Next Year")]
        public double CarryForward { get; set; }
        [Display(Name = "Total PL")]
        public double TotalLeavesNextYear { get; set; }
    }


    /*
     * Controller for Leave Accounting Periods Master data
     */
    [Area("Leave")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class LeaveAccountingPeriodsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LeaveAccountingPeriodsController(ApplicationDbContext context)
        {
            _context = context;
        }

        /*
         * Show list of Leave Accounting Periods
         */
        // GET: Leave/LeaveAccountingPeriods
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveAccountingPeriods.ToListAsync());
        }

        /*
         * Leave Accounting Period Details
         */
        // GET: Leave/LeaveAccountingPeriods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAccountingPeriod = await _context.LeaveAccountingPeriods
                .FirstOrDefaultAsync(m => m.Id == id);
            if (leaveAccountingPeriod == null)
            {
                return NotFound();
            }

            return View(leaveAccountingPeriod);
        }

        /*
         * Show form to creates a new Leave Accounting Period
         */
        // GET: Leave/LeaveAccountingPeriods/Create
        public IActionResult Create()
        {
            return View();
        }

        /*
         * Creates a new Leave Accounting Period from submitted form data
         */
        // POST: Leave/LeaveAccountingPeriods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate,Active,NumberOfDaysOfLeave,MaxCarryForwardFromLastYear,Deactivated,Id")] LeaveAccountingPeriod leaveAccountingPeriod)
        {
            ViewData["CarryForwardData"] = await ProcessCarryForwardLeaves(leaveAccountingPeriod.StartDate);
            return View("ConfirmActivation", leaveAccountingPeriod);
        }

        /*
         * Deactivate a Leave Accounting Period
         */
        // POST: Leave/LeaveAccountingPeriods/Delete/5
        [HttpPost, ActionName("ConfirmActivation")]
        public async Task<IActionResult> ConfirmActivation([Bind("Name,StartDate,EndDate,Active,NumberOfDaysOfLeave,MaxCarryForwardFromLastYear,Deactivated,Id")] LeaveAccountingPeriod leaveAccountingPeriod)
        {
            if (ModelState.IsValid)
            {
                leaveAccountingPeriod.Active = true;
                _context.Add(leaveAccountingPeriod);
                await _context.SaveChangesAsync();
                Dictionary<int, CarryForwardLeavesVm> cfDict = await CarryForwardDict(leaveAccountingPeriod);
                var activeEmployees = _context.Employees.Where(e => e.Status == EmployeeStatus.INSERVICE || e.Status == EmployeeStatus.RESIGNED);
                foreach(var employee in activeEmployees)
                {
                    var cf = null as CarryForwardLeavesVm;
                    cfDict.TryGetValue(employee.Id, out cf);
                    var lcu = new LeaveCreditAndUtilization
                    {
                        EmployeeId = employee.Id,
                        CarryForward = cf == null? 0: cf.CarryForward,
                        LeaveTypeId = 1,
                        NumberOfDays = leaveAccountingPeriod.NumberOfDaysOfLeave,                        
                        AddedUtilized = 1,
                        LeaveAccountingPeriod = leaveAccountingPeriod
                    };
                    _context.Add(lcu);
                }
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task<Dictionary<int, CarryForwardLeavesVm>> CarryForwardDict(LeaveAccountingPeriod leaveAccountingPeriod)
        {
            var carryForwardData = await ProcessCarryForwardLeaves(leaveAccountingPeriod.StartDate);
            Dictionary<int, CarryForwardLeavesVm> cfDict = new Dictionary<int, CarryForwardLeavesVm>();
            foreach (var data in carryForwardData)
            {
                cfDict[data.Id] = data;
            }
            return cfDict;
        }
        /*
         * Show form to edit Leave Accounting Period
         */
        // GET: Leave/LeaveAccountingPeriods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAccountingPeriod = await _context.LeaveAccountingPeriods.FindAsync(id);
            if (leaveAccountingPeriod == null)
            {
                return NotFound();
            }
            return View(leaveAccountingPeriod);
        }

        /*
         * Updates Leave Accounting Period from submitted form data
         */
        // POST: Leave/LeaveAccountingPeriods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,StartDate,EndDate,Active,NumberOfDaysOfLeave,MaxCarryForwardFromLastYear,Deactivated,Id")] LeaveAccountingPeriod leaveAccountingPeriod)
        {
            if (id != leaveAccountingPeriod.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveAccountingPeriod);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveAccountingPeriodExists(leaveAccountingPeriod.Id))
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
            return View(leaveAccountingPeriod);
        }

        /*
         * Confirmation deactivation of the Leave Accounting Period
         */
        // GET: Leave/LeaveAccountingPeriods/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveAccountingPeriod = await _context.LeaveAccountingPeriods
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (leaveAccountingPeriod == null)
            {
                return RedirectToAction("Index");
            }
            var pendingLeaves = _context.LeaveRequests.Where(l => 
                    l.LeaveAccountingPeriod == leaveAccountingPeriod &&
                    l.Status == LeaveRequestStatus.PENDING.ToString());
            LeaveAccountingPeriodDeactivateVm vm = new LeaveAccountingPeriodDeactivateVm();
            vm.LeaveAccountingPeriod = leaveAccountingPeriod;
            vm.PendingLeaves = pendingLeaves;
            return View(vm);
        }

        /*
         * Deactivate a Leave Accounting Period
         */
        // POST: Leave/LeaveAccountingPeriods/Delete/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var leaveAccountingPeriod = await _context.LeaveAccountingPeriods.FindAsync(id);
            leaveAccountingPeriod.Deactivated = true;
            _context.LeaveAccountingPeriods.Update(leaveAccountingPeriod);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveAccountingPeriodExists(int id)
        {
            return _context.LeaveAccountingPeriods.Any(e => e.Id == id);
        }

        public async Task<IEnumerable<CarryForwardLeavesVm>> ProcessCarryForwardLeaves(DateTime startDate)
        {
            var leaveYear = await _context.LeaveAccountingPeriods.Where(
                    p => p.Active ==true && 
                    p.EndDate < startDate).OrderBy(p => p.EndDate).LastOrDefaultAsync();

            if(leaveYear is null)
            {
                return Enumerable.Empty<CarryForwardLeavesVm>();
            }

            var query = _context.LeaveCreditAndUtilization.Where(c => c.LeaveTypeId == 1 
                && c.LeaveAccountingPeriod == leaveYear);

            var leaves = _context.LeaveRequests.Where(l => l.LeaveTypeId == 1 &&
                                            (l.Status == LeaveRequestStatus.PENDING.ToString() ||
                                            l.Status == LeaveRequestStatus.APPROVED.ToString())
                                            && l.LeaveAccountingPeriod == leaveYear);

            var leaveUtilization = PopulateLeaveUtilization(leaves);
            var result = query.Select(b => new CarryForwardLeavesVm
            {
                Id = b.EmployeeId,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                ReportingManager = b.Employee.ReportingTo != null? b.Employee.ReportingTo.Name:"",
                Designation = b.Employee.Designation.Name,
                Department = "",
                Branch = b.Employee.Location.Name,
                AnnualLeaves = b.NumberOfDays,
                CarryForwardLastYear = b.CarryForward,
                TotalLeaves = b.NumberOfDays + b.CarryForward,
                LeavesUtilized = GetLeaveCount(b.EmployeeId, leaveUtilization),
                LeavesBalance = Math.Max(b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),0),
                CarryForward = Math.Max(b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization) > leaveYear.MaxCarryForwardFromLastYear ? leaveYear
                        .MaxCarryForwardFromLastYear :
                        b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),0),
                TotalLeavesNextYear = Math.Max(b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization) > leaveYear.MaxCarryForwardFromLastYear ? 
                        leaveYear.MaxCarryForwardFromLastYear + leaveYear.NumberOfDaysOfLeave :
                        b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization) + leaveYear.NumberOfDaysOfLeave,0)
            });
            
            return result;
        }

        private double GetLeaveCount(int employeeId, Dictionary<int, LeaveUtilization> leaveUtilization)
        {
            LeaveUtilization utilization = new LeaveUtilization();
            if (leaveUtilization.TryGetValue(employeeId, out utilization))
            {
                return utilization.Total;
            }
            else
            {
                return 0;
            }
        }

        private Dictionary<int, LeaveUtilization> PopulateLeaveUtilization(IEnumerable<LeaveRequest> query)
        {
            var leaveUtilization = new Dictionary<int, LeaveUtilization>();
            foreach (var leave in query)
            {
                LeaveUtilization utilization = null;
                if (!leaveUtilization.TryGetValue(leave.EmployeeId.Value, out utilization))
                {
                    utilization = new LeaveUtilization();
                    leaveUtilization.Add(leave.EmployeeId.Value, utilization);
                }
                if(leave.LeaveCategoryId == 1){
                    utilization.Planned = utilization.Planned + leave.NumberOfDays;
                }
                else
                {
                    utilization.Unplanned = utilization.Unplanned + leave.NumberOfDays;
                }
            }
            return leaveUtilization;
        }
    }
}
