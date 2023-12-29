using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Organization.Models
{
    /*
     * Model for Overtime Rule
     */
    public partial class OvertimeRule: Entity<int>
    {
        public OvertimeRule()
        {
        }

        [Display(Name = "Rule Name")]
        public string Name { get; set; }

        [Display(Name = "Overtime Multiplier")]
        public double OvertimeMultiplier { get; set; }
        public bool Deactivated { get; set; } = false;


    }
}
