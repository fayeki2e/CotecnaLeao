using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Areas.Leave.Models.ViewModels
{
    /*
    * View Model for Leave Request Search
    */
    public class LeaveRequestSearchViewModel
    {
        [Display(Name = "Include Deactivated Employees")]
        public bool IncludeDeacivatedEmployees { get; set; }
        public string EmployeeName { get; set; }
        public string EmployeeCode { get; set; }
        public string Status { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }
        public string ApprovedBy { get; set; }
    }
}
