using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;
using Newtonsoft.Json;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
     * Model for Employee
     */
    public partial class Employee : Entity<int>, IAggregateRoot, IValidatableObject
    {
        public Employee()
        {
        }
        [Display(Name = "Employee")]
        [Required]
        public string Name { get; set; }

        public string DisplayName { get
            {
                return this.EmployeeCode + " - " + this.Name;
            }
        }

        [Display(Name = "Designation")]        
        public virtual int? DesignationId { get; set; }
        public virtual Designation Designation { get; set; }


        [Display(Name = "Teams")]
        public virtual int? TeamId { get; set; }
        public virtual Team Teams { get; set; }



        [Display(Name = "Location")]
        [ForeignKey("Location")]
        public virtual int? LocationId { get; set; }
        public virtual Location Location { get; set; }

        [Display(Name = "Authorization Profile")]
        [ForeignKey("AuthorizationProfile")]
        public virtual int? AuthorizationProfileId { get; set; }
        public virtual ApprovalLimitProfile AuthorizationProfile { get; set; }

        [Display(Name = "Expense Profile")]
        [ForeignKey("ExpenseProfile")]
        public virtual int? ExpenseProfileId { get; set; }
        public virtual ExpenseProfile ExpenseProfile { get; set; }

        [Display(Name = "Employee Code")]
        [Required]
        public virtual string EmployeeCode { get; set; }
      
        [Display(Name = "Account Number")]
        public string AccountNumber { get; set; }

        [Display(Name = "Reporting To")]
        [ForeignKey("ReportingTo")]
        public virtual int? ReportingToId { get; set; }
        public virtual Employee ReportingTo { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        public virtual string Email { get; set; }

        public string Gender { get; set; }

        public virtual string teamlist { get; set; }

        public bool CanCreateMaternityLeaves()
        {
            if (DateOfJoining == null) return false;
            if (string.Equals(Gender, "M")) return false;
            if (string.Equals(Gender, "F") && (DateTime.Today-DateOfJoining).TotalDays>90) return true;
            return false;
        }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Date of Joining")]
        public DateTime DateOfJoining { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Display(Name = "Can Create Forex Requests")]
        public bool CanCreateForexRequests { get; set; }

        [Display(Name = "Can Apply Mission Leaves")]
        public bool CanApplyMissionLeaves { get; set; }

        [Display(Name = "Overtime Rule")]
        public virtual int? OvertimeMultiplierId { get; set; }

        [Display(Name = "Overtime Rule")]
        public virtual OvertimeRule OvertimeMultiplier { get; set; }

        [Display(Name = "Can have Credit card")]
        public bool CanHoldCreditCard { get; set; }

        [Display(Name = "On Field Employee")]
        public bool OnFieldEmployee { get; set; }

        [Display(Name = "Specific Weekly-Off")]
        public bool SpecificWeeklyOff { get; set; }

        [Display(Name = "Is Hr")]
        public bool IsHr { get; set; }

        [JsonIgnore]
        public virtual IEnumerable<EmployeeWeeklyOff> EmployeeWeeklyOffs { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Last Working Date")]
        public DateTime? LastWorkingDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Resignation Date")]
        public DateTime? ResignationDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Settlement Date")]        
        public DateTime? SettlementDate { get; set; }

        public double SettlementAmount { get; set; }

        /*
        [Display(Name = "Settlement By")]
        [ForeignKey("SettlementBy")]
        [JsonIgnore]
        public virtual int? SettlementById { get; set; }
        [JsonIgnore]
        public virtual Employee SettlementBy { get; set; }
        */

        public EmployeeStatus Status { get; set; }

        public bool Deactivated { get; set; } = false;        
        public virtual IEnumerable<EmployeeBasicSalary> BasicSalaryHistory { get; set; }

        public virtual DateTime? Created_Date { get; set; }
        public virtual DateTime? Modified_Date { get; set; }
        public virtual String Created_by { get; set; }
        public virtual String Modified_by { get; set; }

        public virtual String roles { get; set; }

     


        public string DefaultPassword()
        {
            return this.EmployeeCode.ToUpper() + "@" + this.DateOfBirth.ToString("ddMMyyyy");
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();
            if (this.DateOfBirth >= DateTime.Now)
            {
                results.Add(new ValidationResult("Date of Birth must be earlier than today",new string[] { "DateOfBirth" }));
            }
            if (this.DateOfBirth >= this.DateOfJoining)
            {
                results.Add(new ValidationResult("Date of Birth must be earlier than Date of Joining", new string[] { "DateOfBirth" }));
            } 
            if(this.OnFieldEmployee && !this.OvertimeMultiplierId.HasValue)
            {
                results.Add(new ValidationResult("Overtime Rule must be selected for On Field employee", new string[] { "OvertimeMultiplierId" }));
            }

            return results;
        }

        public static implicit operator List<object>(Employee v)
        {
            throw new NotImplementedException();
        }
    }

    /*
     * Constant for Employee Status
     */
    public enum EmployeeStatus
    {
        [Display(Name ="In Service")]
        INSERVICE,
        [Display(Name = "Resigned")]
        RESIGNED,
        [Display(Name = "Service Terminated")]
        SERVICETERMINATED
    }
}