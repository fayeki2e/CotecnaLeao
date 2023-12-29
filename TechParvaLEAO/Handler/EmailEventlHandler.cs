using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Services;
using Microsoft.Extensions.Options;
using TechParvaLEAO.Notification;
using Hangfire;

namespace TechParvaLEAO.Areas.Expense.Handler
{
    public class EmailEventlHandler : INotificationHandler<NotificationEventModel>
    {

        public EmailEventlHandler()
        {
        }

        public async Task Handle(NotificationEventModel notification, CancellationToken cancellationToken)
        {
            await Task.Run(()=>
            BackgroundJob.Enqueue<HangfireEmailSender>(x => x.SendEmailNotification(notification)));
        }

    }
}
