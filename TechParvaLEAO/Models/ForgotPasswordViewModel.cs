using System;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Models
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

      
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }


        [Display(Name = "Date Of Birth")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateOfBirth { get; set; }




        public string PasswordResetLink { get; set; }
    }
}
