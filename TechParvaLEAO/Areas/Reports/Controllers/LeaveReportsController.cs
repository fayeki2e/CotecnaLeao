using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Report;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TechParvaLEAO.Areas.Leave.Models;
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
    [Authorize(Roles = AuthorizationRoles.HR + "," + AuthorizationRoles.MANAGER) ]
    
    public class LeaveReportsController : BaseReportsController
    {
        private readonly IApplicationRepository repository;
        private readonly LeaveReportsServices leaveReportsServices;
        private readonly IEmployeeServices employeeServices;
        private readonly ApplicationDbContext dbContext;
        private readonly string REPORT_BASE_LOCATION = @".\XLSTemplates\LeaveReports\xlsx\";
        private readonly string REPORT_PDF_LOCATION = @"Reports/LeaveReports/";
        public LeaveReportsController(ApplicationDbContext dbContext, 
            LeaveReportsServices leaveReportsServices,
            IEmployeeServices employeeServices,
            IConverter converter,
            IApplicationRepository repository

            )
        {
            this.leaveReportsServices = leaveReportsServices;
            this.dbContext = dbContext;
            this.employeeServices = employeeServices;
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

        private async Task PopulateSearchDropdowns(LeaveReportSearchVm searchVm)
        {

            if (User.IsInRole(AuthorizationRoles.HR))
            {

                var employeeSelectList_allemp = new SelectList(await employeeServices.GetAllEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);
                ViewData["Employee"] = employeeSelectList_allemp;
            }
            else
            {
                var employeeSelectList = new SelectList(await employeeServices.GetReportingEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);
                ViewData["Employee"] = employeeSelectList;
            }

          



            //ViewData["ReportingManager"] = employeeSelectList;
            ViewData["ReportingManager"] = new SelectList(employeeServices.GetReportingManager(), "Id", "DisplayName");

            ViewData["Branch"] = new SelectList(await dbContext.Locations.ToListAsync(), "Id", "Name", searchVm.Branch);
            ViewData["LeaveYear"] = new SelectList(await dbContext.LeaveAccountingPeriods.ToListAsync(), "Id", "Name", searchVm.LeaveYear);
            ViewData["LeaveType"] = new SelectList(await dbContext.LeaveTypes.ToListAsync(), "Id", "Name", searchVm.LeaveType);
            ViewData["LeaveStatus"] = new SelectList(new[] { new { Id = "APPROVED", Name = "APPROVED" },
                                                           new { Id = "REJECTED", Name = "REJECTED" },
                                                           new { Id = "PENDING", Name = "PENDING" },
                                                           new { Id = "CANCELED", Name = "CANCELLED" },
                                                        }, "Id", "Name", searchVm.LeaveStatus==null?"APPROVED": searchVm.LeaveStatus);

           

        }

        public async Task<IActionResult> CarryForwardLeavesReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
           // string emp_code = User.Identity.Name;

           //// CarryForwardLeavesReportVm model = new CarryForwardLeavesReportVm();

           // var model = leaveReportsServices.SearchCarryForwardLeaves(searchVm, emp_code);
           // if(model.ToList().Count == 0)
           // {
           //     ViewData["is_reporting_exist"] = 1;
           // }
           // else
           // {
           //     ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
           // }
           
         
            await PopulateSearchDropdowns(searchVm);
            //  return await GenerateReport("CarryForwardLeavesReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("CarryForwardLeavesReport")]
        public List<CarryForwardLeavesReportVm> CarryForwardLeavesReport_json(int? id, LeaveReportSearchVm searchVm)
        {
   
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.SearchCarryForwardLeaves(searchVm, emp_code,User);
            return model.ToList();
        }


        public async Task<IActionResult> DateWiseLeaveReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchDateWiseLeave(searchVm, emp_code);

            //ViewData["is_reporting_exist"] =model.ToList()[0].is_reporting_exist;

            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("DateWiseLeaveReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("DateWiseLeaveReport")]
        public List<DateWiseLeaveReportVm> DateWiseLeaveReport_json(int? id, LeaveReportSearchVm searchVm)
        {
         //   var test = JsonConvert.DeserializeObject(svm.BranchName);

          //  LeaveReportSearchVm searchVm = new LeaveReportSearchVm();
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.SearchDateWiseLeave(searchVm, emp_code,User);
            return model.ToList();
        }


        public async Task<IActionResult> TrendReport1(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchDateWiseLeave(searchVm, emp_code);

            //ViewData["is_reporting_exist"] =model.ToList()[0].is_reporting_exist;

            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("DateWiseLeaveReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("TrendReport1")]
        public List<DateWiseLeaveReportVm> TrendReport1_json(int? id, LeaveReportSearchVm searchVm)
        {
            //   var test = JsonConvert.DeserializeObject(svm.BranchName);

            //  LeaveReportSearchVm searchVm = new LeaveReportSearchVm();
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.Searchtrendreport(searchVm, emp_code,User);
            return model.ToList();
        }

        public async Task<IActionResult> TrendReport2(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchDateWiseLeave(searchVm, emp_code);

            //ViewData["is_reporting_exist"] =model.ToList()[0].is_reporting_exist;

            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("DateWiseLeaveReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("TrendReport2")]
        public List<DateWiseLeaveReportVm> TrendReport2_json(int? id, LeaveReportSearchVm searchVm)
        {
            //   var test = JsonConvert.DeserializeObject(svm.BranchName);

            //  LeaveReportSearchVm searchVm = new LeaveReportSearchVm();
            string emp_code = User.Identity.Name;

            searchVm.ReportType = "trendreport2";

            var model = leaveReportsServices.Searchtrendreport(searchVm, emp_code,User);
            return model.ToList();
        }

        public async Task<IActionResult> TrendReport3(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchDateWiseLeave(searchVm, emp_code);

            //ViewData["is_reporting_exist"] =model.ToList()[0].is_reporting_exist;

            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("DateWiseLeaveReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("TrendReport3")]
        public List<DateWiseLeaveReportVm> TrendReport3_json(int? id, LeaveReportSearchVm searchVm)
        {
            //   var test = JsonConvert.DeserializeObject(svm.BranchName);

            //  LeaveReportSearchVm searchVm = new LeaveReportSearchVm();
            string emp_code = User.Identity.Name;
            searchVm.ReportType = "trendreport3";
            var model = leaveReportsServices.Searchtrendreport(searchVm, emp_code,User);
            return model.ToList();
        }


        public async Task<IActionResult> EmployeeCompOffReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;

            //var model = leaveReportsServices.SearchEmployeeCompOff(searchVm, emp_code);

            //if (model.ToList().Count ==0)
            //{
            //    ViewData["is_reporting_exist"] = 0;
            //}
            //else
            //{
            //    ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
            //}
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("EmployeeCompOffReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("EmployeeCompOffReport")]
        public   List<EmployeeCompOffReportVm> EmployeeCompOffReport_json(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            string emp_code = User.Identity.Name;

            var model = leaveReportsServices.SearchEmployeeCompOff(searchVm, emp_code,User);

            if (model.ToList().Count == 0)
            {
                ViewData["is_reporting_exist"] = 0;
            }
            else
            {
                ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
            }
          //  await PopulateSearchDropdowns(searchVm);
           // return await GenerateReport("EmployeeCompOffReport", model, searchVm, id, download);
            return model.ToList();

        }

        public async Task<IActionResult> JoinedEmployeeLeaveReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchJoinedEmployeeLeaveCredit(searchVm, emp_code);
            //ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("JoinedEmployeeLeaveReport", model, searchVm, id, download);

            return View();
        }


        [HttpPost, ActionName("JoinedEmployeeLeaveReport")]
        public List<JoinedSeperatedEmployeeLeaveReportVm> JoinedEmployeeLeaveReport_json(int? id, LeaveReportSearchVm searchVm)
        {
          
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.SearchJoinedEmployeeLeaveCredit(searchVm, emp_code,User);
            return model.ToList();
        }



        public async Task<IActionResult> PaidLeaveBalanceReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchPaidLeaveBalance(searchVm, emp_code);
            //ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("PaidLeaveBalanceReport", model, searchVm, id, download);

            return View();
        }
        [HttpPost, ActionName("PaidLeaveBalanceReport")]
        public  List<PaidLeaveBalanceReportVm> PaidLeaveBalanceReport_json(int? id, LeaveReportSearchVm searchVm)
        {
           
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.SearchPaidLeaveBalance(searchVm, emp_code,User);    
            return model.ToList();
        }

         

        public async Task<IActionResult> SeperatedEmployeeLeaveReport(LeaveReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            string emp_code = User.Identity.Name;
            //var model = leaveReportsServices.SearchSeperatedEmployeeLeaveBalance(searchVm, emp_code);
            //ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;
            await PopulateSearchDropdowns(searchVm);
            // return await GenerateReport("SeperatedEmployeeLeaveReport", model, searchVm, id, download);

            return View();
        }

        [HttpPost, ActionName("SeperatedEmployeeLeaveReport")]
        public List<JoinedSeperatedEmployeeLeaveReportVm> SeperatedEmployeeLeaveReport_json(int? id, LeaveReportSearchVm searchVm)
        {
        
            string emp_code = User.Identity.Name;
            var model = leaveReportsServices.SearchSeperatedEmployeeLeaveBalance(searchVm, emp_code,User);
            //ViewData["is_reporting_exist"] = model.ToList()[0].is_reporting_exist;

            return model.ToList();
        }


        public override async Task UpdateSearchVmNames(object searchVm)
        {
            if (searchVm is LeaveReportSearchVm)
            {
                LeaveReportSearchVm leaveSearchVm = (LeaveReportSearchVm)searchVm;
                if (leaveSearchVm.Branch != null)
                {
                    var result = await repository.GetByIdAsync<Location>(leaveSearchVm.Branch);
                    leaveSearchVm.BranchName = result.Name;
                }
                if (leaveSearchVm.LeaveType != null)
                {
                    var result = await repository.GetByIdAsync<LeaveType>(leaveSearchVm.LeaveType);
                    leaveSearchVm.LeaveTypeName = result.Name;
                }
                if (leaveSearchVm.Employee != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(leaveSearchVm.Employee);
                    leaveSearchVm.EmployeeName = result.Name;
                }
                if (leaveSearchVm.ReportingManager != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(leaveSearchVm.ReportingManager);
                    leaveSearchVm.ReportingManagerName = result.Name;
                }
            }
        }
    }
}
