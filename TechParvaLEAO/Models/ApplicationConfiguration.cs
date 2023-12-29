using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Techparva.GenericRepository;

namespace TechParvaLEAO.Models
{
    public class ApplicationConfiguration : Entity<int>
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Datatype { get; set; }
    }
}
