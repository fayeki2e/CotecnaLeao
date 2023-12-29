using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
      * Model for TimeSheet Email Model
      */
    public class TimeSheetEmailModel
    {

        public TimeSheetEmailModel()
        {
            TimeSheetDetails = new List<TimeSheetDetails>();
        }

        public int Id { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Employee { get; set; }
        public string ReportingManager { get; set; }
        public string Location { get; set; }
        public string EmployeeCode { get; set; }
        public DateTime CreatdeDate { get; set; }
        public string CoordinatorName { get; set; }
        public string HR { get; set; } = "HR";
        public int NumberOfDays { get; set; }

        public virtual IList<TimeSheetDetails> TimeSheetDetails { get; set; }
    }

    public class TimeSheetDetails
    {
        public string EmployeeCode { get; set; }
        public string Employee { get; set; }
    }
}
