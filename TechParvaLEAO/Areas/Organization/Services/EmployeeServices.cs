﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Models.ViewModels;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Service;

namespace TechParvaLEAO.Areas.Organization.Services
{
    public interface IEmployeeServices
    {
        Task<Employee> GetEmployee(ClaimsPrincipal User);
        Task<Employee> GetEmployee(int employeeId);

        IEnumerable<Team> GetTeam(string teamId);

        Task<IEnumerable<Employee>> GetSubordinates(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetOnFieldSubordinates(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetOwnEnumerable(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetselfEmployee(ClaimsPrincipal User);

        Task<IEnumerable<Employee>> GetReportingEmployeesAsync(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetAllEmployeesAsync(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetOnFieldEmployeeForTimesheet(ClaimsPrincipal User);
        Task<IEnumerable<Employee>> GetOnFieldEmployeeForTimesheet(Employee employee);
        Task<IEnumerable<Employee>> GetAllOnFieldEmployeeForTimesheet();        
        Task<double> GetBasicSalary(int employeeId, DateTime asOnDate);

        double GetBasicSalary_data(int employeeId, DateTime asOnDate);

        Task<EmployeeWeeklyOff> GetSpecificWeeklyOff(Employee employee, DateTime? effectiveDate);
        Task<Employee> GetHr(Employee employee);
        Task<Employee> GetVOCHead();

        IEnumerable<Employee> GetReportingManager();

        DataSet  GetAllEmployeeList_Report();
        DataSet GetAllEmployeeList_TemplateReport();
    }


    public class EmployeeServices : IEmployeeServices
    {
        private readonly IApplicationRepository _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext dbContext;
        private readonly IDBConnectionEnhance dbConnection;

        public EmployeeServices(IApplicationRepository context, UserManager<ApplicationUser> userManager ,
            ApplicationDbContext dbContext, IDBConnectionEnhance DBconnection)
        {
            this._context = context;
            this._userManager = userManager;
            this.dbContext = dbContext;
            dbConnection = DBconnection;
        }

        public async Task<Employee> GetEmployee(ClaimsPrincipal User)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = await GetEmployee(user.EmployeeProfileId.Value);
            return result;
        }

       

        public async Task<IEnumerable<Employee>> GetOwnEnumerable(ClaimsPrincipal User)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            var result = await _context.GetAsync<Employee>(e=> e.Id==user.EmployeeProfileId.Value);
            return result;
        }

        public async Task<Employee> GetEmployee(int employeeId)
        {
            return await _context.GetFirstAsync<Employee>(e => e.Id == employeeId);
        }


        public async Task<IEnumerable<Employee>> GetselfEmployee(ClaimsPrincipal User)
        {
            var employee = await GetEmployee(User);

            return await _context.GetAsync<Employee>(e => e.Deactivated == false &&
                   (e.ReportingToId == employee.Id || e.Id == employee.Id), o => o.OrderBy(b => b.Name));

        }
        public IEnumerable<Team>  GetTeam(string teamIds)
        {

            string sql_query = "";

            // sql_query = " select * from Employees where DesignationId in (select Id from Designations where name like '%manager%')";

            sql_query = "select * from team ";
            sql_query = sql_query + " where id in (" + teamIds +")";
            var teams = dbContext.Team.FromSql(sql_query).ToList();


            // var result = await _context.GetAsync<Employee>(e => e.IsHr == true);
            return teams;



          //  return   _context.GetFirstAsync<Team>(t => t.Id == teamId);
        }


        public async Task<Employee> GetHr(Employee employee)
        {
            var result = await _context.GetAsync<Employee>(e => e.IsHr == true);
            return null;
        }

        public IEnumerable<Employee> GetReportingManager()
        {
            string sql_query = "";

            // sql_query = " select * from Employees where DesignationId in (select Id from Designations where name like '%manager%')";

            sql_query = "select E.* from Employees E inner join AspNetUsers ANU on E.EmployeeCode=ANU.UserName";
            sql_query = sql_query + " inner join AspNetUserRoles AUR on AUR.UserId = ANU.Id";
            sql_query = sql_query + " inner join AspNetRoles ANR on ANR.Id = AUR.RoleId";
            sql_query = sql_query + " where AUR.RoleId = 'MANAGER'";
            var reporting_manager = dbContext.Employees.FromSql(sql_query).ToList();


           // var result = await _context.GetAsync<Employee>(e => e.IsHr == true);
            return reporting_manager;
        }

        public async Task<Employee> GetVOCHead()
        {
            var vocBusinessUnit = await _context.GetOneAsync<BusinessUnit>(u => u.Name == "VOC");
            return vocBusinessUnit.BUHead;
        }

        public async Task<double> GetBasicSalary(int employeeId, DateTime asOnDate)
        {
            var basicSalaryRecord = await _context.GetFirstAsync<EmployeeBasicSalary>(
                s => s.EmployeeId == employeeId && 
                (s.FromDate<=asOnDate&&s.ToDate>=asOnDate));
            if (basicSalaryRecord!=null)
            {
                return basicSalaryRecord.BaseSalary;
            }
            return 0.0;
        }

        public    double GetBasicSalary_data(int employeeId, DateTime asOnDate)
        {
            EmployeeBasicSalary basicSalaryRecord1 = new EmployeeBasicSalary();

            // return _context.Get<EmployeeBasicSalary>(s => s.EmployeeId == employeeId && s.FromDate <= asOnDate && s.ToDate >= asOnDate);

            var basicSalaryRecord = new List<EmployeeBasicSalary>();


            basicSalaryRecord = (List<EmployeeBasicSalary>)_context.Get<EmployeeBasicSalary>(s => s.EmployeeId == employeeId && s.FromDate <= asOnDate && s.ToDate >= asOnDate);

     


            if(basicSalaryRecord.Count() > 0)
            {
                basicSalaryRecord1 = basicSalaryRecord[0];
                return basicSalaryRecord1.BaseSalary;
            }

          //  basicSalaryRecord = (EmployeeBasicSalary)_context.Get<EmployeeBasicSalary>(s => s.EmployeeId == employeeId && s.FromDate <= asOnDate && s.ToDate >= asOnDate);
            //if (basicSalaryRecord1 != null)
            //{
            //    return basicSalaryRecord1.BaseSalary;
            //}
            return 0.0;
        }






        public async Task<IEnumerable<Employee>> GetSubordinates(ClaimsPrincipal User)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return await _context.GetAsync<Employee>(e => e.Deactivated == false && e.ReportingToId == user.EmployeeProfileId);
        }

        public async Task<IEnumerable<Employee>> GetOnFieldSubordinates(ClaimsPrincipal User)
        {
            ApplicationUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return await _context.GetAsync<Employee>(e => e.Deactivated == false && 
                e.OnFieldEmployee == true && 
                e.ReportingToId == user.EmployeeProfileId);
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync(ClaimsPrincipal User)
        {
            return await _context.GetAsync<Employee>(e => e.Deactivated == false, o => o.OrderBy(b => b.Name));
        }

        public async Task<IEnumerable<Employee>> GetReportingEmployeesAsync(ClaimsPrincipal User)
        {
            
             
            if (User.IsInRole(AuthorizationRoles.FINANCE))
            {
                return await _context.GetAsync<Employee>(e => e.Deactivated == false, o => o.OrderBy(b => b.Name));
            }
            else
            {
                var employee = await GetEmployee(User);
                return await _context.GetAsync<Employee>(e => e.Deactivated == false &&
                    (e.ReportingToId == employee.Id || e.Id == employee.Id), o => o.OrderBy(b => b.Name));

            }



        }


        public async Task<IEnumerable<Employee>> GetAvailableEmployeesAsync(ClaimsPrincipal User)
        {
            if (User.IsInRole(AuthorizationRoles.FINANCE))
            {
                return await _context.GetAsync<Employee>(e => e.Deactivated == false, o=>o.OrderBy(b=>b.Name));
            }
            else if (User.IsInRole(AuthorizationRoles.MANAGER))
            {
                var employee = await GetEmployee(User);
                if (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR))
                {
                    return await _context.GetAsync<Employee>(e => e.Deactivated == false &&
                        ((e.LocationId == employee.Location.Id) || e.Id == employee.Id), o => o.OrderBy(b => b.Name));
                }
                else
                {
                    return await _context.GetAsync<Employee>(e => e.Deactivated == false &&
                        (e.ReportingToId == employee.Id || e.Id == employee.Id), o => o.OrderBy(b => b.Name));
                }
            }
            else if (User.IsInRole(AuthorizationRoles.HR))
            {
                return await _context.GetAsync<Employee>(e => e.Deactivated == false, o => o.OrderBy(b => b.Name));
            } else if (User.IsInRole(AuthorizationRoles.LOCATION_COORDINATOR))
            {
                var employee = await GetEmployee(User);

    //            return await _context.GetAsync<Employee>(e => e.Deactivated == false &&
    //((e.LocationId == employee.Location.Id && e.Email == null) || e.Id == employee.Id), o => o.OrderBy(b => b.Name));

                return await _context.GetAsync<Employee>(e => e.Deactivated == false && 
                    ((e.LocationId == employee.Location.Id  )|| e.Id == employee.Id) , o => o.OrderBy(b => b.Name));
            } else if (User.IsInRole(AuthorizationRoles.TIMESHEET))
            {
                var employee = await GetEmployee(User);
                return await _context.GetAsync<Employee>(e => e.Deactivated == false && 
                (e.LocationId == employee.Location.Id || e.Id == employee.Id) && e.OnFieldEmployee == true, o => o.OrderBy(b => b.Name));
            }
            else if(User.IsInRole(AuthorizationRoles.EMPLOYEE))
            {
                var employee = await GetEmployee(User);
                return await _context.GetAsync<Employee>(e => e.Id == employee.Id, o => o.OrderBy(b => b.Name));
            }
            return null;
        }

        public async Task<IEnumerable<Employee>> GetOnFieldEmployeeForTimesheet(Employee employee)
        {
            return await _context.GetAsync<Employee>(e => e.Deactivated == false && e.LocationId == employee.Location.Id
                    && e.OnFieldEmployee == true,
                    o => o.OrderBy(b => b.Name));
        }
        public async Task<IEnumerable<Employee>> GetOnFieldEmployeeForTimesheet(ClaimsPrincipal User)
        {
            var employee = await GetEmployee(User);
            return await this.GetOnFieldEmployeeForTimesheet(employee);
        }
        public async Task<IEnumerable<Employee>> GetAllOnFieldEmployeeForTimesheet()
        {
            return await _context.GetAsync<Employee>(e => e.Deactivated == false && e.OnFieldEmployee == true,
                    o => o.OrderBy(b => b.Name));
        }

        public async Task<EmployeeWeeklyOff> GetSpecificWeeklyOff(Employee employee, DateTime? effectiveDate)
        {
            if (effectiveDate == null) effectiveDate = DateTime.Today;
            if (employee.SpecificWeeklyOff)
            {
                var result = await _context.GetAsync<EmployeeWeeklyOff>(w => w.EmployeeId == employee.Id && 
                             w.FormDate >= effectiveDate && w.ToDate <= effectiveDate);
                return result.FirstOrDefault();
            }
            return null;
        }

        public IEnumerable<Employee> SearchEmployeePrint(EmployeeSearchViewModel searchVm)
        {
            var query = dbContext.Employees;
            var searchResult = ApplySearchOnEmployee(query, searchVm);
            return searchResult;

        }

        private IEnumerable<Employee> ApplySearchOnEmployee(IQueryable<Employee> query, EmployeeSearchViewModel searchVm)
        {
            if (searchVm == null) return query;

            //if (searchVm.FromDate != null)
            //{
            //    query = query.Where(t => t.StartDate >= searchVm.FromDate);
            //}
            //if (searchVm.ToDate != null)
            //{
            //    query = query.Where(t => t.EndDate <= searchVm.ToDate);
            //}
            //if (searchVm.Location != null)
            //{
            //    query = query.Where(t => t.Employee.LocationId == searchVm.Location);
            //}
            //if (searchVm.Employee != null)
            //{
            //    query = query.Where(t => t.EmployeeId == searchVm.Employee);
            //}
            return query;
        }

     

        public IEnumerable<Employee> GetAllEmployeeList()
        {
            var result = dbContext.Employees.FromSql("dbo.sp_GetBalancePayable_Report");

            return result;
        }

        public DataSet GetAllEmployeeList_Report()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", dbConnection.Get_DB_Connection());
                //cmd.Parameters.Add("@BankName", SqlDbType.NVarChar).Value = Bank_Name;
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adpt.Fill(dt);

                ds.Tables.Add(dt);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataSet GetAllEmployeeList_TemplateReport()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_GetAllEmployees", dbConnection.Get_DB_Connection());
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Select";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adpt.Fill(dt);

                ds.Tables.Add(dt);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

    }
}
