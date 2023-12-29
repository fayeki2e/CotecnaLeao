using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechParvaLEAO.Areas.Organization.Models.ViewModels
{
    /*
     * View Model for Designation Search
     */
    public class DesignationSearchVM
    {
        [Display(Name = "Designation")]
        public string Name { get; set; }
    }
}
