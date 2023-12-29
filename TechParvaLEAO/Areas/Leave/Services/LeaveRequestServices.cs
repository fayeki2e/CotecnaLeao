using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Organization.Services;
using System.Collections;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Newtonsoft.Json;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.Leave.Services
{
    public class LeaveRequestServices
    {
        private readonly IApplicationRepository context;
        private readonly LocationWorkdaysService locationWorkdaysService;
        private readonly ApplicationDbContext dbContext;
        private readonly IEmployeeServices employeeServices;
        private int MaxShowDaysHR = 30;

        public LeaveRequestServices(IApplicationRepository context,
            LocationWorkdaysService locationWorkdaysService,
            ApplicationDbContext dbContext,
            IEmployeeServices employeeServices)
        {
            this.context = context;
            this.locationWorkdaysService = locationWorkdaysService;
            this.dbContext = dbContext;
            this.employeeServices = employeeServices;
        }

        private IEnumerable<LeaveRequest> ApplySearch(IEnumerable<LeaveRequest> query, LeaveRequestSearchViewModel searchModel)
        {
            if (searchModel == null) return query;
            if (searchModel.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }
            if (searchModel.EmployeeName != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.Name, "%"+searchModel.EmployeeName+"%"));
            }
            if (searchModel.EmployeeCode != null)
            {
                query = query.Where(p => EF.Functions.Like(p.Employee.EmployeeCode, searchModel.EmployeeCode));
            }
            if (searchModel.ApprovedBy != null)
            {
                query = query.Where(p => EF.Functions.Like(p.LeaveRequestApprovedBy.Name, searchModel.ApprovedBy));
            }
            if (searchModel.FromDate != null && searchModel.FromDate > new DateTime(2000, 01, 01))
            {
                //var fromDate = searchModel.FromDate - TimeSpan.FromDays(1) + -TimeSpan.FromDays(1);
                query = query.Where(p => p.LeaveRequestCreatedDate.Date >= searchModel.FromDate.Value);
            }
            if (searchModel.ToDate != null && searchModel.ToDate > new DateTime(2000, 01, 01))
            {
                query = query.Where(p => p.LeaveRequestCreatedDate.Date <= searchModel.ToDate);
            }
            if (!string.IsNullOrEmpty(searchModel.Status))
            {
                query = query.Where(p => p.Status == searchModel.Status);
            }
            return query;

        }

        public async Task<IEnumerable<LeaveRequest>> GetOwnLeaves(int employeeId, LeaveRequestSearchViewModel searchModel)
        {
            return await context.GetAsync<LeaveRequest>(t => t.EmployeeId == employeeId &&
                t.Status == "APPROVED");

        }
        public IEnumerable<LeaveRequest> GetOwnLeaves(Employee employee, LeaveRequestSearchViewModel searchModel)
        {
            var query = context.Get<LeaveRequest>(t => t.EmployeeId == employee.Id, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }

        public IEnumerable<LeaveDraft> GetDraftLeaves(String user)
        {
            var query = context.Get<LeaveDraft>(t => t.UserIdentity == user, q => q.OrderByDescending(s => s.LastUpdatedOn));
            return query;
        }

        public void DeleteDraft(String user, int id)
        {
            var record = context.Get<LeaveDraft>(t => t.UserIdentity == user && t.Id == id).FirstOrDefault();
            if (record != null)
            {
                context.Delete<LeaveDraft>(record.Id);
                context.Save();
            }
        }

        public IEnumerable<LeaveRequest> GetOnBehalfLeaves(Employee employee, LeaveRequestSearchViewModel searchModel)
        {
            var query = context.Get<LeaveRequest>(t => t.CreatedByEmployeeId == employee.Id && t.Employee.Id != employee.Id, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }

        public IEnumerable<LeaveRequest> GetHRDashboardLeaves(Employee employee, LeaveRequestSearchViewModel searchModel)
        {
            var query = context.Get<LeaveRequest>(t => t.Status == LeaveRequestStatus.PENDING.ToString() ||
                ((t.Status == LeaveRequestStatus.CANCELED.ToString() || 
                    t.Status == LeaveRequestStatus.REJECTED.ToString())
                    && t.EndDate > DateTime.Today.AddDays(-MaxShowDaysHR))
            , q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
            query = ApplySearch(query, searchModel);
            return query;
        }

        public IEnumerable<LeaveCreditAndUtilization> GetAvailableCompOffs(Employee employee)
        {
            return context.Get<LeaveCreditAndUtilization>(t =>
                t.EmployeeId == employee.Id && t.LeaveTypeId == 2 && t.AddedUtilized == 1
                && t.ExpiryDate >= DateTime.Now,
                q => q.OrderByDescending(s => s.AccrualDate));
        }

        public double GetAvailableCompOffsCount(Employee employee)
        {
            var compOffs = GetAvailableCompOffs(employee);
            var compOffsCount = 0.0d;
            //In case employee has availed half day comp off (carry forward stores the value of carry forwarded leaves i.e. 0.5
            foreach (var compoff in compOffs)
            {
                if (compoff.CarryForward == 0.5)
                {
                    compOffsCount = compOffsCount + compoff.CarryForward;
                }
                else
                {
                    compOffsCount += 1;
                }
            }
            return compOffsCount;
        }

        //Dashboard
        public IEnumerable<LeaveRequest> GetForMyApprovalLeaves(Employee employee, LeaveRequestSearchViewModel searchModel)
        {
            var query = context.Get<LeaveRequest>(t => t.LeaveRequestApprovedById == employee.Id, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate)); 
            query = ApplySearch(query, searchModel);    
            return query;
        }

        public IEnumerable<LeaveRequest> GetForHrLeavesPending(Employee employee)
        {
            return context.Get<LeaveRequest>(t => t.Status == LeaveRequestStatus.PENDING.ToString(), q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
        }

        public IEnumerable<LeaveRequest> GetForReportingToLeavesPending(Employee employee)
        {
            return context.Get<LeaveRequest>(t => t.Status == LeaveRequestStatus.PENDING.ToString() && t.Employee.ReportingTo.Id == employee.Id, 
                q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
        }

        public IEnumerable<LeaveRequest> GetOwnEmployeeLeavesPending(Employee employee)
        {
            return context.Get<LeaveRequest>(t => t.Status == LeaveRequestStatus.PENDING.ToString() && t.Employee.Id == employee.Id, 
                q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));
        }

        public LeaveAccountingPeriod GetAccountingPeriod(DateTime startDate)
        {
            var leavePeriods = context.Get<LeaveAccountingPeriod>(p => p.StartDate <= startDate
                    && p.EndDate >= startDate);
            if (leavePeriods.Count() > 0)
            {
                return leavePeriods.Last();
            }
            return null;
        }
        public IEnumerable<LeaveAccountingPeriod> GetAllApplicableAccountingPeriods(DateTime startDate)
        {
            var leavePeriods = context.Get<LeaveAccountingPeriod>(p => p.EndDate >= startDate);            
            return leavePeriods;
        }

        public double GetProratedLeaves(DateTime forDate)
        {
            var accountingPeriod = GetAccountingPeriod(forDate);
            if (accountingPeriod == null) return 0;
            var annualLeaveCredit = accountingPeriod.NumberOfDaysOfLeave;
            var proratedLeaveCredit = annualLeaveCredit - Math.Round((forDate.DayOfYear* annualLeaveCredit) / 365.00f);
            return proratedLeaveCredit;
        }

        public double GetAnnaulLeaveEligibility(Employee employee, DateTime? forDate = null)
        {
            if (!forDate.HasValue) forDate = DateTime.Today;
            var leaveAccountingPeriod = GetAccountingPeriod(forDate.Value);
            if (leaveAccountingPeriod == null) return 0;
            var leaveEligibility = context.Get<LeaveCreditAndUtilization>(
                c => c.LeaveAccountingPeriodId == leaveAccountingPeriod.Id &&
                c.EmployeeId == employee.Id &&
                c.LeaveTypeId == 1 &&
                c.AddedUtilized == 1).Select(s => s.NumberOfDays).Sum();
            return leaveEligibility;
        }

        public double GetAnnaulLeavesCarryForward(Employee employee, DateTime? forDate = null)
        {
            if (!forDate.HasValue) forDate = DateTime.Today;
            var leaveAccountingPeriod = GetAccountingPeriod(forDate.Value);
            if (leaveAccountingPeriod == null) return 0;
            var leavesCarryForward = context.Get<LeaveCreditAndUtilization>(
                c => c.LeaveAccountingPeriodId == leaveAccountingPeriod.Id &&
                c.EmployeeId == employee.Id &&
                c.LeaveTypeId == 1 &&
                c.AddedUtilized == 1).Select(s => s.CarryForward).Sum();
            return leavesCarryForward;
        }
        public double GetAnnaulLeaveUtilized(Employee employee, DateTime? forDate = null)
        {
            if (!forDate.HasValue) forDate = DateTime.Today;
            var leaveAccountingPeriod = GetAccountingPeriod(forDate.Value);
            if (leaveAccountingPeriod == null) return 0;
            var leaveEligibility = context.Get<LeaveRequest>(
                l => l.LeaveTypeId == 1 &&
                l.EmployeeId == employee.Id &&
                l.LeaveAccountingPeriodId == leaveAccountingPeriod.Id &&
                l.Status == LeaveRequestStatus.APPROVED.ToString()).Select(s => s.NumberOfDays).Sum();
            return leaveEligibility;
        }

        public double GetAnnaulLeavePending(Employee employee, DateTime? forDate = null)
        {
            if (!forDate.HasValue) forDate = DateTime.Today;
            var leaveAccountingPeriod = GetAccountingPeriod(forDate.Value);
            if (leaveAccountingPeriod == null) return 0;
            var leaveEligibility = context.Get<LeaveRequest>(
                l => l.LeaveTypeId == 1 &&
                l.EmployeeId == employee.Id &&
                l.LeaveAccountingPeriodId == leaveAccountingPeriod.Id &&
                l.Status == LeaveRequestStatus.PENDING.ToString()).Select(s => s.NumberOfDays).Sum();
       
         DateTime? previousdate_year = DateTime.Today.AddDays(-365);
              leaveAccountingPeriod = GetAccountingPeriod(previousdate_year.Value);
          
            var leaveEligibility1 = context.Get<LeaveRequest>(
                       l => l.LeaveTypeId == 1 &&
                       l.EmployeeId == employee.Id &&
                       l.LeaveAccountingPeriodId == leaveAccountingPeriod.Id &&
                       l.Status == LeaveRequestStatus.PENDING.ToString()).Select(s => s.NumberOfDays).Sum();




            return leaveEligibility+ leaveEligibility1;
        }

        public double GetLWPDays(Employee employee, DateTime? forDate = null)
        {
            if (!forDate.HasValue) forDate = DateTime.Today;
            var leaveAccountingPeriod = GetAccountingPeriod(forDate.Value);
            if (leaveAccountingPeriod == null) return 0;
            var leaveEligibility = context.Get<LeaveRequest>(
                l => l.EmployeeId == employee.Id &&
                l.LeaveTypeId == 3 &&
                l.LeaveAccountingPeriodId == leaveAccountingPeriod.Id&&
                l.Status == LeaveRequestStatus.APPROVED.ToString()).Select(s => s.NumberOfDays).Sum();
            return leaveEligibility;
        }

        public double GetBusinessDays(Employee employee, DateTime startDate, DateTime endDate, bool halfDayStart, bool halfDayEnd)
        {
            IList<LocationCalendar> calendar = locationWorkdaysService.GetLocationCalendar(employee.LocationId.Value, startDate, endDate);
            var offDays = 0;
            var halfDays = 0.0;
            foreach (var day in calendar)
            {
                if (day.IsHoliday || day.IsWeekOff)
                {
                    offDays += 1;
                    continue;
                }
                if (day.IsHalfDay)
                {
                    halfDays = 0.5;

                }
            }
            return (endDate - startDate).TotalDays + 1 - offDays - halfDays;
        }

        public double GetNumberOfLeaveDays(Employee employee, DateTime startDate, DateTime endDate, bool halfDayStart, bool halfDayEnd, int leaveTypeId)
        {
            IList<LocationCalendar> calendar;
            if (employee.SpecificWeeklyOff)
            {
                calendar = locationWorkdaysService.GetEmployeeCalendar(employee.LocationId.Value, employee.Id, startDate, endDate);
            }
            else
            {
                calendar = locationWorkdaysService.GetLocationCalendar(employee.LocationId.Value, startDate, endDate);
            }
            var offDays = 0;
            var halfDays = 0.0;
            foreach (var day in calendar)
            {
                if ((day.IsHoliday || day.IsWeekOff) && leaveTypeId!=3)
                {
                    offDays += 1;
                    continue;
                }
                if (day.IsHalfDay && leaveTypeId != 3)
                {
                    halfDays += 0.5;
                }
            }
            if (halfDayStart)
            {
                halfDays += 0.5;
            }
            if (halfDayEnd)
            {
                halfDays += 0.5;
            }
            return (endDate - startDate).TotalDays + 1 - offDays - halfDays;
        }

        public bool LeaveExists(int employeeId, DateTime forDate)
        {
            return context.Get<LeaveRequest>(l => l.EmployeeId == employeeId &&
                l.Status == LeaveRequestStatus.APPROVED.ToString() &&
                l.LeaveTypeId != 5 &&
                l.StartDate <= forDate & l.EndDate >= forDate).Count() > 0;
        }

        public async Task UpdateEmployeeLeaves(IList<LocationCalendar> calendar, int employeeId, DateTime startDate, DateTime endDate)
        {

            var leaves = await GetOwnLeaves(employeeId,
                new TechParvaLEAO.Areas.Leave.Models.ViewModels.LeaveRequestSearchViewModel
                { FromDate = startDate, ToDate = endDate, Status = "APPROVED" });
            foreach (var leave in leaves)
            {
                for (var date = leave.StartDate; date <= leave.EndDate; date = date.AddDays(1))
                {
                    var isHalfDay = false;
                    if (date == leave.StartDate && leave.HalfDayStart) isHalfDay = true;
                    if (date == leave.EndDate && leave.HalfDayEnd) isHalfDay = true;
                    UpdateLeaveInCalendar(calendar, date, isHalfDay);
                }
            }
        }

        private void UpdateLeaveInCalendar(IList<LocationCalendar> calendar, DateTime date, bool isHalfday)
        {
            foreach (var day in calendar)
            {
                if (day.Date.Equals(date))
                {
                    day.IsLeave = true;
                    day.IsHalfDayLeave = isHalfday;
                }
            }
        }

        public NewLeaveViewModel GetLeaveDraft(ClaimsPrincipal user, int id)
        {
            var leave_draft = context.Get<LeaveDraft>(d => d.UserIdentity == user.Identity.Name && d.Id == id).FirstOrDefault();
            return JsonConvert.DeserializeObject<NewLeaveViewModel>(leave_draft.FormData);
        }

        public async Task<List<LeaveType>> GetLeaveTypesForEmployee(int employeeId)
        {
            var employee = await dbContext.Employees.FirstOrDefaultAsync(e => e.Id == employeeId);
            var result = await dbContext.LeaveTypes.Where(t => t.IsCommon == true).ToListAsync();

            if (employee != null && employee.CanCreateMaternityLeaves())
            {
                result.AddRange(await dbContext.LeaveTypes.Where(t => t.Deactivated == false && t.IsMaternity == true).ToListAsync());
            }
            if (employee != null && employee.CanApplyMissionLeaves)
            {
                result.AddRange(await dbContext.LeaveTypes.Where(t => t.Deactivated == false && t.IsMission == true).ToListAsync());
            }
            if (employee != null && employee.OnFieldEmployee)
            {
                result.AddRange(await dbContext.LeaveTypes.Where(t => t.Deactivated == false && t.Id == 2).ToListAsync());
            }
            return result;
        }

        public async Task<List<DropDownVO>> GetLeaveSubCategories(int leaveCategory, int leaveType)
        {
            var result = new List<DropDownVO>();
            var mappings = await dbContext.LeaveTypeLeaveCategoryLeaveSubCategoryMapping
                .Where(c => c.Deactivated == false && c.LeaveCategoryId == leaveCategory && c.LeaveTypeId == leaveType).ToListAsync();

            foreach (var mapping in mappings)
            {
                if (!mapping.Deactivated)
                {
                    var vo = new DropDownVO
                    {
                        Id = mapping.LeaveSubCategory.Id,
                        Text = mapping.LeaveSubCategory.Text
                    };
                    result.Add(vo);
                }
            }
            return result;
        }

        public async Task<List<DropDownVO>> GetLeaveCategories(int leaveType)
        {
            var result = new List<DropDownVO>();
            var resultObj = new List<LeaveCategory>();
            var mappings = await dbContext.LeaveTypeLeaveCategoryLeaveSubCategoryMapping
                .Where(c => c.Deactivated == false && c.LeaveTypeId == leaveType).ToListAsync();

            foreach (var mapping in mappings)
            {
                if (mapping.LeaveCategory.Deactivated == false)
                {
                    var vo = new DropDownVO
                    {
                        Id = mapping.LeaveCategory.Id,
                        Text = mapping.LeaveCategory.Text
                    };

                    if (!resultObj.Contains(mapping.LeaveCategory))
                    {
                        result.Add(vo);
                        resultObj.Add(mapping.LeaveCategory);
                    }
                }
            }
            return result;
        }

        
        public IEnumerable<LeaveRequest> Get_Leave(int leave_request_id)
        {
            var query = context.Get<LeaveRequest>(u => u.Id == leave_request_id).FirstOrDefault();
            yield return query;
        }

        public IEnumerable<Employee> Get_TeamMember_list(int leave_request_id )
        {
            var emp  = dbContext.LeaveRequests.Where(u => u.Id == leave_request_id).FirstOrDefault();
             

       // var employee_teamid = dbContext.Employees.Where(u => u.Id == emp.EmployeeId).Select(u => u.TeamId).ToList()[0];

        var employee_teamid = dbContext.Employees.Where(u => u.Id == emp.EmployeeId).Select(u => u.teamlist).ToList()[0];


            if (employee_teamid == null)
            {
                return Enumerable.Empty<Employee>();
            }

            
            // var team_members = dbContext.Employees.Where(u => u.TeamId == employee_teamid).Select(u => u.TeamId);

            string sql_query1 = "";   

          //  var team_members = dbContext.Employees.FromSql(sql_query1).ToList();

            string[] e_teamids = employee_teamid.Split(",");
            string sql_query = "";

            string str_employee_in_query = "(";

            for (int j = 0; j < e_teamids.Length; j++)
            {
                if (j != 0)
                {
                    str_employee_in_query = str_employee_in_query + ",";
                }
               // sql_query = "select * from Employees where teamlist like '%" + e_teamids[j] + "%'";

                sql_query = "SELECT * FROM Employees  WHERE (',' + RTRIM(teamlist) + ',') LIKE '%," + e_teamids[j] + ",%' and Deactivated=0";




                var employee_current_team = dbContext.Employees.FromSql(sql_query).ToList();
                for (int i = 0; i < employee_current_team.Count; i++)
                {

                    if (i == employee_current_team.Count - 1)
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id;
                    }
                    else
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id + ",";
                    }
                }
            }

            if (e_teamids.Length == 0)
            {
                str_employee_in_query = str_employee_in_query + 0;
            }


            str_employee_in_query = str_employee_in_query + ")";

            sql_query1 = "Select * from Employees where id in " + str_employee_in_query;

            var team_members = dbContext.Employees.FromSql(sql_query1).ToList();
            return team_members;

             

        }

            public IEnumerable<LeaveRequest> LeavesOfTeamMember(ClaimsPrincipal user,string sdate)
        {
           // var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.TeamId).ToList()[0];

            var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.teamlist).ToList()[0];

            if (current_employee_teamid == null)
             {
                return Enumerable.Empty<LeaveRequest>();
            }


            string[] e_teamids = current_employee_teamid.Split(",");
            string sql_query = "";

            string str_employee_in_query = "(";

            for (int j = 0; j < e_teamids.Length; j++)
            {
                if (j != 0)
                {
                    str_employee_in_query = str_employee_in_query + ",";
                }
                //   sql_query = "select * from Employees where teamlist like '%" + e_teamids[j] + "%'";

                sql_query = "SELECT * FROM Employees  WHERE (',' + RTRIM(teamlist) + ',') LIKE '%," + e_teamids[j] + ",%'  and Deactivated=0 ";


                var employee_current_team = dbContext.Employees.FromSql(sql_query).ToList();
                for (int i = 0; i < employee_current_team.Count; i++)
                {

                    if (i == employee_current_team.Count - 1)
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id;
                    }
                    else
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id + ",";
                    }
                }
            }

            if (e_teamids.Length == 0)
            {
                str_employee_in_query = str_employee_in_query + 0;
            }


            str_employee_in_query = str_employee_in_query + ")";












           // var employee_current_team = dbContext.Employees.Where(u => u.TeamId == current_employee_teamid).Select(u => u.Id).ToList();
            var query = context.Get<LeaveRequest>(t => t.EmployeeId == t.EmployeeId, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));

            //string str_employee_in_query = "(";


            //for(int i=0;i< employee_current_team.Count;i++)
            //{

            //    if(i== employee_current_team.Count-1)
            //    {
            //        str_employee_in_query = str_employee_in_query+ employee_current_team[i];
            //    }
            //    else
            //    {
            //        str_employee_in_query = str_employee_in_query + employee_current_team[i] + ",";
            //    }
            //}

            //str_employee_in_query = str_employee_in_query + ")";


            // query = query.Where(p => EF.Functions.Contains(p.Employee.Id, "%test%")).ToList();

           // string sql_query = "";

            if(sdate =="")
            {
                
                sql_query  = "select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query; 
            }
            else
            {
                sql_query = " select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query + " and  CONVERT(varchar, StartDate, 23) = '" + sdate.ToString() +"'";


            }

            var leaves = dbContext.LeaveRequests.FromSql(sql_query).ToList();
            return leaves;

        }

        public IEnumerable<NewLeaveViewModel> get_LeavesrecordOfEmployee(int id)
        {
            //  var objLeaves = null as List<NewLeaveViewModel>;

            var objLeaves = new List<NewLeaveViewModel>();

            //objLeaves = dbContext.LeaveRequests
            //          .Where(p => p.Id == id).ToList();


            var   obj  = (from lr in dbContext.LeaveRequests
                          join e in dbContext.Employees on lr.EmployeeId equals e.Id                       
                              where lr.Id == id
                              select new
                              {
                                  EmployeeCode = e.EmployeeCode,
                                  StartDate =lr.StartDate,
                                  EndDate= lr.EndDate,
                                  EmployeeName = e.Name,
                                  Email = e.Email,
                                  teamlist = e.teamlist
                              }).Take(1);


            var vm = new NewLeaveViewModel();
            vm.EmployeeCode = obj.ToList()[0].EmployeeCode;
            vm.EmployeeName = obj.ToList()[0].EmployeeName;
            vm.StartDate = obj.ToList()[0].StartDate;
            vm.EndDate = obj.ToList()[0].EndDate;
            vm.Email = obj.ToList()[0].Email;
            vm.teamlist=obj.ToList()[0].teamlist;
            
            objLeaves.Add(vm);
            return objLeaves;
        }


        public IEnumerable<EmailNotificationConfiguration> get_NoticationConfiguration(NotificationEventModel notification )
        {
            var configuration = dbContext.EmailNotificationConfiguration.
                         Where(n => n.Type == notification.Type).
                         Where(n => n.Name == notification.Event).ToList();


            return configuration;


        }


        public IEnumerable<LeaveRequest> Check_LeavesOfTeam(ClaimsPrincipal user, string sdate, DateTime fromdate, DateTime todate)
        {
          
          //  var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.TeamId).ToList()[0];
            var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.teamlist).ToList()[0];


            if (current_employee_teamid == null)
            {
             
                return Enumerable.Empty<LeaveRequest>();

            }

            


            string[] e_teamids = current_employee_teamid.Split(",");
            string sql_query = "";

            string str_employee_in_query = "(";

            for (int j = 0; j < e_teamids.Length; j++)
            {
                if (j != 0)
                {
                    str_employee_in_query = str_employee_in_query + ",";
                }
                //   sql_query = "select * from Employees where teamlist like '%" + e_teamids[j] + "%'";


                sql_query = "SELECT * FROM Employees  WHERE (',' + RTRIM(teamlist) + ',') LIKE '%," + e_teamids[j] + ",%'  and Deactivated=0";

                var employee_current_team = dbContext.Employees.FromSql(sql_query).ToList();
                for (int i = 0; i < employee_current_team.Count; i++)
                {

                    if (i == employee_current_team.Count - 1)
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id;
                    }
                    else
                    {
                        str_employee_in_query = str_employee_in_query + employee_current_team[i].Id + ",";
                    }
                }
            }




            if (e_teamids.Length == 0)
            {
                str_employee_in_query = str_employee_in_query + 0;
            }

            string sql_query1 = "";

            str_employee_in_query = str_employee_in_query + ")";

           // sql_query1 = "Select * from Employees where id in " + str_empid_in_query;

           // var employee = dbContext.Employees.FromSql(sql_query1).ToList();




         //   var employee_current_team = dbContext.Employees.Where(u => u.TeamId == current_employee_teamid).Select(u => u.Id).ToList();
           // var query = context.Get<LeaveRequest>(t => t.EmployeeId == t.EmployeeId, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));

            //string str_employee_in_query = "(";


            ////for (int i = 0; i < employee_current_team.Count; i++)
            ////{

            ////    if (i == employee_current_team.Count - 1)
            ////    {
            ////        str_employee_in_query = str_employee_in_query + employee_current_team[i];
            ////    }
            ////    else
            ////    {
            ////        str_employee_in_query = str_employee_in_query + employee_current_team[i] + ",";
            ////    }
            ////}

            //if (employee_current_team.Count == 0)
            //{
            //    str_employee_in_query = str_employee_in_query + 0;
            //}


            //str_employee_in_query = str_employee_in_query + ")";


            // query = query.Where(p => EF.Functions.Contains(p.Employee.Id, "%test%")).ToList();

         

            if (sdate == "")
            {

                // sql_query = "select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query + " and StartDate between Convert(datetime,'" + fromdate.ToString("yyyy/MM/dd 00:00:00.000")+ "') and Convert(datetime,'" + fromdate.ToString("yyyy/MM/dd 00:00:00.000") + "')  or EndDate between Convert(datetime,'" + fromdate.ToString("yyyy/MM/dd 00:00:00.000") + "') and Convert(datetime,'" + fromdate.ToString("yyyy/MM/dd 00:00:00.000") + "') ";

                sql_query = "select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query + " and StartDate <= Convert(datetime,'" + fromdate.ToString("yyyy/MM/dd 00:00:00.000") + "')   and EndDate >= Convert(datetime,'" + todate.ToString("yyyy/MM/dd 00:00:00.000") + "') ";

            }
            else
            {
                sql_query = " select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query + " and  CONVERT(varchar, StartDate, 23) = '" + sdate.ToString() + "'";


            }

            var leaves = dbContext.LeaveRequests.FromSql(sql_query).ToList();
            return leaves;

        }

        public IEnumerable<Employee> LeavesEmployee(ClaimsPrincipal user, string sdate)
        {
            //var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.TeamId).ToList()[0];
            var current_employee_teamid = dbContext.Employees.Where(u => u.EmployeeCode == user.Identity.Name).Select(u => u.teamlist).ToList()[0];


            string[] e_teamids = current_employee_teamid.Split(",");
            string sql_query = "";

            string str_employee_in_query = "(";

            //foreach (string tid in e_teamids)
            //{
                for(int j = 0; j < e_teamids.Length; j++)
                {
                    if(j!=0)
                    {
                        str_employee_in_query = str_employee_in_query + ",";
                    }

                //sql_query = "select * from Employees where teamlist like '%" + e_teamids[j] + "%'";

                sql_query = "SELECT * FROM Employees  WHERE (',' + RTRIM(teamlist) + ',') LIKE '%," + e_teamids[j] + ",%'  and Deactivated=0";

                var employee_current_team = dbContext.Employees.FromSql(sql_query).ToList();
                    for (int i = 0; i < employee_current_team.Count; i++)
                    {

                        if (i == employee_current_team.Count - 1)
                        {
                            str_employee_in_query = str_employee_in_query + employee_current_team[i].Id;
                        }
                        else
                        {
                            str_employee_in_query = str_employee_in_query + employee_current_team[i].Id + ",";
                        }
                    }

             //   }   
            }

            if (e_teamids.Length == 0)
            {
                str_employee_in_query = str_employee_in_query + 0;
            }



            //  var employee_current_team = dbContext.Employees.Where(u => u.TeamId == current_employee_teamid).Select(u => u.Id).ToList();
            // var query = context.Get<LeaveRequest>(t => t.EmployeeId == t.EmployeeId, q => q.OrderByDescending(s => s.LeaveRequestCreatedDate));






            str_employee_in_query = str_employee_in_query + ")";


            // query = query.Where(p => EF.Functions.Contains(p.Employee.Id, "%test%")).ToList();

            

            if (sdate == "")
            {

                sql_query = "select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query;
            }
            else
            {
                sql_query = " select * from LeaveRequests where (Status='PENDING' or Status='APPROVED') and  EmployeeId in " + str_employee_in_query + " and  CONVERT(varchar, StartDate, 23) = '" + sdate.ToString() + "'";


            }

            var leaves = dbContext.LeaveRequests.FromSql(sql_query).ToList();


            string str_empid_in_query = "(";

            if (leaves.Count > 0)
            {
                for (int i = 0; i < leaves.Count; i++)
                {

                    if (i == leaves.Count - 1)
                    {
                        str_empid_in_query = str_empid_in_query + leaves[i].EmployeeId;
                    }
                    else
                    {
                        str_empid_in_query = str_empid_in_query + leaves[i].EmployeeId + ",";
                    }
                }
            }
            else
            {
                str_empid_in_query = str_empid_in_query + 0;
            }

            str_empid_in_query = str_empid_in_query + ")";





            string sql_query1 = "";



            sql_query1 = "Select * from Employees where id in " + str_empid_in_query;

            var employee = dbContext.Employees.FromSql(sql_query1).ToList();
            return employee;

        }




        }
}
