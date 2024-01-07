using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;
using System.ComponentModel;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AutoMapper;
using TechParvaLEAO.Areas.Organization.Services;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.SharePoint.Client.Sharing;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Areas.Reports.Services;
using System.Data;
using TechParvaLEAO.Areas.BulkUploads.Models;

namespace TechParvaLEAO.Areas.Organization.Controllers.MasterData
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "User Id")]
        public string UserId { get; set; }

        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage =
            "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserRolesViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }

    [Area("Organization")]
    [Authorize(Roles = AuthorizationRoles.HR_MASTER)]
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly LeaveRequestServices _leaveRequestServices;
        private readonly IMapper _mapper;

  
        private readonly IEmployeeServices _employeeServices;


        public EmployeesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, LeaveRequestServices leaveRequestServices, IMapper mapper, IEmployeeServices employeeServices)
        {
            _context = context;    
            _userManager = userManager;
            _roleManager = roleManager;
            _leaveRequestServices = leaveRequestServices;
            _employeeServices = employeeServices;
            _mapper = mapper;
  
        }


        public async Task<IActionResult> Index()
        {
            //var model_list = new List<AddEditEmployeeVM>();

            //return View(model_list);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
           // var result = _context.Employees.FromSql("dbo.sp_GetAllEmployees");

              return View();

           // return result.ToList();

        }

        [HttpPost, ActionName("GetAllEmployeeList")]
        public List<EmployeeListViewModel> GetAllEmployeeList_json()
        {
            DataSet ds = new DataSet();

            // dt = (DataTable)financeReportsServices.GetBalancePayableOrReceivable_Report();

            //ds = (DataSet)EmployeeServices.GetAllEmployeeList_Report();

            ds = _employeeServices.GetAllEmployeeList_Report();

            var model_list = new List<EmployeeListViewModel>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {

                model_list.Add(new EmployeeListViewModel
                {
                    Id = Convert.ToInt32(row["Id"]),
                    name = row["Name"].ToString(),
                    employeeCode = row["employeeCode"].ToString(),
                    Designation = row["Designation"].ToString(),
                    status = row["Status"].ToString(),
                    email = row["Email"].ToString(),
                    accountNumber = row["AccountNumber"].ToString(),
                    dateOfJoining = row["DateOfJoining"].ToString(),
                    dateOfBirth = row["DateOfBirth"].ToString(),
                    expenseProfile = row["expenseProfile"].ToString(),
                    gender = row["Gender"].ToString(),
                    location = row["Location"].ToString(),
                    reportingTo = row["ReportingTo"].ToString(),
                    teamlist = row["TeamsList"].ToString(),
                    authorizationProfile = row["AuthorizationProfile"].ToString(),
                    roles = row["roles"].ToString(),
                    created_by = row["Created_By"].ToString(),
                    created_Date = row["Created_Date"].ToString(),
                    modified_by = row["Modified_By"].ToString(),
                    modified_Date = row["Modified_Date"].ToString(),
                    teams = row["Teams"].ToString(),
                    overtimerule=row["Overtimerule"].ToString(),
                    canApplyMissionLeaves=row["Can Apply Mission Leaves"].ToString(),
                    canCreateForexRequests=row["Can Create Forex Requests"].ToString(),
                    canHoldCreditCard=row["Can have Credit Card"].ToString(),
                    isHr=row["Is Hr"].ToString(),
                    onFieldEmployee=row["On Field Employee"].ToString(),
                    specificWeeklyOff=row["SpecificWeeklyOff"].ToString()

                });

            }


            return model_list.ToList();
        }


        [HttpPost, ActionName("GetAllEmployeeTemplate")]
        public ActionResult GetAllEmployeeTemplate(List<EmployeeListViewModel> emodel )
        {
            DataSet ds = new DataSet();

            // dt = (DataTable)financeReportsServices.GetBalancePayableOrReceivable_Report();

            //ds = (DataSet)EmployeeServices.GetAllEmployeeList_Report();

            ds = _employeeServices.GetAllEmployeeList_TemplateReport();

            var model_list = new List<ExcelStructure>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {

                model_list.Add(new ExcelStructure
                {
                    Row_ID = Convert.ToInt32(row["ID"]),
                    Employee = row["Name"].ToString(),
                    Employee_Code = row["Employee Code"].ToString(),
                    Designation = row["Designation"].ToString(),
                    Overtime_Rule = row["Overtime Rule"].ToString(),
                    Email = row["Email"].ToString(),
                    Account_Number = row["AccountNumber"].ToString(),
                    Date_of_Joining = row["Date Of Joining"].ToString(),
                    Date_of_Birth = row["Date Of Birth"].ToString(),
                    Expense_Profile = row["Expense Profile"].ToString(),
                    Gender = row["Gender"].ToString(),
                    Location = row["Location"].ToString(),
                    Reporting_To = row["Reporting To"].ToString(),
                    Teams = row["Teams"].ToString(),
                    Authorization_Profile = row["Authorization Profile"].ToString(),
                    Can_Apply_Mission_Leaves=row["Can Apply Mission Leaves"].ToString(),
                    Can_Create_Forex_Requests=row["Can Create Forex Requests"].ToString(),
                    Can_have_Credit_card=row["Can have Credit Card"].ToString(),
                    Is_Hr=Convert.ToBoolean (row["Is Hr"].ToString()),
                    On_Field_Employee=row["On Field Employee"].ToString(),
                    Specific_Weekly_Off=row["Specific Weekly-Off"].ToString()
                });

            }

            ViewData["Employeetemplate"] = model_list;
            return View();
        }



        [HttpPost, ActionName("GetAllEmployeeList1")]
            public List<Employee> BalancePayableOrReceivableReport_json1()
            {
            //string emp_code = User.Identity.Name;
            //var model = EmployeeServices.GetAllEmployeeList();
            ////ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;

            //return model.ToList();
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            var result = _context.Employees.FromSql("dbo.sp_GetBalancePayable_Report");

            return result.ToList();

        }


        // GET: Organization/Employees
        public async Task<IActionResult> Index1(EmployeeSearchViewModel employeeSearchViewModel)
        {
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");

            var employeeList = _context.Employees.AsQueryable<Employee>();

           

            



            if (employeeSearchViewModel.DesignationId != null)
            {
                employeeList = employeeList.Where(e => e.DesignationId == employeeSearchViewModel.DesignationId);
            }
            if (employeeSearchViewModel.EmployeeCode != null)
            {
                employeeList = employeeList.Where(e => e.EmployeeCode == employeeSearchViewModel.EmployeeCode);
            }
            if (employeeSearchViewModel.EmployeeName != null)
            {
                employeeList = employeeList.Where(e => EF.Functions.Like(e.Name, "%" + employeeSearchViewModel.EmployeeName.Trim() + "%"));
            }
            if (employeeSearchViewModel.LocationId != null)
            {
                employeeList = employeeList.Where(e => e.LocationId == employeeSearchViewModel.LocationId);
            }
            if (employeeSearchViewModel.Status != null)
            {
                employeeList = employeeList.Where(e => e.Status == employeeSearchViewModel.Status);
            }

            //if (employeeSearchViewModel.TeamId != null)
            //{
            //    employeeList = employeeList.Where(e => e.TeamId == employeeSearchViewModel.TeamId);
            //}


            var model_list = new List<AddEditEmployeeVM>();



            AddEditEmployeeVM E = new AddEditEmployeeVM();

            foreach (var emp in employeeList)
            {
                var user = await _userManager.FindByNameAsync(emp.EmployeeCode);
                var str_roles = "";

                if (user != null)
                {
                    ViewData["UserRoles"] = await _userManager.GetRolesAsync(user);
                    var l_roles = (IList<string>)ViewData["UserRoles"];
                    foreach (var role in l_roles)
                    {
                        if (l_roles.IndexOf(role) == l_roles.Count - 1)
                        {
                            str_roles = str_roles + role;
                        }
                        else
                        {

                            str_roles = str_roles + role + ";";

                        }
                    }
                }else
                {
                    str_roles = "";
                }

                var team_ids= (emp.teamlist!=null)? emp.teamlist.ToString():"";
                var str_emp_teams = "";
                if (team_ids != "")
                {
                    if (team_ids.ToString() != "0")
                    {
                        var team_id = _employeeServices.GetTeam(team_ids.ToString());


                        foreach (var t in team_id)
                        {
                            str_emp_teams = str_emp_teams + t.TeamName + ";";

                        }

                    }
                }

                model_list.Add(new AddEditEmployeeVM
                {
                    Id = emp.Id,
                    EmployeeCode = emp.EmployeeCode,
                    Name = emp.Name,
                    DesignationId = emp.DesignationId,
                    Designation = emp.Designation,
                    TeamId = emp.TeamId,
                    Teams = emp.Teams,
                    LocationId = emp.LocationId,
                    Location = emp.Location,
                    AuthorizationProfileId = emp.AuthorizationProfileId,
                    AuthorizationProfile = emp.AuthorizationProfile,
                    ExpenseProfileId = emp.ExpenseProfileId,
                    ExpenseProfile = emp.ExpenseProfile,
                    AccountNumber = emp.AccountNumber,
                    ReportingToId = emp.ReportingToId,
                    Email = emp.Email,
                    Gender = emp.Gender,
                    DateOfJoining = emp.DateOfJoining,
                    DateOfBirth = emp.DateOfBirth,
                    CanCreateForexRequests = emp.CanCreateForexRequests,
                    CanApplyMissionLeaves = emp.CanApplyMissionLeaves,
                    OvertimeMultiplierId = emp.OvertimeMultiplierId,
                    OvertimeMultiplier = emp.OvertimeMultiplier,
                    CanHoldCreditCard = emp.CanHoldCreditCard,
                    OnFieldEmployee = emp.OnFieldEmployee,
                    SpecificWeeklyOff = emp.SpecificWeeklyOff,
                    IsHr = emp.IsHr,
                    EmployeeWeeklyOffs = emp.EmployeeWeeklyOffs,
                    LastWorkingDate = emp.LastWorkingDate,
                    ResignationDate = emp.ResignationDate,
                    SettlementDate = emp.SettlementDate,
                    SettlementAmount = emp.SettlementAmount,
                    Status = emp.Status,
                    Deactivated = emp.Deactivated,
                    BasicSalaryHistory = emp.BasicSalaryHistory,
                    Created_Date = emp.Created_Date,
                    Modified_Date = emp.Modified_Date,
                    Created_by = emp.Created_by,
                    Modified_by = emp.Modified_by,
                    roles = str_roles,
                    ReportingTo = emp.ReportingTo,
                    str_teamlist = str_emp_teams





                });

              


            }


            


            return View(model_list); 

            // return View(await employeeList.ToListAsync());
        }

        // GET: Organization/Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.AuthorizationProfile)
                .Include(e => e.Designation)
                .Include(e => e.ExpenseProfile)
                .Include(e => e.Location)
                .Include(e => e.OvertimeMultiplier)
                .Include(e => e.ReportingTo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return NotFound();
            }            
            var user = await _userManager.FindByNameAsync(employee.EmployeeCode);
            ViewData["ApplicationUser"] = user;
            if(user != null)
            {
                ViewData["UserRoles"] = await _userManager.GetRolesAsync(user);
            }

            var employeeVM = _mapper.Map<AddEditEmployeeVM>(employee);

            var teamlist = employee.teamlist;

            if(string.IsNullOrEmpty(teamlist))
            {
                ViewData["teams"] = null;
            }
            else
            {
                ViewData["teams"] = _employeeServices.GetTeam(teamlist);
            }

            
            




            return View(employeeVM);
        }

        // GET: Organization/Employees/Create
        public IActionResult Create()
        {
            ViewData["AuthorizationProfileId"] = new SelectList(_context.ApprovalLimitProfiles, "Id", "Name");
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name");
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name");
            ViewData["OvertimeMultiplierId"] = new SelectList(_context.OvertimeRule, "Id", "Name");
            //ViewData["ReportingToId"] = new SelectList(_context.Employees, "Id", "DisplayName");

            //string sql_query = "";

            //// sql_query = " select * from Employees where DesignationId in (select Id from Designations where name like '%manager%')";

            //sql_query = "select E.* from Employees E inner join AspNetUsers ANU on E.EmployeeCode=ANU.UserName";
            //sql_query = sql_query + " inner join AspNetUserRoles AUR on AUR.UserId = ANU.Id";
            //sql_query = sql_query + " inner join AspNetRoles ANR on ANR.Id = AUR.RoleId";
            //sql_query = sql_query + " where AUR.RoleId = 'MANAGER'";
            //var reporting_manager = _context.Employees.FromSql(sql_query).ToList();
            //ViewData["ReportingToId"] = new SelectList(reporting_manager, "Id", "DisplayName");

            ViewData["ReportingToId"] = new SelectList(_employeeServices.GetReportingManager(), "Id", "DisplayName");

            //   var Users = _userManager.Users.ToList();



            List<Team> teamlist = new List<Team>();
            teamlist = (from Team in _context.Team select Team ).ToList();
            ViewData["ListofTeam"] = teamlist;
            return View();
        }

        // POST: Organization/Employees/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,DesignationId,LocationId,TeamList,AuthorizationProfileId,ExpenseProfileId,EmployeeCode,AccountNumber,ReportingToId,Email,Gender,DateOfJoining,DateOfBirth,CanCreateForexRequests,CanApplyMissionLeaves,OvertimeMultiplierId,CanHoldCreditCard,OnFieldEmployee,SpecificWeeklyOff,IsHr,LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,Id")] Employee employee)
        {
          //  employee.TeamId = Convert.ToInt32(Request.Form["TeamList"].ToString());

            employee.teamlist = Request.Form["TeamList"].ToString();


            employee.Created_Date= DateTime.Now;
            employee.Created_by = User.Identity.Name;


            //var employee_old = await _context.Employees
            // .Include(e => e.AuthorizationProfile)
            // .Include(e => e.Designation)
            // .Include(e => e.ExpenseProfile)
            // .Include(e => e.Location)
            // .Include(e => e.ReportingTo)
            // .FirstOrDefaultAsync(m => m.Id == employee.Id);

            if(employee.teamlist =="")
            {
                ModelState.AddModelError("teamlist", "Please select Team");
            }


            if (ModelState.IsValid)
            {                
                //if(employee_old != null)
                //{
                //    if(employee_old.Email == employee.Email)
                //    {

                //    }
                //}

                _context.Add(employee);
                await _context.SaveChangesAsync();
                var leaveAccountPeriods = _leaveRequestServices.GetAllApplicableAccountingPeriods(employee.DateOfJoining);
                var carryForwardLeavesCount = 0d;
                foreach (var leaveAccountPeriod in leaveAccountPeriods)
                {
                    var isJoiningInCurrentPeriod = employee.DateOfJoining >= leaveAccountPeriod.StartDate && employee.DateOfJoining <= leaveAccountPeriod.EndDate;
                    var proratedLeaveStartDate = employee.DateOfJoining >= leaveAccountPeriod.StartDate ? employee.DateOfJoining : leaveAccountPeriod.StartDate;
                    var proratedLeavesCount = _leaveRequestServices.GetProratedLeaves(proratedLeaveStartDate);
                    var leaveCredit = new LeaveCreditAndUtilization
                    {
                        Employee = employee,
                        LeaveTypeId = 1,
                        NumberOfDays = proratedLeavesCount,
                        AddedUtilized = 1,
                        CarryForward = carryForwardLeavesCount,
                        LeaveAccountingPeriod = leaveAccountPeriod,
                    };
                    _context.Add(leaveCredit);
                    await _context.SaveChangesAsync();

                    if (employee.DateOfJoining >= leaveAccountPeriod.StartDate)
                    {
                        carryForwardLeavesCount = Math.Min(leaveAccountPeriod.MaxCarryForwardFromLastYear, proratedLeavesCount);
                    }
                    else
                    {
                        carryForwardLeavesCount = 0;
                    }
                }

                if (employee.Email != null && !"".Equals(employee.Email))
                {
                    await _userManager.CreateAsync(new ApplicationUser
                    {
                        Email = employee.Email,
                        UserName = employee.EmployeeCode,
                        EmployeeProfileId = employee.Id
                    }, employee.DefaultPassword()); ;
                }
                // return RedirectToAction(nameof(Index));
                // 

                return RedirectToAction("Details", new { Id = employee.Id });
            }
            ViewData["AuthorizationProfileId"] = new SelectList(_context.ApprovalLimitProfiles, "Id", "Name", employee.AuthorizationProfileId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", employee.ExpenseProfileId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", employee.LocationId);
            ViewData["OvertimeMultiplierId"] = new SelectList(_context.OvertimeRule, "Id", "Name", employee.OvertimeMultiplierId);
            ViewData["ReportingToId"] = new SelectList(_context.Employees, "Id", "Name", employee.ReportingToId);

            List<Team> teamlist = new List<Team>();
            teamlist = (from Team in _context.Team select Team).ToList();
            ViewData["ListofTeam"] = teamlist;

            var employeeVM = _mapper.Map<AddEditEmployeeVM>(employee);

            return View(employeeVM);
        }

        // GET: Organization/Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = _mapper.Map<AddEditEmployeeVM>(await _context.Employees.FindAsync(id));
            if (employee == null)
            {
                return NotFound();
            }

            ViewData["AuthorizationProfileId"] = new SelectList(_context.ApprovalLimitProfiles, "Id", "Name", employee.AuthorizationProfileId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", employee.ExpenseProfileId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", employee.LocationId);
            ViewData["OvertimeMultiplierId"] = new SelectList(_context.OvertimeRule, "Id", "Name", employee.OvertimeMultiplierId);
            //ViewData["ReportingToId"] = new SelectList(_context.Employees, "Id", "Name", employee.ReportingToId);

            ViewData["ReportingToId"] = new SelectList(_employeeServices.GetReportingManager(), "Id", "Name", employee.ReportingToId);

            List<Team> teamlist = new List<Team>();
            teamlist = (from Team in _context.Team select Team).ToList();
            ViewData["ListofTeam"] = teamlist;



            var teamlists = employee.teamlist;

            if (string.IsNullOrEmpty(teamlists))
            {
                ViewData["teams"] = null;
            }
            else
            {
                List<Team> selectedteamlist = new List<Team>();

                selectedteamlist = (List<Team>)_employeeServices.GetTeam(teamlists);

                ViewData["teams"] = selectedteamlist;
            }



            return View(employee);
        }

        // POST: Organization/Employees/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,DesignationId,LocationId,AuthorizationProfileId,ExpenseProfileId,EmployeeCode,AccountNumber,ReportingToId,Email,Gender,DateOfJoining,DateOfBirth,CanCreateForexRequests,CanApplyMissionLeaves,OvertimeMultiplierId,CanHoldCreditCard,OnFieldEmployee,SpecificWeeklyOff,IsHr,LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,Id,Created_by,Created_Date")] Employee employee)
        {

            employee.teamlist = Request.Form["TeamList"].ToString();

            if (employee.teamlist == "")
            {
                ModelState.AddModelError("teamlist", "Please select Team");
            }


          //  employee.TeamId = Convert.ToInt32(Request.Form["TeamList"].ToString());

            if (id != employee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    employee.Modified_Date = DateTime.Now;
                    employee.Modified_by = User.Identity.Name;
                  //  employee.Created_by = employee.Created_by ??  User.Identity.Name ;
                  //  employee.Created_Date = employee.Created_Date ?? DateTime.Now;
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.Id))
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
            ViewData["AuthorizationProfileId"] = new SelectList(_context.ApprovalLimitProfiles, "Id", "Name", employee.AuthorizationProfileId);
            ViewData["DesignationId"] = new SelectList(_context.Designations, "Id", "Name", employee.DesignationId);
            ViewData["ExpenseProfileId"] = new SelectList(_context.ExpenseProfiles, "Id", "Name", employee.ExpenseProfileId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Name", employee.LocationId);
            ViewData["OvertimeMultiplierId"] = new SelectList(_context.OvertimeRule, "Id", "Name", employee.OvertimeMultiplierId);
            ViewData["ReportingToId"] = new SelectList(_context.Employees, "Id", "Name", employee.ReportingToId);
            var employeeVM = _mapper.Map<AddEditEmployeeVM>(employee);
            
            List<Team> teamlist = new List<Team>();
            teamlist = (from Team in _context.Team select Team).ToList();
            ViewData["ListofTeam"] = teamlist;

            return View(employeeVM);
        }

        /*
         * Change reporting manager
         */
        // GET: Organization/Employees/Create
        public IActionResult AssignReportingManager()
        {
            ViewData["OldReportingToId"] = new SelectList(_context.Employees.Where(e => e.AuthorizationProfile.Approval_Limit > 0).ToList(), "Id", "Name");
            ViewData["NewReportingToId"] = new SelectList(_context.Employees.Where(e => e.AuthorizationProfile.Approval_Limit > 0).ToList(), "Id", "Name");
            return View();
        }

        /*
         * Change reporting manager
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignReportingManager([Bind("OldReportingToId, NewReportingToId")] AssignReportingManagerViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var reportingEmployees = _context.Employees.Where(e => e.ReportingToId == vm.OldReportingToId);
                foreach (var employee in reportingEmployees)
                {
                    employee.ReportingToId = vm.NewReportingToId;
                    _context.Entry(employee).State = EntityState.Modified;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OldReportingToId"] = new SelectList(_context.Employees.Where(e => e.AuthorizationProfile.Approval_Limit > 0).ToList(), "Id", "Name");
            ViewData["NewReportingToId"] = new SelectList(_context.Employees.Where(e => e.AuthorizationProfile.Approval_Limit > 0).ToList(), "Id", "Name");
            return View(vm);

        }

        /*
        * Confirmation deactivation of the Employee
        */
        // GET: Organization/Employees/Delete/5
        public async Task<IActionResult> Deactive(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.AuthorizationProfile)
                .Include(e => e.Designation)
                .Include(e => e.ExpenseProfile)
                .Include(e => e.Location)
                .Include(e => e.ReportingTo)
                .FirstOrDefaultAsync(m => m.Id == id && m.Deactivated == false);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }

            return View(employee);
        }


       

        /*
         * Deactivate a Employee
         */
        // POST: Organization/Employees/Deactivate/5
        [HttpPost, ActionName("Deactive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactiveConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            employee.Deactivated = true;
            _context.Employees.Update(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employees
                .Include(e => e.AuthorizationProfile)
                .Include(e => e.Designation)
                .Include(e => e.ExpenseProfile)
                .Include(e => e.Location)
                .Include(e => e.ReportingTo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (employee == null)
            {
                return RedirectToAction("Index");
            }

            return View(employee);
        }


        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var employee = await _context.Employees.FindAsync(id);
            var emp_weekoff = _context.EmployeeWeeklyOff.Where(l => l.EmployeeId == employee.Id).FirstOrDefault();
            _context.EmployeeWeeklyOff.Remove(emp_weekoff);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ChangePassword(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var vm = new ChangePasswordViewModel();
            vm.UserId = user.Id;
            vm.UserName = user.NormalizedUserName;
            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateUser(int employeeId)
        {
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee.Email != null && !"".Equals(employee.Email))
            {
                await _userManager.CreateAsync(new ApplicationUser
                {
                    Email = employee.Email,
                    UserName = employee.EmployeeCode,
                    EmployeeProfileId = employee.Id
                }, employee.DefaultPassword()); ;
            }
            return RedirectToAction("Details", new { Id = employee.Id });
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.UserId);
                if (user == null)
                {
                    return RedirectToAction("Login");
                }
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

                // The new password did not meet the complexity rules or
                // the current password is incorrect. Add these errors to
                // the ModelState and rerender ChangePassword view
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View();
                }
                // Upon successfully changing the password refresh sign-in cookie
                return RedirectToAction("Details", new { Id = user.EmployeeProfileId });
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AssignRoles(string userId)
        {
            ViewData["UserId"]  = userId;
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            var model = new List<UserRolesViewModel>();
            foreach (var role in _roleManager.Roles)
            {
                var userRolesViewModel = new UserRolesViewModel
                {
                    RoleId = role.NormalizedName,
                    RoleName = role.NormalizedName
                };
                if (await _userManager.IsInRoleAsync(user, role.NormalizedName))
                {
                    userRolesViewModel.IsSelected = true;
                }
                else
                {
                    userRolesViewModel.IsSelected = false;
                }
                model.Add(userRolesViewModel);
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignRoles(List<UserRolesViewModel> model, string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {userId} cannot be found";
                return View("NotFound");
            }
            var roles = await _userManager.GetRolesAsync(user);
            
            var result = await _userManager.RemoveFromRolesAsync(user, roles);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot remove user existing roles");
                return View(model);
            }
            
            result = await _userManager.AddToRolesAsync(user,
                model.Where(x => x.IsSelected).Select(y => y.RoleId));
            if (!result.Succeeded)
            {
                ModelState.AddModelError("", "Cannot add selected roles to user");
                //return View(model);
            }
            return RedirectToAction("Details", new { Id = user.EmployeeProfileId });
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }

      

        [AcceptVerbs("GET", "POST")]
        public IActionResult VerifyDuplicateExists(int id, string employeeCode, string email)
        {
            email = String.IsNullOrEmpty(email) ? "" : email;
            var employeeAlreadyExists = _context.Employees.Where(emp => id != emp.Id && (emp.EmployeeCode == employeeCode || emp.Email == email));
            var employee_oldrecord = _context.Employees.Where(emp => id == emp.Id);


            if (employeeAlreadyExists.Count() > 0)
            {
                if (employeeAlreadyExists.Any(emp => emp.EmployeeCode == employeeCode))
                    return Json($"A user with Employee Code {employeeCode} already exists.");
              
                    
                if (employee_oldrecord.ToList()[0].Email == email)
                {
                    return Json(true);
                }
                else
                {

                    if (employeeAlreadyExists.Any(emp => !String.IsNullOrEmpty(email) && emp.Email == email))
                    {
                        return Json($"A user with Email {email} already exists.");
                    }
                }
            }
            return Json(true);
        }



    }
}
