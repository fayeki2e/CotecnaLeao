using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TechParvaLEAO.Areas.Organization.Models;
using Techparva.GenericRepository;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Expense.Models;

namespace TechParvaLEAO.Areas.BulkUploads.Models
{
    /*
     * Model for ExcelStructure
     */
    public partial class ExcelStructure : Entity<int>, IAggregateRoot
    {
        public ExcelStructure() { }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Row_ID { get; set; }
        [Display(Name = "Employee Code")]
        public string Employee_Code { get; set; }

        [Display(Name = "Employee")]
        public string Employee { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Authorization Profile")]
        public string Authorization_Profile { get; set; }
        [Display(Name = "Expense Profile")]
        public string Expense_Profile { get; set; }

        [Display(Name = "Teams")]
        public string Teams { get; set; }

        [Display(Name = "Account Number")]
        public string Account_Number { get; set; }
        [Display(Name = "Reporting To")]
        public string Reporting_To { get; set; }
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Joining")]
        public string Date_of_Joining { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Date of Birth")]
        public string Date_of_Birth { get; set; }
        [Display(Name = "Overtime Rule")]
        public string Overtime_Rule { get; set; }
        [Display(Name = "Can Apply Mission Leaves")]
        public string Can_Apply_Mission_Leaves { get; set; }

        [Display(Name = "Can Create Forex Requests")]
        public string Can_Create_Forex_Requests { get; set; }

        [Display(Name = "Can have Credit card")]
        public string Can_have_Credit_card{ get;set;}

        [Display(Name = "Is Hr")]
        public Boolean Is_Hr { get; set; }

        [Display(Name = "On Field Employee")]
        public string On_Field_Employee        { get; set; }

        [Display(Name = "Specific Weekly-Off")]
        public string Specific_Weekly_Off { get; set; }
    }


}