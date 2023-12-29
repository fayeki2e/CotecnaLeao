using System;

namespace TechParvaLEAO.Areas.Leave.Models.ViewModels
{
    /*
    * View Model for Calculate Leave Days
    */
    public class CalculateLeaveDaysVO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public double NumberOfDays { get; set; }
        public string Employeelocation { get; set; }

    }
}
