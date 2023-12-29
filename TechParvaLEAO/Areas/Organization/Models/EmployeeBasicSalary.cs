using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
     * Model for Employee Basic Salary
     */
    public partial class EmployeeBasicSalary: Entity<int>
    {
        public EmployeeBasicSalary()
        {
        }
        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; } = new DateTime(2099, 12, 31);

        [Display(Name = "Base Salary")]
        public double BaseSalary { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}