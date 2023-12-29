using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Handler
{
    public abstract class BaseNotificationHandler 
    {
        protected IMediator _mediator;
        public async Task SendNotification(string Type, string Event, Type ModelType, int ObjectId)
        {
            var notification = new NotificationEventModel
            {
                Type = Type,
                Event = Event,
                ModelType = ModelType,
                ObjectId = ObjectId
            };
            await _mediator.Publish<NotificationEventModel>(notification);
        }
        public async Task SendNotification(string Type, string Event, Type ModelType, IList<int> ObjectIds)
        {
            var notification = new NotificationEventModel
            {
                Type = Type,
                Event = Event,
                ModelType = ModelType,
                ObjectIds = ObjectIds
            };
            await _mediator.Publish<NotificationEventModel>(notification);
        }
        public async Task SendNotification(string Type, string Event, Type ModelType, IList<int> ObjectIds, 
            Employee Receiver, DateTime FromDate, DateTime ToDate)
        {
            var notification = new NotificationEventModel
            {
                Type = Type,
                Event = Event,
                ModelType = ModelType,
                ObjectIds = ObjectIds,
                FromDate = FromDate,
                ToDate = ToDate,
                EmployeeId = Receiver.Id
            };
            await _mediator.Publish<NotificationEventModel>(notification);
        }
    }
}
