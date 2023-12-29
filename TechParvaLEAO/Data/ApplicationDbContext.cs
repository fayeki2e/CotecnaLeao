using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Models;
using System.Security.Claims;
using TechParvaLEAO.Areas.Reports.Models;

namespace TechParvaLEAO.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
     

            this.Database.SetCommandTimeout(TimeSpan.FromSeconds(500));   
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
          

        }
        
        public virtual DbSet<ApplicationConfiguration> ApplicationConfiguration { get; set; }
        public virtual DbSet<LeaveDraft> LeaveDrafts { get; set; }
        public virtual DbSet<PaymentRequestDraft> PaymentRequestDraft { get; set; }        
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Designation> Designations { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<LocationOvertimeRule> LocationOvertimeRule { get; set; }
        public virtual DbSet<LocationWorkHours> LocationWorkHours { get; set; }
        public virtual DbSet<ApprovalLimitProfile> ApprovalLimitProfiles { get; set; }
        public virtual DbSet<BusinessUnit> BusinessUnits { get; set; }
        public virtual DbSet<ExpenseProfile> ExpenseProfiles { get; set; }
        public virtual DbSet<EmployeeBasicSalary> EmployeeBasicSalaries { get; set; }
        public virtual DbSet<EmployeeWeeklyOff> EmployeeWeeklyOff { get; set; }
        public virtual DbSet<FinancialYear> FinancialYears { get; set; }
        public virtual DbSet<EmployeeClaimSeries> EmployeeClaimSeries { get; set; }
        public virtual DbSet<BusinessActivity> BusinessActivities { get; set; }
        public virtual DbSet<Currency> Currencies { get; set; }
        public virtual DbSet<CustomerMarket> CustomerMarkets { get; set; }
        public virtual DbSet<BusinessActivityCustomerMarketMapping> BusinessActivityCustomerMarketMapping { get; set; }
        public virtual DbSet<ExpenseHead> ExpenseHeads { get; set; }
        public virtual DbSet<PaymentRequest> PaymentRequests { get; set; }
        public virtual DbSet<PaymentRequestLineItems> PaymentRequestLineItems { get; set; }
        public virtual DbSet<PaymentRequestApprovalAction> PaymentRequestApprovalActions { get; set; }
        public virtual DbSet<PaymentRequestRejectionReason> PaymentRequestRejectionReasons { get; set; }
        public virtual DbSet<PaymentRequestSeriesSequence> PaymentRequestSeriesSequence { get; set; }
        public virtual DbSet<PaymentRequestPaymentRecord> PaymentRequestPaymentRecords { get; set; }
        public virtual DbSet<LeaveType> LeaveTypes { get; set; }
        public virtual DbSet<LeaveRejectionReason> LeaveRejectionReasons { get; set; }
        public virtual DbSet<LeaveAccountingPeriod> LeaveAccountingPeriods { get; set; }
        public virtual DbSet<Holiday> Holidays { get; set; }
        public virtual DbSet<LeaveRequest> LeaveRequests { get; set; }
        public virtual DbSet<LeaveRequestAction> LeaveRequestActions { get; set; }
        public virtual DbSet<LeaveCreditAndUtilization> LeaveCreditAndUtilization { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Job> Jobs { get; set; }
        public virtual DbSet<TimeSheet> TimeSheets { get; set; }
        public virtual DbSet<TimesheetAttendanceRecord> TimesheetAttendanceRecord { get; set; }
        public virtual DbSet<TimesheetCompOff> TimesheetCompOff { get; set; }
        public virtual DbSet<ExpenseHeadExpenseProfileMapping> ExpenseHeadExpenseProfileMappings { get; set; }
        public virtual DbSet<LeaveCategory> LeaveCategories { get; set; }
        public virtual DbSet<LeaveSubCategory> LeaveSubCategories { get; set; }
        public virtual DbSet<LeaveTypeLeaveCategoryLeaveSubCategoryMapping> LeaveTypeLeaveCategoryLeaveSubCategoryMapping { get; set; }
        public virtual DbSet<EmailNotificationConfiguration> EmailNotificationConfiguration { get; set; }
        public DbSet<OvertimeRule> OvertimeRule { get; set; }
        public virtual DbSet<AdvanceExpenseAdjustment> AdvanceExpenseAdjustments { get; set; }


        public virtual DbSet<Team> Team { get; set; }
    }
}
