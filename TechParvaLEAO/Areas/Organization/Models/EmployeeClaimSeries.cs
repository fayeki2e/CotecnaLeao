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
     * Model for Employee Claim Series
     */
    public partial class EmployeeClaimSeries : Entity<int>
    {
        public EmployeeClaimSeries()
        {
        }
        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }

        [Display(Name = "Financial Year")]
        [ForeignKey("FinancialYear")]
        public virtual int FinancialYearId { get; set; }
        public virtual FinancialYear FinancialYear { get; set; }

        [Display(Name = "Serial Number")]
        public int SerialNumber { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}