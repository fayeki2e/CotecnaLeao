using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Report;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Areas.Reports.Services;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using X.PagedList;

namespace TechParvaLEAO.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize(Roles = AuthorizationRoles.HR)]
    public class TimesheetReportsController : BaseReportsController
    {
        private readonly IApplicationRepository repository;
        private readonly TimesheetReportsServices timesheetReportsServices;
        private readonly IEmployeeServices employeeServices;
        private readonly ApplicationDbContext context;
        private readonly ReportsOptions reportsOptions;
        private readonly string REPORT_BASE_LOCATION = @".\XLSTemplates\TimesheetReports\xlsx\";
        private readonly string REPORT_PDF_LOCATION = @"Reports/TimesheetReports/";

        public TimesheetReportsController(
            ApplicationDbContext context,
            TimesheetReportsServices timesheetReportsServices,
            IEmployeeServices employeeServices,
            IOptions<ReportsOptions> options,
            IConverter converter,
            IApplicationRepository repository)
        {
            this.context = context;
            this.timesheetReportsServices = timesheetReportsServices;
            this.employeeServices = employeeServices;
            this.reportsOptions = options.Value;
            this.pdfConverter = converter;
            this.repository = repository;
        }

        public override string GetXsltTemplatePath()
        {
            return REPORT_BASE_LOCATION;
        }

        public override string GetPdfTemplatePath()
        {
            return REPORT_PDF_LOCATION;
        }

        public IActionResult Index()
        {
            return View();
        }

        private async Task PopulateSearchDropdowns(TimesheetReportSearchVm searchVm)
        {
            ViewData["Employee"] = new SelectList(await employeeServices.GetAllOnFieldEmployeeForTimesheet(), "Id", "DisplayName", searchVm.Employee);
            ViewData["Location"] = new SelectList(await context.Locations.Where(e => e.Deactivated == false).ToListAsync(), "Id", "Name", searchVm.Location);
        }


        public async Task<IActionResult> TimesheetEntriesReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchTimesheetEntries(searchVm);
            return await GenerateReport("TimesheetEntriesReport", model, searchVm, id, download);

        }

        public async Task<IActionResult> LocationOvertimeReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchLocationOvertime(searchVm);
            return await GenerateReport("LocationOvertimeReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> LocationOvertimePaymentReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchLocationOvertimePayment(searchVm);
            return await GenerateReport("LocationOvertimePaymentReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> EmployeeWeeklyWorkedHoursReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchEmployeeWeeklyWorkedHours(searchVm);
            return await GenerateReport("EmployeeWeeklyWorkedHoursReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> EmployeeWeeklyOTReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchEmployeeWeeklyOT(searchVm);
            return await GenerateReport("EmployeeWeeklyOTReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> TimesheetPrintReport(TimesheetReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            await PopulateSearchDropdowns(searchVm);
            var model = timesheetReportsServices.SearchTimesheetPrint(searchVm);
            return await GenerateReport("TimesheetPrintReport", model, searchVm, id, download);
        }

        public override async Task UpdateSearchVmNames(object searchVm)
        {
            if (searchVm is TimesheetReportSearchVm)
            {
                TimesheetReportSearchVm tsSearchVm = (TimesheetReportSearchVm)searchVm;
                if (tsSearchVm.Location != null)
                {
                    var result = await repository.GetByIdAsync<Location>(tsSearchVm.Location);
                    tsSearchVm.LocationName = result.Name;
                }
                if (tsSearchVm.Employee != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(tsSearchVm.Employee);
                    tsSearchVm.EmployeeName = result.Name;
                }
            }
        }
    }
}