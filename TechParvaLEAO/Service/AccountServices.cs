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

namespace TechParvaLEAO.Services
{
    public interface IAccountServices
    {
        Task<Employee> GetEmployee(string EmployeeCode);

        Employee validateempdetails(string employeecode, DateTime dob);

    }

    public class AccountServices : IAccountServices
    {
        private readonly IApplicationRepository context;
        private readonly LocationWorkdaysService locationWorkdaysService;
        private readonly ApplicationDbContext dbContext;
        private readonly IEmployeeServices employeeServices;
       

        public AccountServices(IApplicationRepository context,
            LocationWorkdaysService locationWorkdaysService,
            ApplicationDbContext dbContext,
            IEmployeeServices employeeServices)
        {
            this.context = context;
            this.locationWorkdaysService = locationWorkdaysService;
            this.dbContext = dbContext;
            this.employeeServices = employeeServices;
        }


        public async Task<Employee> GetEmployee(string EmployeeCode)
        {
            return await context.GetFirstAsync<Employee>(e => e.EmployeeCode == EmployeeCode);
        }


        public  Employee validateempdetails(string employeecode,DateTime dob)
        {
            var emp =  context.GetFirst<Employee>(e => e.EmployeeCode == employeecode && e.DateOfBirth==dob);


            return emp;
        }

        //public async void  GetUser(string email)
        //{
        //    return await context.GetFirstAsync<ApplicationUser>(e => e.Email == email);
        //}


    }
}
