using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Areas.Attendance.Models
{
    /*
     * Model for Client
     */
    public partial class Client: Entity<int>
    {
        public Client()
        {
        }
        public string Name { get; set; }

        public bool Deactivated { get; set; } = false;

    }
}