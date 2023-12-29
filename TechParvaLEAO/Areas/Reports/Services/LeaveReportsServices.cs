using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;

namespace TechParvaLEAO.Areas.Reports.Services
{
    public class LeaveReportsServices
    {
        private readonly ApplicationDbContext dbContext;
        private readonly LeaveRequestServices leaveRequestServices;

        public LeaveReportsServices(ApplicationDbContext dbContext, LeaveRequestServices leaveRequestServices)
        {
            this.dbContext = dbContext;
            this.leaveRequestServices = leaveRequestServices;
        }

        private IEnumerable<LeaveRequest> ApplyLeaveSearch(IEnumerable<LeaveRequest> query, LeaveReportSearchVm searchVm)
        {
            if(searchVm.FromDate == null && searchVm.ToDate == null && searchVm.LeaveYear == null)
            {
                var presentYear = leaveRequestServices.GetAccountingPeriod(DateTime.Today);
                query = query.Where(l => l.LeaveAccountingPeriod == presentYear);
            }
            if (searchVm.Employee != null)
            {
                query = query.Where(l => l.EmployeeId == searchVm.Employee);
            }
            if (searchVm.is_reporting_exist == 1)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.current_emp_id);
            }
            if (searchVm.is_reporting_exist == 1)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.current_emp_id);
            }

            if (searchVm.ReportingManager != null)
            {
                query = query.Where(l => l.LeaveRequestApprovedById == searchVm.ReportingManager);
            }
            if (searchVm.Branch != null)
            {
                query = query.Where(l => l.Employee.LocationId == searchVm.Branch);
            }

            if (searchVm.FromDate != null && searchVm.ToDate != null)
            {
                query = query.Where(l => l.StartDate >= searchVm.FromDate && l.StartDate<= searchVm.ToDate||
                (l.StartDate <= searchVm.ToDate && l.EndDate >= searchVm.ToDate)||
                (l.StartDate <= searchVm.FromDate && l.EndDate >= searchVm.FromDate));
            }
            else
            {
                if (searchVm.FromDate != null)
                {
                    query = query.Where(l => l.StartDate >= searchVm.FromDate || l.EndDate >= searchVm.FromDate);
                }
                if (searchVm.ToDate != null)
                {
                    query = query.Where(l => l.StartDate <= searchVm.ToDate || l.EndDate <= searchVm.ToDate);
                }
            }

            if (searchVm.LeaveYear != null)
            {
                query = query.Where(l => l.LeaveAccountingPeriodId == searchVm.LeaveYear);
            }
            if (searchVm.LeaveType != null)
            {
                query = query.Where(l => l.LeaveTypeId == searchVm.LeaveType);
            }
            if(searchVm.JoiningFromDate != null)
            {
                query = query.Where(l => l.Employee.DateOfJoining >= searchVm.JoiningFromDate);
            }
            if (searchVm.JoiningToDate != null)
            {
                query = query.Where(l => l.Employee.DateOfJoining <= searchVm.JoiningToDate);
            }
            if (searchVm.LWDFromDate != null)
            {
                query = query.Where(l => l.Employee.LastWorkingDate >= searchVm.LWDFromDate);
            }
            if (searchVm.LWDToDate != null)
            {
                query = query.Where(l => l.Employee.LastWorkingDate <= searchVm.LWDToDate);
            }
            if (searchVm.LeaveStatus != null)
            {
                query = query.Where(l => l.Status == searchVm.LeaveStatus);
            }
            else
            {
                query = query.Where(l => l.Status == "APPROVED");
            }

            if(searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            return query;
        }

        private IEnumerable<LeaveCreditAndUtilization> ApplyLeaveBalanceSearch(IEnumerable<LeaveCreditAndUtilization> query, LeaveReportSearchVm searchVm)
        {
            if (searchVm.LeaveYear == null)
            {
                var presentYear = leaveRequestServices.GetAccountingPeriod(DateTime.Today);
                query = query.Where(l => l.LeaveAccountingPeriod == presentYear);
            }
            if (searchVm.Employee != null)
            {
                query = query.Where(l => l.EmployeeId == searchVm.Employee);
            }
            if (searchVm.is_reporting_exist == 1)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.current_emp_id);
            }

            if(searchVm.ReportingManager != null)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.ReportingManager);
            }

            if (searchVm.Branch != null)
            {
                query = query.Where(l => l.Employee.LocationId == searchVm.Branch);
            }
            if (searchVm.LeaveYear != null)
            {
                query = query.Where(l => l.LeaveAccountingPeriodId == searchVm.LeaveYear);
            }
            //if (searchVm.FromDate != null)
            //{
            //    query = query.Where(l => l.AccrualDate >= searchVm.FromDate);
            //}
            //if (searchVm.ToDate != null)
            //{
            //    query = query.Where(l => l.AccrualDate <= searchVm.ToDate);
            //}
            if (searchVm.JoiningFromDate != null)
            {
                query = query.Where(l => l.Employee.DateOfJoining >= searchVm.JoiningFromDate);
            }
            if (searchVm.JoiningToDate != null)
            {
                query = query.Where(l => l.Employee.DateOfJoining <= searchVm.JoiningToDate);
            }
            if (searchVm.LWDFromDate != null)
            {
                query = query.Where(l => l.Employee.LastWorkingDate >= searchVm.LWDFromDate);
            }
            if (searchVm.LWDToDate != null)
            {
                query = query.Where(l => l.Employee.LastWorkingDate <= searchVm.LWDToDate);
            }
            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }
            if(searchVm.is_reporting_exist == 1)
            {
                query = query.Where(l => l.Employee.ReportingToId == searchVm.current_emp_id);
            }

            //query = query.Where(l => l.Employee.Deactivated == searchVm.IncludeDeacivatedEmployees);
            return query;

        }

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

        private double GetPlannedLeaveCount(int employeeId, Dictionary<int, LeaveUtilization> leaveUtilization)
        {
            LeaveUtilization utilization = null;
            if (leaveUtilization.TryGetValue(employeeId, out utilization))
            {
                return utilization.Planned;
            }
            else
            {
                return 0;
            }
        }

        private double GetUnplannedLeaveCount(int employeeId, Dictionary<int, LeaveUtilization> leaveUtilization)
        {
            LeaveUtilization utilization = null;
            if (leaveUtilization.TryGetValue(employeeId, out utilization))
            {
                return utilization.Unplanned;
            }
            else
            {
                return 0;
            }
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

        private Dictionary<int, LeaveUtilization> PopulateLeaveUtilization(IEnumerable<LeaveRequest> query, LeaveReportSearchVm searchVm=null)
        {
            var leaveUtilization = new Dictionary<int, LeaveUtilization>();

            if (searchVm.FromDate != null)
            {
                query = query.Where(l => l.StartDate >= searchVm.FromDate);
            }
            if (searchVm.ToDate != null)
            {
                query = query.Where(l => l.EndDate <= searchVm.ToDate);
            }



            foreach (var leave in query)
            {
                LeaveUtilization utilization = null;
                if (!leaveUtilization.TryGetValue(leave.EmployeeId.Value, out utilization))
                {
                    utilization = new LeaveUtilization();
                    leaveUtilization.Add(leave.EmployeeId.Value, utilization);
                }
                if (leave.LeaveCategoryId == 1)
                {
                    utilization.Planned = utilization.Planned + leave.NumberOfDays;
                }
                else
                {
                    utilization.Unplanned = utilization.Unplanned + leave.NumberOfDays;
                }
            }
            return leaveUtilization;
        }

        public IEnumerable<CarryForwardLeavesReportVm> SearchCarryForwardLeaves(LeaveReportSearchVm searchVm,string empcode,ClaimsPrincipal User)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();

            searchVm.current_emp_id = current_employee_id;
            var is_hr = 0;

            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }


            var query = dbContext.LeaveCreditAndUtilization.Where(c => c.LeaveTypeId == 1);
            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            var searchReesult = ApplyLeaveBalanceSearch(query, searchVm);

            var leaves = dbContext.LeaveRequests.Where(l => l.LeaveTypeId == 1 &&
                                            (l.Status == LeaveRequestStatus.PENDING.ToString() ||
                                                            l.Status == LeaveRequestStatus.APPROVED.ToString()));
            var searchLeavesResult = ApplyLeaveSearch(leaves, searchVm);

            var leaveUtilization = PopulateLeaveUtilization(searchLeavesResult, searchVm);

            var leaveYear = searchVm.LeaveYear == null ? leaveRequestServices.GetAccountingPeriod(DateTime.Today) :
                        dbContext.LeaveAccountingPeriods.Where(l => l.Id == searchVm.LeaveYear).FirstOrDefault();
            var result = searchReesult.Select(b => new CarryForwardLeavesReportVm
            {
                Id = b.Id,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                ReportingManager = b.Employee.ReportingTo?.Name,
                is_reporting_exist = is_reporting_exist,
                Designation = b.Employee.Designation.Name,
                Department = "",
                Branch = b.Employee.Location.Name,
                AnnualLeaves = b.NumberOfDays,
                CarryForwardLastYear = b.CarryForward,
                TotalLeaves = b.NumberOfDays + b.CarryForward,
                LeavesUtilized = GetLeaveCount(b.EmployeeId, leaveUtilization),
                PlannedLeaves = GetPlannedLeaveCount(b.EmployeeId, leaveUtilization),
                UnplannedLeaves = GetUnplannedLeaveCount(b.EmployeeId, leaveUtilization),
                LeavesBalance = b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),
                CarryForward = b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization) >
                        leaveYear.MaxCarryForwardFromLastYear ? leaveYear.MaxCarryForwardFromLastYear :
                        b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),
                TotalLeavesNextYear = b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization) >
                        leaveYear.MaxCarryForwardFromLastYear ? leaveYear.MaxCarryForwardFromLastYear + leaveYear.NumberOfDaysOfLeave:
                        b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization)+ leaveYear.NumberOfDaysOfLeave,
                is_hr = is_hr
            });
            return result;
        }

        public IEnumerable<DateWiseLeaveReportVm> SearchDateWiseLeave(LeaveReportSearchVm searchVm, string empcode,ClaimsPrincipal User)
        {

         

            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;
            var is_hr = 0;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }


            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }


            var query = dbContext.LeaveRequests.Where(l => l.Id == l.Id);//.Where(l => l.Status == LeaveRequestStatus.APPROVED.ToString());
            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            var searchResult = ApplyLeaveSearch(query, searchVm);
            var result = searchResult.Select(l => new DateWiseLeaveReportVm
            {
                Id = l.Id,
                EmployeeCode = l.Employee.EmployeeCode,
                Name = l.Employee.Name,

            
                ReportingManager = l.LeaveRequestApprovedBy.Name,
                is_reporting_exist= is_reporting_exist,


                Designation = l.Employee.Designation.Name,
                Department = l.Employee.Designation.Name,
                Branch = l.Employee.Location.Name,
                FromDate = l.StartDate,
                ToDate = l.EndDate,
                NumberOfDays = l.NumberOfDays,
                LeaveType = l.LeaveType.Name,
                LeaveCatagory = l.LeaveCategory.Text,
                LeaveSubType = l.LeaveSubCategory.Text,
                LeaveStatus = string.Equals(l.Status, "CANCELED") ? "CANCELLED" : l.Status,
                CompOffAgainstDate = l.CompOffAgainstDate?.AccrualDate,
                AnnualLeaves = l.LeaveEligibility,
                CarryForwardLastYear = l.LeavesCarriedForward,
                TotalLeaves = l.LeaveEligibility + l.LeavesCarriedForward,
                LeavesUtilized = l.LeaveTypeId ==1 && string.Equals(l.Status, "APPROVED")? l.LeavesAvailed + l.NumberOfDays: l.LeavesAvailed,
                LeavesBalance = l.LeaveTypeId == 1 && string.Equals(l.Status, "APPROVED") ? l.LeavesPending - l.NumberOfDays: l.LeavesPending,
                is_hr = is_hr
            });
            return result;
        }

        public IEnumerable<DateWiseLeaveReportVm> Searchtrendreport(LeaveReportSearchVm searchVm, string empcode, ClaimsPrincipal User)
        {



            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();
            searchVm.current_emp_id = current_employee_id;

            var is_hr = 0;

            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }



            var query = dbContext.LeaveRequests.Where(l => l.Id == l.Id);//.Where(l => l.Status == LeaveRequestStatus.APPROVED.ToString());

            if (searchVm.ReportType == "trendreport2")
            {
                query = query.Where(l => l.LeaveTypeId == 1 || l.LeaveTypeId == 2 || l.LeaveTypeId == 4 );
            }

            if (searchVm.ReportType == "trendreport3")
            {
                query = query.Where(l => l.LeaveCategoryId ==1 || l.LeaveCategoryId == 2);
            }


            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            var searchResult = ApplyLeaveSearch(query, searchVm);
            var result = searchResult.Select(l => new DateWiseLeaveReportVm
            {
                Id = l.Id,
                EmployeeCode = l.Employee.EmployeeCode,
                Name = l.Employee.Name,


                ReportingManager = l.LeaveRequestApprovedBy.Name,
                is_reporting_exist = is_reporting_exist,


                Designation = l.Employee.Designation.Name,
                Department = l.Employee.Designation.Name,
                Branch = l.Employee.Location.Name,
                FromDate = l.StartDate,
                ToDate = l.EndDate,
                NumberOfDays = l.NumberOfDays,
                LeaveType = l.LeaveType.Name,
                LeaveCatagory = l.LeaveCategory.Text,
                LeaveSubType = l.LeaveSubCategory.Text,
                LeaveStatus = string.Equals(l.Status, "CANCELED") ? "CANCELLED" : l.Status,
                CompOffAgainstDate = l.CompOffAgainstDate?.AccrualDate,
                AnnualLeaves = l.LeaveEligibility,
                CarryForwardLastYear = l.LeavesCarriedForward,
                TotalLeaves = l.LeaveEligibility + l.LeavesCarriedForward,
                LeavesUtilized = l.LeaveTypeId == 1 && string.Equals(l.Status, "APPROVED") ? l.LeavesAvailed + l.NumberOfDays : l.LeavesAvailed,
                LeavesBalance = l.LeaveTypeId == 1 && string.Equals(l.Status, "APPROVED") ? l.LeavesPending - l.NumberOfDays : l.LeavesPending,
               // Day=l.StartDate.DayOfWeek.ToString()
               Day = getdays(l.StartDate, l.EndDate),
                is_hr = is_hr
            });
            return result;
        }

        public string getdays(DateTime fromdate,DateTime todate)
        {
           // var result = new List<DateTime>();

            var result = "";
            var i = 0;
            var day_count= DateIteration(fromdate, todate).ToList().Count();
 
            foreach (DateTime day in DateIteration(fromdate, todate))
            {

                if (++i == day_count)
                {
                    result = result +  day.DayOfWeek.ToString();
                }
                else
                {
                    result = result +  day.DayOfWeek.ToString() +",";
                }


                    
            }

            return result.ToString();

            
        }
        private static IEnumerable<DateTime> DateIteration(DateTime startDate, DateTime endDate)
        {
            for (var date = startDate.Date; date.Date <= endDate.Date; date = date.AddDays(1)) yield
              return date;
        }

        public IEnumerable<EmployeeCompOffReportVm> SearchEmployeeCompOff(LeaveReportSearchVm searchVm, string empcode,ClaimsPrincipal User)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();

            searchVm.current_emp_id = current_employee_id;
            var is_hr = 0;

            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }

            var query = dbContext.LeaveCreditAndUtilization.Where(c=>c.LeaveTypeId==2);

            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            var searchReesult = ApplyLeaveBalanceSearch(query, searchVm);
            var result = searchReesult.Select(b => new EmployeeCompOffReportVm
            {
                Id = b.Id,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                is_reporting_exist = is_reporting_exist,
                ReportingManager = b.Employee.ReportingTo?.Name,
                Designation = b.Employee.Designation.Name,
                Department = "",
                Location = b.Employee.Location.Name,
                LeaveYear = b.LeaveAccountingPeriod.Name,
                CompOffForDate = b.AccrualDate,
                ApprovalDate = b.ApprovedDate,
                ExpiryDate = b.ExpiryDate,
                Utilized = b.AddedUtilized == 0?"Yes": "No",
                is_hr = is_hr
            });
            return result;

        }

        public IEnumerable<JoinedSeperatedEmployeeLeaveReportVm> SearchJoinedEmployeeLeaveCredit(LeaveReportSearchVm searchVm, string empcode,ClaimsPrincipal User)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();

            searchVm.current_emp_id = current_employee_id;
            var is_hr = 0;

            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }
            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }


            var query = dbContext.LeaveCreditAndUtilization.Where(c => c.LeaveTypeId == 1);
            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

            var searchReesult = ApplyLeaveBalanceSearch(query, searchVm);
            var leaveYear = searchVm.LeaveYear == null ? leaveRequestServices.GetAccountingPeriod(DateTime.Today) :
                        dbContext.LeaveAccountingPeriods.Where(l => l.Id == searchVm.LeaveYear).FirstOrDefault();
            searchReesult = searchReesult.Where(c => c.Employee.DateOfJoining >= leaveYear.StartDate && 
                                                c.Employee.DateOfJoining <= leaveYear.EndDate);

            var leaves = dbContext.LeaveRequests.Where(l => l.LeaveTypeId == 1 &&
                                    (l.Employee.DateOfJoining >= leaveYear.StartDate &&
                                    l.Employee.DateOfJoining <= leaveYear.EndDate) &&
                    (l.Status == LeaveRequestStatus.PENDING.ToString() ||
                    l.Status == LeaveRequestStatus.APPROVED.ToString()));
            var searchLeavesResult = ApplyLeaveSearch(leaves, searchVm);
            var leaveUtilization = PopulateLeaveUtilization(searchLeavesResult, searchVm);


            var result = searchReesult.Select(b => new JoinedSeperatedEmployeeLeaveReportVm
            {
                Id = b.Id,
                LeaveYear = leaveYear.StartDate,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                is_reporting_exist = is_reporting_exist,
                ReportingManager = b.Employee.ReportingTo?.Name,
                Designation = b.Employee.Designation.Name,
                Department = "",
                Branch = b.Employee.Location.Name,
                DateOfJoining = b.Employee.DateOfJoining,
                LastWorkingDay = b.Employee.LastWorkingDate,
                AnnualLeaves = b.NumberOfDays,
                CarryForwardLastYear = b.CarryForward,
                TotalLeaves = b.NumberOfDays + b.CarryForward,
                LeavesUtilized = GetLeaveCount(b.EmployeeId, leaveUtilization),
                PlannedLeaves = GetPlannedLeaveCount(b.EmployeeId, leaveUtilization),
                UnplannedLeaves = GetUnplannedLeaveCount(b.EmployeeId, leaveUtilization),
                LeavesBalance = b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),
                is_hr = is_hr
            });
            return result;
        }

        private double ProrateThisYear(DateTime joiningDate, DateTime? lastWorkingDate, double numberOfDays)
        {
            if (lastWorkingDate.HasValue) {
                if (lastWorkingDate.Value.Year == joiningDate.Year)
                {
                    return Math.Round((lastWorkingDate.Value.DayOfYear- joiningDate.DayOfYear) * numberOfDays / 365.00f);
                }
                else
                {
                    return Math.Round(lastWorkingDate.Value.DayOfYear * numberOfDays / 365.00f);
                }

            }
            return numberOfDays;
            

        }
        public IEnumerable<JoinedSeperatedEmployeeLeaveReportVm> SearchSeperatedEmployeeLeaveBalance(LeaveReportSearchVm searchVm, string empcode,ClaimsPrincipal User)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();

            searchVm.current_emp_id = current_employee_id;
            var is_hr = 0;
            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }



            var query = dbContext.LeaveCreditAndUtilization.Where(b=>(b.Employee.Status==EmployeeStatus.RESIGNED||
                                                                     b.Employee.Status == EmployeeStatus.SERVICETERMINATED)
                                                                     && b.LeaveTypeId == 1
                                                                     );
            //if (searchVm.IncludeDeacivatedEmployees == false)
            //{
            //    query = query.Where(l => l.Employee.Deactivated == false);
            //}

            searchVm.IncludeDeacivatedEmployees = true;



            var searchReesult = ApplyLeaveBalanceSearch(query, searchVm);

            var leaves = dbContext.LeaveRequests.Where(l => l.LeaveTypeId == 1 &&
                            (l.Employee.Status == EmployeeStatus.RESIGNED ||
                            l.Employee.Status == EmployeeStatus.SERVICETERMINATED)&&
                                (l.Status == LeaveRequestStatus.PENDING.ToString() ||
                                l.Status == LeaveRequestStatus.APPROVED.ToString()));
            var leaveYear = searchVm.LeaveYear == null ? leaveRequestServices.GetAccountingPeriod(DateTime.Today) :
                        dbContext.LeaveAccountingPeriods.Where(l => l.Id == searchVm.LeaveYear).FirstOrDefault();

            var searchLeavesResult = ApplyLeaveSearch(leaves, searchVm);
            var leaveUtilization = PopulateLeaveUtilization(searchLeavesResult, searchVm);

            var result = searchReesult.Select(b => new JoinedSeperatedEmployeeLeaveReportVm
            {
                Id = b.Id,
                LeaveYear = leaveYear.StartDate,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                ReportingManager = b.Employee.ReportingTo?.Name,
                Designation = b.Employee.Designation.Name,
                Department = "",
                Branch = b.Employee.Location.Name,
                DateOfJoining = b.Employee.DateOfJoining,
                LastWorkingDay = b.Employee.LastWorkingDate,
                AnnualLeaves = ProrateThisYear(b.Employee.DateOfJoining, b.Employee.LastWorkingDate, b.NumberOfDays),
                CarryForwardLastYear = b.CarryForward,
                TotalLeaves = ProrateThisYear(b.Employee.DateOfJoining, b.Employee.LastWorkingDate, b.NumberOfDays) + b.CarryForward,
                LeavesUtilized = GetLeaveCount(b.EmployeeId, leaveUtilization),
                PlannedLeaves = GetPlannedLeaveCount(b.EmployeeId, leaveUtilization),
                UnplannedLeaves = GetUnplannedLeaveCount(b.EmployeeId, leaveUtilization),
                LeavesBalance = ProrateThisYear(b.Employee.DateOfJoining, b.Employee.LastWorkingDate, b.NumberOfDays) + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),
                is_hr = is_hr
            });
            return result;
        }

        public IEnumerable<PaidLeaveBalanceReportVm> SearchPaidLeaveBalance(LeaveReportSearchVm searchVm, string empcode, ClaimsPrincipal User)
        {
            var current_employee_id = dbContext.Employees.Where(u => u.EmployeeCode == empcode).Select(u => u.Id).ToList()[0];
            var is_reporting_exist = dbContext.Employees.Where(u => u.ReportingToId == current_employee_id).Count();

            var is_hr = 0;


            searchVm.current_emp_id = current_employee_id;

            if (is_reporting_exist > 0)
            {
                searchVm.is_reporting_exist = 1;
            }
            else
            {
                searchVm.is_reporting_exist = 0;
            }

            if (User.IsInRole(AuthorizationRoles.HR))
            {
                searchVm.is_reporting_exist = 0;
                is_hr = 1;
            }


                var query = dbContext.LeaveCreditAndUtilization.Where(c => c.LeaveTypeId == 1);
            if (searchVm.IncludeDeacivatedEmployees == false)
            {
                query = query.Where(l => l.Employee.Deactivated == false);
            }

           


                var searchReesult = ApplyLeaveBalanceSearch(query, searchVm);

            var leaves = dbContext.LeaveRequests.Where(l => l.LeaveTypeId ==1 && 
                                            (l.Status == LeaveRequestStatus.PENDING.ToString() ||
                                                            l.Status == LeaveRequestStatus.APPROVED.ToString()));
            var searchLeavesResult = ApplyLeaveSearch(leaves, searchVm);

            var leaveUtilization = PopulateLeaveUtilization(searchLeavesResult, searchVm);

            var result = searchReesult.Select(b => new PaidLeaveBalanceReportVm
            {
                Id = b.Id,
                EmployeeCode = b.Employee.EmployeeCode,
                Name = b.Employee.Name,
                is_reporting_exist = is_reporting_exist,
                ReportingManager = b.Employee.ReportingTo?.Name,
                Designation = b.Employee.Designation.Name,
                Department = "",
                Branch = b.Employee.Location.Name,
                AnnualLeaves = b.NumberOfDays,
                CarryForwardLastYear = b.CarryForward,
                TotalLeaves = b.NumberOfDays + b.CarryForward,
                LeavesUtilized = GetLeaveCount(b.EmployeeId, leaveUtilization),
                PlannedLeaves = GetPlannedLeaveCount(b.EmployeeId, leaveUtilization),
                UnplannedLeaves = GetUnplannedLeaveCount(b.EmployeeId, leaveUtilization),
                LeavesBalance = b.NumberOfDays + b.CarryForward - GetLeaveCount(b.EmployeeId, leaveUtilization),
                is_hr = is_hr
            });
            return result;
        }  
    }
}

