using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace TechParvaLEAO.Authorization
{
    public class AuthorizationRoles
    {
        /*
        public const string LOCATION_COORDINATOR = "Location Coordinator";
        public const string MANAGER = "Manager";
        public const string EMPLOYEE = "Employee";
        public const string FINANCE = "Finance";
        public const string HR = "Human Resources";
        public const string TIMESHEET = "Timesheet Coordinator";
        public const string MASTER = "Master Data Manager";
        public const string FINANCE_MASTER = "Finance Masters";
        public const string HR_MASTER = "Human Resources Masters";
        */
        public const string LOCATION_COORDINATOR = "LOCATION_COORDINATOR";
        public const string MANAGER = "MANAGER";
        public const string EMPLOYEE = "EMPLOYEE";
        public const string FINANCE = "FINANCE";
        public const string HR = "HR";
        public const string TIMESHEET = "TIMESHEET";
        public const string MASTER = "MASTER";
        public const string FINANCE_MASTER = "FINANCE_MASTER";
        public const string HR_MASTER = "HR_MASTER";

        public static string MultiAuthorizationRoles(params string[] roles)
        {
            StringBuilder stringBuilder = new StringBuilder("");
            foreach(string role in roles)
            {
                stringBuilder.Append(role);
                stringBuilder.Append(",");
            }
            stringBuilder.Remove(stringBuilder.Length, 1);
            return stringBuilder.ToString();
        }
    }
}