using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Leave.Models;
using TechParvaLEAO.Data;
using AutoMapper;
using TechParvaLEAO.Authorization;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Controllers;
using MediatR;
using System;
using TechParvaLEAO.Areas.Organization.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
     * Controller for Leave Calculation
     */
    public class DateFormatConverter : IsoDateTimeConverter
    {
        public DateFormatConverter(string format)
        {
            DateTimeFormat = format;
        }
    }
    public class LeaveCalculationJson
    {
        public double NumberOfDays { get; set; }
        [JsonConverter(typeof(DateFormatConverter), "yyyy-MM-dd")]
        public DateTime EndDate { get; set; }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class LeaveCalculationController : BaseDefaultController
    {
        private readonly IApplicationRepository repository;
        private readonly IEmployeeServices employeeServices;
        private readonly IMediator mediator;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly IMapper mapper;
        private readonly LocationWorkdaysService locationWorkdaysService;

        public LeaveCalculationController(IApplicationRepository repository,
            IMapper mapper,
            IMediator mediator,
            IEmployeeServices employeeServices,
            LeaveRequestServices leaveRequestServices,
            LocationWorkdaysService locationWorkdaysService)
        {
            this.repository = repository;
            this.mediator = mediator;
            this.mapper = mapper;
            this.employeeServices = employeeServices;
            this.leaveRequestServices = leaveRequestServices;
            this.locationWorkdaysService = locationWorkdaysService;
        }

        [HttpGet]
        public ActionResult<LeaveCalculationJson> GetDetails(DateTime StartDate, DateTime EndDate, int EmployeeId, int LeaveTypeId, bool HalfDayStart, bool HalfDayEnd)
        {
            var employee = repository.GetById<Employee>(EmployeeId);
            if (employee != null)
            {
                if (LeaveTypeId == 4)
                {
                    EndDate = StartDate.AddDays(180);
                    return new LeaveCalculationJson { EndDate = EndDate, NumberOfDays = 180 };
                }
                var days = leaveRequestServices.GetNumberOfLeaveDays(
                    employee, StartDate, EndDate, HalfDayStart, HalfDayEnd, LeaveTypeId);
                return new LeaveCalculationJson { EndDate = EndDate, NumberOfDays = days };
            }
            else
            {
                return new LeaveCalculationJson { EndDate = EndDate, NumberOfDays = 0 }; ;
            }
        }
    }
}