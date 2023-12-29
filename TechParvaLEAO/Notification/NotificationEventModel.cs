using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Notification
{
    public class NotificationEventModel : INotification
    {
        public string Type { get; set; }
        public string Event { get; set; }
        public Type ModelType { get; set; }
        public int ObjectId { get; set; }
        public int? ActionById { get; set; }
        public int? NextActionById { get; set; }
        public IEnumerable<int> ObjectIds { get; set; } = new List<int>();
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public int? EmployeeId { get; set; }
    }
}
