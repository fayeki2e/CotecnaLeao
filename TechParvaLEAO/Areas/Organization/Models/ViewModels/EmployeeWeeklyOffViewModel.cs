using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using MediatR;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    /*
     * View Model for Employee Weekly Off
     */
    public class EmployeeWeeklyOffViewModel : IRequest<bool>
    {
        [Required(ErrorMessage = "Please Select Employee.")]
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }

        [Display(Name = "Weekly Off Day")]
        public DayOfWeek WeeklyOffDay { get; set; }

        [Display(Name = "Other Weekly Off Day")]
        public DayOfWeek? OtherWeeklyOffDay { get; set; }

        [Display(Name = "Other Weekly Off Day Rule")]
        public WorkDayType? OtherWeeklyOffRule { get; set; }
    }
}
