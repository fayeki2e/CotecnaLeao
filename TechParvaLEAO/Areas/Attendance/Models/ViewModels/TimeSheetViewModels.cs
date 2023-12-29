using System;
using MediatR;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using UoN.ExpressiveAnnotations.NetCore.Attributes;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * View model for each employee timesheet
     */
    public class EmployeeTimeSheetMonth
    {
        public EmployeeTimeSheetMonth()
        {
            TimeSheetWeeks = new List<TimeSheetWeek>();

        }
        [Display(Name = "Employee")]
        public string Employee { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        [Display(Name = "Employee Id")]
        public int EmployeeId { get; set; }
        public string EmployeeCode { get; set; }
        [Display(Name = "Month")]
        [DisplayFormat(DataFormatString = "{0:MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Month { get; set; }
        public IList<TimeSheetWeek> TimeSheetWeeks { get; set; }
    }

    /*
     * View model for TimeSheetWeek
     */
    public class TimeSheetWeek
    {

        [Display(Name = "Week Number In Month")]
        public int WeekInMonth { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public int TimeSheetId { get; set; }
        public double OvertimeHours { get; set; }
        public double CompOffs { get; set; }
        public string Status { get; set; }
    }

    /*
     * View model for Week
     */
    public class Week
    {
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public int WeekInYear { get; set; }
    }

    /*
     * View model for TimeSheet Overview
     */
    public class TimeSheetOverviewViewModel
    {
        public TimeSheetOverviewViewModel()
        {
            EmployeeTimeSheetMonth = new List<EmployeeTimeSheetMonth>();
            Weeks = new List<Week>();

        }
        public IList<EmployeeTimeSheetMonth> EmployeeTimeSheetMonth { get; set; }
        public IList<Week> Weeks { get; set; }

    }

    /*
     * View model for TimeSheet
     */
    public class TimeSheetViewModel : IRequest<int>
    {
        public TimeSheetViewModel() 
        {
            TimeSheetEntries = new List<TimeSheetEntryViewModel>();
        }
        public int Id { get; set; }
        [Display(Name = "Employee")]
        public string Employee { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public string WeekNumber { get; set; }
        public int WeekInMonth { get; set; }
        public int WeekInYear { get; set; }
        public string Submit { get; set; }
        public int CreatedByEmployeeId { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MMMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EndDate { get; set; }
        public List<TimeSheetEntryViewModel> TimeSheetEntries { get; set; }
    }

    /*
     * View model for TimeSheet Edit
     */
    public class TimeSheetEditViewModel : TimeSheetViewModel
    {
        public int TimesheetId { get; set; }
    }

    /*
     * View model for TimeSheet Entry
     */
    public class TimeSheetEntryViewModel : IValidatableObject
    {
        public int TimesheetEntryId { get; set; }
        public string Weekday { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        public string JobNumber { get; set; } = "";

        public string Client { get; set; }  = "";

        public string InTime { get; set; } = "";

        public string OutTime { get; set; } = "";

        public string HoursWorked { get; set; } = "";

        public bool IsWeekOff { get; set; } = false;
        public bool IsHoliday { get; set; } = false;
        public string HolidayReason { get; set; } = "";
        public bool IsHalfDay { get; set; } = false;
        public bool IsLeave { get; set; } = false;
        public bool IsHalfDayLeave { get; set; } = false;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if ((!(IsWeekOff || IsHoliday || IsLeave ))&& string.IsNullOrEmpty(JobNumber)){
                yield return new ValidationResult("JobNumber is mandatory");
            }
            if ((!(IsWeekOff || IsHoliday || IsLeave)) && string.IsNullOrEmpty(Client))
            {
                yield return new ValidationResult("Client is mandatory");
            }
            if ((!(IsWeekOff || IsHoliday || IsLeave)) && string.IsNullOrEmpty(InTime))
            {
                yield return new ValidationResult("In Time is mandatory");
            }
            if ((!(IsWeekOff || IsHoliday || IsLeave)) && string.IsNullOrEmpty(OutTime))
            {
                yield return new ValidationResult("Out Time is mandatory");
            }

            if (IsLeave && !string.IsNullOrEmpty(JobNumber))
            {
                yield return new ValidationResult("Job Number must not be filled for employee on leave");
            }
            if (IsLeave && !string.IsNullOrEmpty(Client))
            {
                yield return new ValidationResult("This data must not be filled for employee on leave");
            }
            if (IsLeave && !string.IsNullOrEmpty(InTime))
            {
                yield return new ValidationResult("In Time must not be filled for employee on leave");
            }
            if (IsLeave && !string.IsNullOrEmpty(OutTime))
            {
                yield return new ValidationResult("Out Time must not be filled for employee on leave");
            }

        }
    }

    /*
     * View model for Base Reminder
     */
    public class BaseReminderViewModel : IRequest<bool>
    {
        [Display(Name = "For Date")]
        public DateTime ForDate { get; set; }
    }

    /*
     * View model for TimeSheet Reminder
     */
    public class TimeSheetReminderViewModel : BaseReminderViewModel { }

    /*
     * View model for TimeSheet Final Reminder
     */
    public class TimeSheetFinalReminderViewModel : BaseReminderViewModel { }

    /*
     * View model for TimeSheet Approve Reject
     */
    public class TimeSheetApproveRejectViewModel: IRequest<bool>
    {
        public int Id { get; set; }
        public int RejecttionReason { get; set; }
        public string ApproveReject { get; set; }
        public int ApprovedById { get; set; }
    }

    /*
     * View model for Employee Profile Details
     */
    public class EmployeeProfileDetailsViewModel : IRequest<bool>
    {
        public int Id { get; set; }
        [Display(Name = "Employee")]
        public string Employee { get; set; }
        [Display(Name = "Designation")]
        public string Designation { get; set; }
        public string EmployeeCode { get; set; }
        public int EmployeeId { get; set; }
        public int LocationId { get; set; }
        public string ReportingTo { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
    }
}
