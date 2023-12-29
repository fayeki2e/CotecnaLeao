using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
    * Model for Financial Year
    */
    public partial class FinancialYear: Entity<int>
    {
        public FinancialYear()
        {
        }

        public string Code { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = false)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}