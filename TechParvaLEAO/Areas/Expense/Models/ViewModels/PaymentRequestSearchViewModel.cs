using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Areas.Expense.Models.ViewModels
{
    /*
  * View Model for Payment Request Search
  */
    public class PaymentRequestSearchViewModel
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
        public double? FromAmount { get; set; }
        public double? ToAmount { get; set; }
        public string RequestNumber { get; set; }
        public string Currency { get; set; }
        public string CurrencyId { get; set; }

        public bool NoSearchRequested()
        {
            return string.IsNullOrEmpty(EmployeeName) && string.IsNullOrEmpty(EmployeeCode) &&
                string.IsNullOrEmpty(Status) && !FromDate.HasValue && !ToDate.HasValue &&
                !FromAmount.HasValue && !ToAmount.HasValue && string.IsNullOrEmpty(RequestNumber) &&
                string.IsNullOrEmpty(Currency) && string.IsNullOrEmpty(CurrencyId);
        }
    }
}
