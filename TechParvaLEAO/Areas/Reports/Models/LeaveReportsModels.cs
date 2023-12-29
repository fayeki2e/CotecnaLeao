using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Reports.Models
{
    public class LeaveReportSearchVm
    {
        [Key]
        int? Id { get; set; }

        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ToDate { get; set; }

        [Display(Name = "Joining From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningFromDate { get; set; }

        [Display(Name = "Joining To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? JoiningToDate { get; set; }

        [Display(Name = "Last Working From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LWDFromDate { get; set; }

        [Display(Name = "Last Working To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? LWDToDate { get; set; }

        [Display(Name = "Leave Year")]
        public int? LeaveYear { get; set; }

        [Display(Name = "Leave Type")]
        public int? LeaveType { get; set; }
        public string LeaveTypeName { get; set; }

        [Display(Name = "Leave Status")]
        public string LeaveStatus { get; set; }

        [Display(Name = "Employee")]
        public int? Employee { get; set; }
        public string EmployeeName { get; set; }

        [Display(Name = "Reporting Manager")]
        public int? ReportingManager { get; set; }
        public string ReportingManagerName { get; set; }

        [Display(Name = "Branch")]
        public int? Branch { get; set; }
        public string BranchName { get; set; }

        public bool Joining { get; set; }

        [Display(Name = "Include Deactivated Employees")]
        public bool IncludeDeacivatedEmployees { get; set; }
        public int is_reporting_exist { get; set; }

        public int current_emp_id { get; set; }

        public string ReportType { get; set; }

        public int is_hr { get; set; }

    }

    public class PaidLeaveBalanceReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "Annual Leaves Pro Rata")]
        public double AnnualLeaves { get; set; }
        [Display(Name = "Carry Forward From Last Year")]
        public double CarryForwardLastYear { get; set; }
        [Display(Name = "Total Leaves")]
        public double TotalLeaves { get; set; }
        [Display(Name = "Leaves Utilized")]
        public double LeavesUtilized { get; set; }
        [Display(Name = "Planned Leaves")]
        public double PlannedLeaves { get; set; }
        [Display(Name = "Unplanned Leaves")]
        public double UnplannedLeaves { get; set; }
        [Display(Name = "Leaves Pending")]
        public double LeavesBalance { get; set; }
        public int is_reporting_exist { get; set; }

        public int is_hr { get; set; }
    }

    public class DateWiseLeaveReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Branch")]
        public string Branch { get; set; }
        [Display(Name = "From Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime FromDate { get; set; }
        [Display(Name = "To Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime ToDate { get; set; }
        [Display(Name = "Number Of Days")]
        //[DisplayFormat(DataFormatString = "{0:##,##,##0.00}")]
        public double NumberOfDays { get; set; }
        [Display(Name = "Leave Type")]
        public string LeaveType { get; set; }
        [Display(Name = "Leave Sub Type")]
        public string LeaveSubType { get; set; }
        [Display(Name = "Leave Category")]
        public string LeaveCatagory { get; set; }
        [Display(Name = "Leave Status")]
        public string LeaveStatus { get; set; }
        [Display(Name = "Comp Off Against Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? CompOffAgainstDate { get; set; }
        [Display(Name = "Annual Leaves Pro Rata")]
        public double AnnualLeaves { get; set; }
        [Display(Name = "Carry Forward From Last Year")]
        public double CarryForwardLastYear { get; set; }
        [Display(Name = "Total Leaves")]
        public double TotalLeaves { get; set; }
        [Display(Name = "Leaves Utilized")]
        public double LeavesUtilized { get; set; }
        public double UnplannedLeaves { get; set; }
        [Display(Name = "Leaves Pending")]
        public double LeavesBalance { get; set; }

        public int is_reporting_exist { get;set; }
        public string Day { get; set; }
        public int is_hr { get; set; }
    }

    public class CarryForwardLeavesReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime Date { get; set; }
        [Display(Name = "Location")]
        public string Branch { get; set; }

        [Display(Name = "Annual Leaves Pro Rata")]
        public double AnnualLeaves { get; set; }
        [Display(Name = "Carry Forward From Last Year")]
        public double CarryForwardLastYear { get; set; }
        [Display(Name = "Total Leaves")]
        public double TotalLeaves { get; set; }
        [Display(Name = "Leaves Utilized")]
        public double LeavesUtilized { get; set; }
        [Display(Name = "Planned Leaves")]
        public double PlannedLeaves { get; set; }
        [Display(Name = "Unplanned Leaves")]
        public double UnplannedLeaves { get; set; }
        [Display(Name = "Leaves Pending")]
        public double LeavesBalance { get; set; }
        [Display(Name = "Carry Forward To Next Year")]
        public double CarryForward { get; set; }
        [Display(Name = "Total PL")]
        public double TotalLeavesNextYear { get; set; }

        public int is_reporting_exist {get; set;}
        public int is_hr { get; set; }
    }

    public class JoinedSeperatedEmployeeLeaveReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Leave Year")]
        [DisplayFormat(DataFormatString = "{0:yyyy}")]
        public DateTime LeaveYear { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Date Of Joining")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime DateOfJoining { get; set; }
        [Display(Name = "Last Working Day")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? LastWorkingDay { get; set; }
        public string Branch { get; set; }
        [Display(Name = "Eligiblity till LWD (Pro-rata)")]
        public double AnnualLeaves { get; set; }
        [Display(Name = "Carry Forward From Last Year")]
        public double CarryForwardLastYear { get; set; }
        [Display(Name = "Total Leaves")]
        public double TotalLeaves { get; set; }
        [Display(Name = "Leaves Utilized")]
        public double LeavesUtilized { get; set; }
        [Display(Name = "Planned Leaves")]
        public double PlannedLeaves { get; set; }
        [Display(Name = "Unplanned Leaves")]
        public double UnplannedLeaves { get; set; }
        [Display(Name = "Leaves Pending")]
        public double LeavesBalance { get; set; }
        public int is_reporting_exist { get; set; }
        public int is_hr { get; set; }
    }

    public class EmployeeCompOffReportVm
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Leave Year")]
        [DisplayFormat(DataFormatString = "{0:yyyy}")]
        public string LeaveYear { get; set; }
        [Display(Name = "Emp Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Name")]
        public string Name { get; set; }
        [Display(Name = "Reporting Manager")]
        public string ReportingManager { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Department")]
        public string Department { get; set; }
        [Display(Name = "Location")]
        public string Location { get; set; }
        [Display(Name = "Comp Off For Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? CompOffForDate { get; set; }
        [Display(Name = "Approval Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? ApprovalDate { get; set; }
        [Display(Name = "Expiry Date")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public DateTime? ExpiryDate { get; set; }
        [Display(Name = "Utilized")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}")]
        public string Utilized { get; set; }
        public int is_reporting_exist { get; set; }
        public int is_hr { get; set; }
    }
}
