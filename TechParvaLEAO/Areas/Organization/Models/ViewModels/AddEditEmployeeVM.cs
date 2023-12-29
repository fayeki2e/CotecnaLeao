using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    public class AddEditEmployeeVM:Employee
    {
        [Remote(action: "VerifyDuplicateExists", controller: "Employees", AdditionalFields = nameof(Id) + "," + "," + nameof(EmployeeCode))]
        [Display(Name = "Employee Code")]
        [Required]
        public override string EmployeeCode { get; set; }

        [Remote(action: "VerifyDuplicateExists", controller: "Employees", AdditionalFields = nameof(Id) + "," + "," + nameof(Email))]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
         public override string Email { get; set; }

        [Display(Name = "Designation")]
        [Required]
        public override int? DesignationId { get; set; }


        [Display(Name = "Location")]
        [Required]
        public override int? LocationId { get; set; }

        [Display(Name = "Authorization Profile")]
        [Required]
        public override int? AuthorizationProfileId { get; set; }

        [Display(Name = "Expense Profile")]
        [Required]
        public override int? ExpenseProfileId { get; set; }

        [Display(Name = "Reporting To")]
        [Required]
        public override int? ReportingToId { get; set; }


        [Display(Name = "Teams")]
        [Required]
        public override int? TeamId { get; set; }

        public override DateTime? Created_Date { get; set; }
        public override DateTime? Modified_Date { get; set; }
        public override String Created_by { get; set; }
        public override String Modified_by { get; set; }

        public override string roles { get; set; }
       // public string teamlist { get; set; }

        public string  str_teamlist { get; set; }


    }
}
