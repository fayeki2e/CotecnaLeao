using CsvHelper.Configuration.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    public class BasicSalaryVm
    {
        [Name("Employee Code")]
        public string EmployeeCode { get; set; }
        [Name("From Date")]
        public DateTime FromDate { get; set; }
        [Name("To Date")]
        public DateTime? ToDate { get; set; }
        public double Salary { get; set; }
    }

    public class BasicSalaryDataImportResult
    {
        public BasicSalaryDataImportResult(){}
        public BasicSalaryDataImportResult(BasicSalaryVm record)
        {
            this.EmployeeCode = record.EmployeeCode;
            this.FromDate = record.FromDate;
            this.ToDate = record.ToDate;
            this.Salary = record.Salary;
        }

        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; }
        [Display(Name = "Employee Code")]
        [Name("From Date")]
        public DateTime FromDate { get; set; }
        [Name("To Date")]
        public DateTime? ToDate { get; set; }
        public double Salary { get; set; }
        public string Result { get; set; }

    }

}
