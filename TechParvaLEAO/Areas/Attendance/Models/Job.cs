using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * Model for Job
     */
    public partial class Job: Entity<int>
    {
        public Job()
        {
        }
        public string Name { get; set; }
        [Display(Name = "Client")]
        [ForeignKey("Client")]
        public virtual int ClientId { get; set; }
        public virtual Client Client { get; set; }

        public bool Deactivated { get; set; } = false;
    }
}