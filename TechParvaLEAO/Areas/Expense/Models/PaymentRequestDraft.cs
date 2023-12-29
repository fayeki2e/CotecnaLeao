using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Areas.Expense.Models
{
    /*
     * Model for Payment Request Draft
     */
    public class PaymentRequestDraft : Entity<int>
    {
        public string UserIdentity { get; set; }

        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy HH:mm}", ApplyFormatInEditMode = false)]
        [Display(Name = "Last Updated On")]
        public DateTime LastUpdatedOn { get; set; }

        [Display(Name = "Employee")]
        [ForeignKey("Employee")]
        public virtual int? EmployeeId { get; set; }
        public virtual Employee Employee { get; set; }
        public string FormData { get; set; }
        public string Type { get; set; }
        public string DraftId { get; set; }
    }
}
