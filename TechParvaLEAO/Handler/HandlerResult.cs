using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Handler
{
    public class Result
    {
        public bool Success { get; set; }
        public IList<string> ErrorMessage { get; set; }
    }
}
