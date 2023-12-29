using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Reports.Models
{
    public class TimesheetReportSearchVm
    {
        [Key]
        int? Id { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }
        public string JobNumber { get; set; }
        public string Client { get; set; }
        public int? Employee { get; set; }
        public string EmployeeName { get; set; }
        public int? Location { get; set; }
        public string LocationName { get; set; }
    }

    public class TimesheetEntriesReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "Job Number")]
        public string JobNumber { get; set; }
        [Display(Name = "Business Activity")]
        public string Client { get; set; }
        [Display(Name = "Job Description")]
        public string JobDescription { get; set; }
        [Display(Name = "In Time")]
        public string InTime { get; set; }
        [Display(Name = "Out Time")]
        public string OutTime { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime TimesheetDate { get; set; }
        [Display(Name = "Total Hours")]
        public string TotalHours { get; set; }
        [Display(Name = "Working Hours")]
        public string WorkingHours{ get; set; }
        [Display(Name = "Overtime Hours")]
        public string OvertimeHours { get; set; }
    }
    public class LocationOvertimeReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Total Number of OT Hours")]
        public double TotalNumberOfOTHours { get; set; }
    }
    public class LocationOvertimePaymentReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Overtime Rule")]
        public string OTRule { get; set; }
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "Total Number of OT Hours")]
        public double TotalNumberOfOTHours { get; set; }
        [Display(Name = "Overtime Amount")]
        [DisplayFormat(DataFormatString = "{0:##,##,##0.00}")]
        public double OvertimeAmount { get; set; }
    }
    public class EmployeeWeeklyWorkedHoursReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }
        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }
        [Display(Name = "Total Hours Worked")]
        public string TotalNumberOfHours { get; set; }
    }
    public class EmployeeWeeklyOTReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Name")]
        public string EmployeeName { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "From Date")]
        public DateTime FromDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        [Display(Name = "To Date")]
        public DateTime ToDate { get; set; }
        [Display(Name = "Total Hours Worked")]
        public string TotalNumberOfHours { get; set; }
        [Display(Name = "Total Number of OT Hours")]
        public string TotalNumberOfOTHours { get; set; }
        [Display(Name = "OT Amount")]
        [DisplayFormat(DataFormatString = "{0:##,##,##0.00}")]
        public double OvertimeAmount { get; set; }
    }
}
