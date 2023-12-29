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
     * Model for Designation
     */
    public partial class Designation : Entity<int>
    {
        public Designation()
        {

        }
        [Display(Name = "Designation")]
        public string Name { get; set; }

        public bool Deactivated { get; set; } = false;

    }
}