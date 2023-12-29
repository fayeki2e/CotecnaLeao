using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Models
{
    public class ApplicationUserSearchVM
    {
        [Display(Name = "Employee ID")]
        public int? EmployeeProfileId { get; set; }
    }
}
