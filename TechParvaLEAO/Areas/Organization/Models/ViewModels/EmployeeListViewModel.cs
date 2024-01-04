using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    /*
     * View Model for Employee Search
     */
    public class EmployeeListViewModel
    {
        public int Id { get; set; }
        public string name { get; set; }
        public string employeeCode { get; set; }
        public string Designation { get; set; }
        public string status { get; set; }
        public string email { get; set; }
        public string accountNumber { get; set; }
        public string dateOfJoining { get; set; }
        public string dateOfBirth { get; set; }
        public string expenseProfile { get; set; }
        public string gender { get; set; }
        public string location { get; set; }
        public string reportingTo { get; set; }
        public string teamlist { get; set; }
        public string teams { get; set; }
        public string authorizationProfile { get; set; }
        public string roles { get; set; }
        public string created_by { get; set; }
        public string created_Date { get; set; }
        public string modified_by { get; set; }
        public string modified_Date { get; set; }

        public string overtimerule { get; set; }
        public string canApplyMissionLeaves { get; set; }
        public string canCreateForexRequests { get; set; }
        public string canHoldCreditCard { get; set; }
        public string isHr { get; set; }
        public string onFieldEmployee { get; set; }
        public string specificWeeklyOff { get; set; }
    }
}
