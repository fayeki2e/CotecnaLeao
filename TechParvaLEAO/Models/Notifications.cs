using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TechParvaLEAO.Models
{
    public class EmailNotificationConfiguration
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public string TemplatePathText { get; set; }
        public string TemplatePathHtml { get; set; }
        public string SubjectLine { get; set; }
        public string Receiver { get; set; }
    }
}
