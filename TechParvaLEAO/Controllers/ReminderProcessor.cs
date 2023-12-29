using System;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using TechParvaLEAO.Areas.Attendance.Models;
using System.Net.Http;

namespace TechParvaLEAO.Controllers
{
    public sealed class ReminderProcessor
    {
        static readonly HttpClient client = new HttpClient();
        public async static Task Process(String operation)
        {
            var response = await client.GetAsync(string.Format("http://localhost:44320/DataImport/Reminder/?operation={0}", operation));
            Console.WriteLine("Processed {}", operation);
        }
    }
}
