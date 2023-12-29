using AutoMapper;
using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Areas.Leave.Handler;
using MediatR;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Services;

namespace TechParvaLEAO.Notification
{
    public class HangfireReminderSender
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IMediator _mediatR;

        public HangfireReminderSender(ApplicationDbContext context, IMapper mapper,
            UserManager<ApplicationUser> userManager,
            IMediator mediatR)
        {
            try
            {


                this._context = context;
                this._mapper = mapper;
                this._userManager = userManager;
                this._mediatR = mediatR;
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog(" HangfireReminderSender - HangfireReminderSender", ex.Message);
            }
        }

        public async Task ProcessRemindersAsync(String operation)
        {
            if (string.Equals(operation, "LeaveReminder"))
            {
                LeaveReminderViewModel vm = new LeaveReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }

            else if (string.Equals(operation, "LeaveFinalReminder"))
            {
                LeaveFinalReminderViewModel vm = new LeaveFinalReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }

            else if (string.Equals(operation, "AdvanceReminder"))
            {
                AdvanceReminderViewModel vm = new AdvanceReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "AdvanceFinalReminder"))
            {
                AdvanceFianlReminderViewModel vm = new AdvanceFianlReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "AdvanceReminderMoreThanThreeDays"))
            {
                AdvanceReminderMoreThanThreeDaysViewModel vm = new AdvanceReminderMoreThanThreeDaysViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "FinanceAdvanceReminder"))
            {
                FinanceAdvanceReminderViewModel vm = new FinanceAdvanceReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            


            else if (string.Equals(operation, "ExpenseReminder"))
            {
                try
                {
                    ExpenseReminderViewModel vm = new ExpenseReminderViewModel
                    {
                        ForDate = DateTime.Today
                    };
                    await _mediatR.Send(vm);
                }
                catch (Exception ex)
                {
                    LogWriter.WriteLog("HangfireReminderSender - ExpenseReminder", ex.Message);
                }
            }
            else if (string.Equals(operation, "ExpenseFinalReminder"))
            {
                ExpenseFinalReminderViewModel vm = new ExpenseFinalReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "FinanceDocumentsPendingReminder"))
            {
                DocumentSubmissionReminderViewModel vm = new DocumentSubmissionReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
            else if (string.Equals(operation, "FinanceExpenseReminder"))
            {
                FinanceExpenseReminderViewModel vm = new FinanceExpenseReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }

            else if (string.Equals(operation, "TimeSheetReminder"))
            {
                TimeSheetReminderViewModel vm = new TimeSheetReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }

            else if (string.Equals(operation, "TimeSheetFinalReminder"))
            {
                TimeSheetFinalReminderViewModel vm = new TimeSheetFinalReminderViewModel
                {
                    ForDate = DateTime.Today
                };
                await _mediatR.Send(vm);
            }
        }
    }
}
