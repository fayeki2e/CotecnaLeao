using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    /*
     * View Model for Employee Search
     */
    public class EmployeeSearchViewModel
    {
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }     
        public int? LocationId { get; set; }
        public int? DesignationId { get; set; }
        public EmployeeStatus? Status { get; set; }

        public int? TeamId { get; set; }
    }
}
