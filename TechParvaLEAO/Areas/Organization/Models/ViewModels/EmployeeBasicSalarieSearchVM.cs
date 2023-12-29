using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    /*
     * View Model for Employee Basic Salarie Search
     */
    public class EmployeeBasicSalarieSearchVM
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        public int? EmployeeId { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }

        [Display(Name = "Base Salary")]
        public double BaseSalary { get; set; }
    }
}
