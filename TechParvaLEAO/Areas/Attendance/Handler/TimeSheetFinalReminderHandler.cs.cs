using AutoMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Data;
using TechParvaLEAO.Handler;
using TechParvaLEAO.Notification;
using CalendarUtilities;
using TechParvaLEAO.Areas.Organization.Models;

namespace TechParvaLEAO.Areas.Attendance.Handler
{
    /*
     * Handler class for TimeSheet Final Reminder.
     */
    public class TimeSheetFinalReminderHandler : BaseNotificationHandler, IRequestHandler<TimeSheetFinalReminderViewModel, bool>
    {
        private readonly IApplicationRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly TimeSheetServices _timeSheetServices;
        private readonly IMapper _mapper;

        public TimeSheetFinalReminderHandler(IApplicationRepository repository, IMapper mapper,
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


        public async Task<bool> Handle(TimeSheetFinalReminderViewModel TimeSheetReminderVm, CancellationToken cancellationToken)
        {
            var date = DateTime.Now.AddDays(-30);
            //if (date.Day != 15) return true;
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
                    EmailNotification.STATUS_TIMESHEET_REMINDER_FINAL,
                    typeof(TimeSheet),
                    item.Value.Select((p) => p.Id).ToList(),
                    item.Key, firstDateOfMonth, lastDateOfMonth);
            }
            return true;
        }

    }
}

