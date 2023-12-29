using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Handler;
using MediatR;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Expense.Services;

namespace TechParvaLEAO.Controllers
{

    public class EmployeeCSVRecord
    {
        public string Name { get; set; }
        public string DesignationName { get; set; }
        public string LocationText { get; set; }
        public string LocationCode { get; set; }
        public string AuthorizationProfileName { get; set; }
        public string ExpenseProfileName { get; set; }
        public string EmployeeCode { get; set; }
        public string AccountNumber { get; set; }
        public string ReportingToName { get; set; }
        public string ReportingToEmployeeCode { get; set; }
        public string Email { get; set;}
        public string Username { get; set; }
        public string DateOfBirth { get; set; }
        public string DateOfJoining { get; set; }
        public string BaseSalary { get; set; }
        public string BaseSalaryFromDate { get; set; }
        public string BaseSalaryToDate { get; set; }
        public string Gender { get; set; }
        public string Role1 { get; set; }
        public string Role2 { get; set; }
        public string Role3 { get; set; }
        public string Role4 { get; set; }
        public string Role5 { get; set; }
        public string Role6 { get; set; }
        public string Role7 { get; set; }
        public string Role8 { get; set; }
        public string Role9 { get; set; }
        public string Role10 { get; set; }
        public string OvertimeRule { get; set; }
        public string OVERTIME { get; set; }
        public string Mission { get; set; }
        public string SaturdayWeeklyOff { get; set; }
        //public string AdvancePendingAmount { get; set; }
        public string WeeklyOff { get; set; }
        public double CarryForwardLeaves { get; set; }
        public double LeaveEligibility { get; set; }
        public double TotalLeaves { get; set; }
    }

    public class DataImportController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediatR;
        private readonly PaymentRequestService _paymentRequestService;

        public DataImportController(ApplicationDbContext context, IMapper mapper, 
            UserManager<ApplicationUser> userManager,
            IMediator mediatR,
            PaymentRequestService paymentRequestService)
        {
            this._context = context;
            this._mapper = mapper;
            this._userManager = userManager;
            this._mediatR = mediatR;
            this._paymentRequestService = paymentRequestService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreditLeaveBalance(int number_of_days)
        {
            foreach(var employee in _context.Employees)
            {
                LeaveCreditAndUtilization leaveCredit = new LeaveCreditAndUtilization
                {
                    EmployeeId = employee.Id,
                    LeaveTypeId = 1,
                    NumberOfDays = number_of_days,
                    AddedUtilized = 1,
                    LeaveAccountingPeriodId = 1
                };
                _context.Entry(leaveCredit).State = EntityState.Added;
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> MapAdvanceExpenses()
        {
            await _paymentRequestService.MapAdvanceExpenses();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> UploadEmployeeFileAsync(IFormFile file, string operation)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            CsvReader csvReader = new CsvReader(new StreamReader(file.OpenReadStream()));
            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;

            var records = csvReader.GetRecords<EmployeeCSVRecord>().ToList<EmployeeCSVRecord>();
            await CreateDesignations(records);
            await CreateOrUpdateEmployees(records);
            await AssignReportingTo(records);
            await CreateLeaves(records);

            await CreateUserIfRequired(records);
            return RedirectToAction("Index");
        }

        private async Task CreateDesignations(IEnumerable<EmployeeCSVRecord> data)
        {
            foreach (EmployeeCSVRecord employee in data)
            {
                var designation = _context.Designations
                    .FirstOrDefault(d => string.Equals(d.Name, employee.DesignationName));
                if (designation == null)
                {
                    designation = new Designation
                    {
                        Name = employee.DesignationName
                    };
                    _context.Add(designation);
                    await _context.SaveChangesAsync();
                };
            }
        }

        private async Task AssignReportingTo(IEnumerable<EmployeeCSVRecord> data)
        {
            foreach (EmployeeCSVRecord employeeCSVRecord in data)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(x => string.Equals(x.EmployeeCode, employeeCSVRecord.EmployeeCode));
                var reportingManager = await _context.Employees
                    .FirstOrDefaultAsync(x => string.Equals(x.Name, employeeCSVRecord.ReportingToName));
                if (reportingManager != null){
                    if (employee.ReportingToId != reportingManager.Id){
                        employee.ReportingToId = reportingManager.Id;
                        _context.Entry(employee).State = EntityState.Modified;
                    }
                }
            }
            await _context.SaveChangesAsync();
        }

        private async Task CreateLeaves(IEnumerable<EmployeeCSVRecord> data)
        {
            foreach (EmployeeCSVRecord employeeCSVRecord in data)
            {
                var employee = await _context.Employees.FirstOrDefaultAsync(x => string.Equals(x.EmployeeCode, employeeCSVRecord.EmployeeCode));
                var leaveRecord = await _context.LeaveCreditAndUtilization
                    .FirstOrDefaultAsync(x => x.EmployeeId == employee.Id && x.LeaveAccountingPeriodId == 1 );
                if (leaveRecord == null)
                {
                    leaveRecord = new LeaveCreditAndUtilization
                    {
                        Employee = employee,
                        LeaveTypeId = 1,
                        AddedUtilized = 1,
                        LeaveAccountingPeriodId = 1,
                        CarryForward = employeeCSVRecord.CarryForwardLeaves,
                        NumberOfDays = employeeCSVRecord.LeaveEligibility,
                        AnnualLeaves = employeeCSVRecord.TotalLeaves
                    };
                    _context.Add(leaveRecord);
                }
            }
            await _context.SaveChangesAsync();
        }


        public async Task<IActionResult> TestReminderAsync(string operation)
        {
            if (string.Equals(operation,"LeaveReminderHandler"))
            {
                LeaveReminderViewModel vm = new LeaveReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);

            }else if (string.Equals(operation, "LeaveFinalReminderHandler"))
            {
                LeaveFinalReminderViewModel vm = new LeaveFinalReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "ExpenseFinalReminderHandler"))
            {
                ExpenseFinalReminderViewModel vm = new ExpenseFinalReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "ExpenseReminderHandler"))
            {
                ExpenseReminderViewModel vm = new ExpenseReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "AdvanceReminderHandler"))
            {
                AdvanceReminderViewModel vm = new AdvanceReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "AdvanceFinalReminderHandler"))
            {
                AdvanceFianlReminderViewModel vm = new AdvanceFianlReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "TimeSheetReminderHandler"))
            {
                TimeSheetReminderViewModel vm = new TimeSheetReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "TimeSheetFinalReminderHandler"))
            {
                TimeSheetFinalReminderViewModel vm = new TimeSheetFinalReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "AdvanceReminderMoreThanThreeDaysHandler"))
            {
                AdvanceReminderMoreThanThreeDaysViewModel vm = new AdvanceReminderMoreThanThreeDaysViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "LeaveReminderMoreThanTenDaysHandler"))
            {
                LeaveReminderViewModel vm = new LeaveReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }

            else if (string.Equals(operation, "FinanceAdvanceReminder"))
            {
                FinanceAdvanceReminderViewModel vm = new FinanceAdvanceReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "FinanceExpenseReminder"))
            {
                FinanceExpenseReminderViewModel vm = new FinanceExpenseReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "ExpenseDocumentSubmissionReminder"))
            {
                DocumentSubmissionReminderViewModel vm = new DocumentSubmissionReminderViewModel
                {
                    ForDate = DateTime.Now
                };
                await _mediatR.Send(vm);
            }
            return RedirectToAction("Index");
        }

        public IActionResult UploadBACMFileAsync(IFormFile file, string operation)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            CsvReader csvReader = new CsvReader(new StreamReader(file.OpenReadStream()));
            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;

            var records = csvReader.GetRecords<dynamic>();
            foreach (var record in records)
            {
                var mappingArray = ((ExpandoObject)record).ToArray();
                var businessActivityCode = mappingArray[0].Value;
                var businessActivityRecord = _context.BusinessActivities.Where(b => string.Equals(b.Code, businessActivityCode)).SingleOrDefault();
                if (businessActivityRecord != null)
                {
                    for (var i = 2; i < mappingArray.Length; i++)
                    {
                        var customerMarketKv = mappingArray[i];
                        var customerMarket = customerMarketKv.Key;
                        var customerMarketIsBlocked = customerMarketKv.Value;
                        if (!"Blocked".Equals(customerMarketIsBlocked))
                        {
                            var customerMarketRecord = _context.CustomerMarkets.Where(m => string.Equals(m.Code, customerMarket)).FirstOrDefault();
                            if (customerMarketRecord != null)
                            {
                                var mapping = _context.BusinessActivityCustomerMarketMapping.
                                    Where(bacm => bacm.BusinessActivity == businessActivityRecord && bacm.CustomerMarket == customerMarketRecord).
                                    FirstOrDefault();
                                if (mapping == null)
                                {
                                    var newMappingRecord = new BusinessActivityCustomerMarketMapping
                                    {
                                        BusinessActivity = businessActivityRecord,
                                        CustomerMarket = customerMarketRecord
                                    };
                                    _context.Add(newMappingRecord);
                                    _context.SaveChanges();
                                }
                            }
                        }
                    }
                }

            }
            return RedirectToAction("Index");
        }

        private async Task CreateOrUpdateEmployees(IEnumerable<EmployeeCSVRecord> data)
        {
            var allEmployees = _context.Employees.ToList();
            var employees = new Dictionary<string, Employee>();
            foreach (var emp in allEmployees)
            {
                employees.Add(emp.EmployeeCode, emp);
            }
            foreach (EmployeeCSVRecord employee in data)
            {

                //var employeeModel = _context.Employees.AsNoTracking().FirstOrDefault(x => string.Equals(x.EmployeeCode, employee.EmployeeCode));
                var employeeModel = new Employee();
                employees.TryGetValue(employee.EmployeeCode, out employeeModel);

                _mapper.Map<EmployeeCSVRecord, Employee>(employee, employeeModel);
                if (employeeModel == null)
                {
                    employeeModel = _mapper.Map<EmployeeCSVRecord, Employee>(employee);
                    if ("Rule 1".Equals(employee.OvertimeRule))
                    {
                        employeeModel.OvertimeMultiplierId = 1;
                    }
                    else if ("Rule 2".Equals(employee.OvertimeRule))
                    {
                        employeeModel.OvertimeMultiplierId = 2;
                    }
                    else
                    {
                        employeeModel.OvertimeMultiplierId = null;
                    }

                    if ("YES".Equals(employee.OVERTIME))
                    {
                        employeeModel.OnFieldEmployee = true;
                        employeeModel.SpecificWeeklyOff = true;
                    }
                    else
                    {
                        employeeModel.OnFieldEmployee = false;
                        employeeModel.SpecificWeeklyOff = false;
                    }

                    if ("YES".Equals(employee.Mission))
                    {
                        employeeModel.CanApplyMissionLeaves = true;
                    }
                    else
                    {
                        employeeModel.CanApplyMissionLeaves = false;
                    }

                    if ("CREDIT CARD".Equals(employee.Role5))
                    {
                        employeeModel.CanHoldCreditCard = true;
                    }
                    else
                    {
                        employeeModel.CanHoldCreditCard = false;
                    }

                    if ("CURRENCY".Equals(employee.Role6))
                    {
                        employeeModel.CanCreateForexRequests = true;
                    }
                    else
                    {
                        employeeModel.CanCreateForexRequests = false;
                    }

                    _context.Add(employeeModel);
                }
                else
                {
                    _mapper.Map<EmployeeCSVRecord, Employee>(employee, employeeModel);
                    if ("Rule 1".Equals(employee.OvertimeRule))
                    {
                        employeeModel.OvertimeMultiplierId = 1;
                    }
                    else if("Rule 1".Equals(employee.OvertimeRule))
                    {
                        employeeModel.OvertimeMultiplierId = 2;
                    }
                    else
                    {
                        employeeModel.OvertimeMultiplierId = null;
                    }

                    if ("YES".Equals(employee.OVERTIME))
                    {
                        employeeModel.OnFieldEmployee = true;
                        employeeModel.SpecificWeeklyOff = true;
                    }
                    else
                    {
                        employeeModel.OnFieldEmployee = false;
                        employeeModel.SpecificWeeklyOff = false;
                    }

                    if ("YES".Equals(employee.Mission))
                    {
                        employeeModel.CanApplyMissionLeaves = true;
                    }
                    else
                    {
                        employeeModel.CanApplyMissionLeaves = false;
                    }

                    if ("CREDIT CARD".Equals(employee.Role5))
                    {
                        employeeModel.CanHoldCreditCard = true;
                    }
                    else
                    {
                        employeeModel.CanHoldCreditCard = false;
                    }

                    if ("CURRENCY".Equals(employee.Role6))
                    {
                        employeeModel.CanCreateForexRequests = true;
                    }
                    else
                    {
                        employeeModel.CanCreateForexRequests = false;
                    }
                    _context.Update(employeeModel);
                }
                CreateOrUpdateRelated(employee, employeeModel);
                CreateBaseSalary(employee, employeeModel);
            }
            await _context.SaveChangesAsync();

        }

        private void CreateBaseSalary(EmployeeCSVRecord employeeCSVRecord, Employee employeeModel)
        {
            if (string.IsNullOrEmpty(employeeCSVRecord.BaseSalary)||
                        Int32.Parse(employeeCSVRecord.BaseSalary) == 0)
            {
                return;
            }
            var basesalary = _context.EmployeeBasicSalaries.FirstOrDefault(salary => salary.EmployeeId == employeeModel.Id);
            if (basesalary == null)
            {
                CultureInfo provider = CultureInfo.InvariantCulture;
                basesalary = new EmployeeBasicSalary
                {
                    Employee = employeeModel,
                    BaseSalary = string.IsNullOrEmpty(employeeCSVRecord.BaseSalary)? 0: 
                        Int32.Parse(employeeCSVRecord.BaseSalary),
                    FromDate = string.IsNullOrEmpty(employeeCSVRecord.BaseSalaryFromDate)? DateTime.Today: DateTime.ParseExact(employeeCSVRecord.BaseSalaryFromDate, "dd-mm-yyyy", provider),
                    ToDate = string.IsNullOrEmpty(employeeCSVRecord.BaseSalaryToDate) ? new DateTime(2099,12,31) : DateTime.ParseExact(employeeCSVRecord.BaseSalaryToDate, "dd-mm-yyyy", provider),
                };
                _context.Add(basesalary);
                //_context.SaveChanges();
            }
        }

        private DayOfWeek GetWeeklyOffDay(string weeklyOff)
        {
            if (string.Equals("Sunday", weeklyOff)){
                return DayOfWeek.Sunday;
            }else if (string.Equals("Monday", weeklyOff))
            {
                return DayOfWeek.Monday;
            }else if (string.Equals("Tuesday", weeklyOff))
            {
                return DayOfWeek.Tuesday;
            }
            else if (string.Equals("Wednesday", weeklyOff))
            {
                return DayOfWeek.Wednesday;
            }
            else if (string.Equals("Thursday", weeklyOff))
            {
                return DayOfWeek.Thursday;
            }
            else if (string.Equals("Friday", weeklyOff))
            {
                return DayOfWeek.Friday;
            }
            else if (string.Equals("Saturday", weeklyOff))
            {
                return DayOfWeek.Saturday;
            }
            return DayOfWeek.Sunday;
        }

        private void CreateOrUpdateRelated(EmployeeCSVRecord employeeCSVRecord, Employee employeeModel)
        {
            if (employeeModel == null)
            {
                return;
            }
            else
            {
                var authorizationProfile = 
                    _context.ApprovalLimitProfiles.AsNoTracking()
                    .FirstOrDefault(profile => 
                        profile.Approval_Limit == Int32.Parse(employeeCSVRecord.AuthorizationProfileName));
                var location =
                    _context.Locations.AsNoTracking()
                    .FirstOrDefault(l =>
                        string.Equals(l.Name, employeeCSVRecord.LocationText));
                var expenseProfile =
                    _context.ExpenseProfiles.AsNoTracking()
                    .FirstOrDefault(profile =>
                        string.Equals(profile.Name, employeeCSVRecord.ExpenseProfileName));
                var designation = _context.Designations
                    .FirstOrDefault(d => string.Equals(d.Name, employeeCSVRecord.DesignationName));
                var weeklyOff = _context.EmployeeWeeklyOff
                    .FirstOrDefault(d => d.Employee.EmployeeCode == employeeCSVRecord.EmployeeCode && string.Equals(d.WeeklyOffDay, GetWeeklyOffDay(employeeCSVRecord.WeeklyOff)));
                if (weeklyOff == null && employeeModel.SpecificWeeklyOff)
                {
                    weeklyOff = new EmployeeWeeklyOff
                    {
                        Employee = employeeModel,
                        WeeklyOffDay = GetWeeklyOffDay(employeeCSVRecord.WeeklyOff),
                        OtherWeeklyOffDay = null,
                        OtherWeeklyOffRule = null,
                        FormDate = new DateTime(2020, 1, 1),
                        ToDate = new DateTime(2099, 12, 31)
                    };
                    _context.Add(weeklyOff);
                };
                employeeModel.Designation = designation;
                if (location != null) employeeModel.LocationId = location.Id;
                if (authorizationProfile != null) employeeModel.AuthorizationProfileId = authorizationProfile.Id;
                if(expenseProfile != null) employeeModel.ExpenseProfileId = expenseProfile.Id;
                //_context.Update(employeeModel);
                //_context.SaveChanges();
            }
        }

        private async Task CreateUserIfRequired(IEnumerable<EmployeeCSVRecord> employeeCSVRecords)
        {
            var allEmployees = _context.Employees.ToList();
            var employees = new Dictionary<string, Employee>();

            foreach (var emp in allEmployees)
            {
                employees.Add(emp.EmployeeCode, emp);
            }

            var allUsers = _userManager.Users.ToList();
            var users = new Dictionary<string, ApplicationUser>();

            foreach (var user in allUsers)
            {
                users.Add(user.UserName, user);
            }

            foreach (EmployeeCSVRecord employeeCSVRecord in employeeCSVRecords)
            {
                //ApplicationUser user = await _userManager.FindByNameAsync(employeeCSVRecord.Username);
                ApplicationUser user = null;
                users.TryGetValue(employeeCSVRecord.Username, out user);
                var employee = _context.Employees.AsNoTracking().
                    FirstOrDefault(e => string.Equals(e.EmployeeCode, employeeCSVRecord.EmployeeCode));
                //var employee = new Employee();
                if (user == null && employees.TryGetValue(employeeCSVRecord.EmployeeCode, out employee))
                {
                    ApplicationUser newUser = new ApplicationUser
                    {
                        Email = employeeCSVRecord.Email,
                        UserName = employeeCSVRecord.Username,
                        EmployeeProfileId = employee.Id
                    };
                    IdentityResult result = await _userManager.CreateAsync(newUser, employee.DefaultPassword());
                    //_context.SaveChanges();

                    IEnumerable<string> roleList = GetRoleListForUser(employeeCSVRecord);
                    if (roleList.Count() > 0)
                    {
                        _userManager.AddToRolesAsync(newUser, roleList).Wait();
                    }
                }
            }
        }

        private IEnumerable<string> GetRoleListForUser(EmployeeCSVRecord employeeCSVRecord)
        {
            List<string> roleList = new List<string>();
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role1))
            {
                roleList.Add(employeeCSVRecord.Role1);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role2))
            {
                roleList.Add(employeeCSVRecord.Role2);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role3))
            {
                roleList.Add(employeeCSVRecord.Role3);
            }
            /*
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role4))
            {
                roleList.Add(employeeCSVRecord.Role4);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role5))
            {
                roleList.Add(employeeCSVRecord.Role5);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role6))
            {
                roleList.Add(employeeCSVRecord.Role6);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role7))
            {
                roleList.Add(employeeCSVRecord.Role7);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role8))
            {
                roleList.Add(employeeCSVRecord.Role8);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role9))
            {
                roleList.Add(employeeCSVRecord.Role9);
            }
            if (!string.IsNullOrEmpty(employeeCSVRecord.Role10))
            {
                roleList.Add(employeeCSVRecord.Role10);
            }
            */
            return roleList;
        }
    }
}