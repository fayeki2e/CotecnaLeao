using AutoMapper;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Handler;
using CalendarUtilities;
using System.Linq;
using TechParvaLEAO.Areas.Organization.Models;
using System.Collections.Generic;

namespace TechParvaLEAO.Areas.Attendance.Handler
{
    /*
     * Handler class for TimeSheet Reminder.
     */
    public class TimeSheetReminderHandler : BaseNotificationHandler, IRequestHandler<TimeSheetReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly TimeSheetServices _timeSheetServices;
        private readonly IMapper _mapper;

        public TimeSheetReminderHandler(IApplicationRepository repository, IMapper mapper,
            ApplicationDbContext dbContext,
            TimeSheetServices timeSheetServices,
            IMediator mediator
            )
        {
            _repository = repository;
            _mapper = mapper;
            _dbContext = dbContext;
            _timeSheetServices = timeSheetServices;
            _mediator = mediator;
        }

        private class MonthRange
        {
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }

        public async Task<bool> Handle(TimeSheetReminderViewModel TimeSheetReminderVm, CancellationToken cancellationToken)
        {
            var date = DateTime.Now.AddDays(-30);
            //if (date.Day >= 12 && date.Day < 15) return true;
            var firstDateOfMonth = date.GetFirstDayOfMonth();
            var lastDateOfMonth = date.GetLastDayOfMonth();

            var timesheets = _dbContext.TimeSheets.Where(t => t.StartDate >= firstDateOfMonth && t.EndDate <= lastDateOfMonth &&
                    t.Status == TimesheetStatus.PENDING.ToString());

            Dictionary<Employee, List<TimeSheet>> reminders = new Dictionary<Employee, List<TimeSheet>>();

            foreach (var timesheet in timesheets)
            {
                if (timesheet.Employee.ReportingTo != null)
                {
                    var ts = reminders.GetOrCreate(timesheet.Employee.ReportingTo);
                    ts.Add(timesheet);

                }
            }

            foreach (KeyValuePair<Employee, List<TimeSheet>> item in reminders)
            {
                await SendNotification(EmailNotification.TYPE_TIMESHEET,
                    EmailNotification.STATUS_TIMESHEET_REMINDER,
                    typeof(TimeSheet),
                    item.Value.Select((p) => p.Id).ToList(),
                    item.Key, firstDateOfMonth, lastDateOfMonth);
            }
            return true;
        }

    }
}
