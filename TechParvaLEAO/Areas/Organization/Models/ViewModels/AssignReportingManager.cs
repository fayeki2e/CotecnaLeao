using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    public class AssignReportingManagerViewModel
    {
        [Display(Name = "Old Reporting Manager")]
        public int OldReportingToId { get; set; }

        [Display(Name = "New Reporting Manager")]
        public int NewReportingToId { get; set; }
    }
}
