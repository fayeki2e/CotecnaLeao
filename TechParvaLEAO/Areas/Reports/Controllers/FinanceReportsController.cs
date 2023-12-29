using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DinkToPdf.Contracts;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Reports.Models;
using TechParvaLEAO.Areas.Reports.Services;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using TechParvaLEAO.Services;
using X.PagedList;
using static TechParvaLEAO.Areas.Organization.Controllers.EmployeesJsonController;

namespace TechParvaLEAO.Areas.Reports.Controllers
{
    [Area("Reports")]
    [Authorize(Roles = AuthorizationRoles.FINANCE + "," + AuthorizationRoles.MANAGER)]
    public class FinanceReportsController : BaseReportsController
    {
        private readonly IApplicationRepository repository;
        private readonly FinanceReportsServices financeReportsServices;
        private readonly PaymentRequestService paymentRequestServices;
        private readonly string REPORT_BASE_LOCATION = @".\XLSTemplates\FinanceReports\xlsx\";
        private readonly string REPORT_PDF_LOCATION = @"Reports/FinanceReports/";
        private readonly ApplicationDbContext context;
        private readonly PaymentRequestService paymentRequestService;
        private readonly IEmployeeServices employeeServices;
        private IApplicationRepository _context;



        public FinanceReportsController(ApplicationDbContext context, IApplicationRepository repository,
            FinanceReportsServices financeReportsServices,
            PaymentRequestService paymentRequestServices,
             IEmployeeServices employeeServices,
            IConverter converter,
            IViewRenderService viewRenderService

        )
        {
            this.context = context;
            this.repository = repository;
            this.financeReportsServices = financeReportsServices;
            this.paymentRequestServices = paymentRequestServices;
            this.pdfConverter = converter;
            this.viewRenderService = viewRenderService;
            this.employeeServices = employeeServices;
            this._context = repository;
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

        private async Task PopulateSearchDropdowns(FinanceReportSearchVm searchVm)
        {
            // ViewData["Employee"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            // var employeeSelectList = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);
            var employeeSelectList = new SelectList(await employeeServices.GetAllEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);

            ViewData["Employee"] = employeeSelectList;
            //ViewData["BusinessActivity"] = new SelectList(repository.Get<BusinessActivity>(e => e.Deactivated == false), "Id", "Name");
            //ViewData["CustomerMarket"] = new SelectList(repository.Get<CustomerMarket>(e => e.Deactivated == false), "Id", "Name");
            ViewData["Branch"] = new SelectList(await repository.GetAsync<Location>(e => e.Deactivated == false), "Id", "Name");
        }

        private void SetDefaultSearchVm(FinanceReportSearchVm searchVm)
        {
            var date = paymentRequestServices.GetFinancialYear(DateTime.Today).StartDate;
            var firstDayOfMonth = new DateTime(date.Year, date.Month, 1);

            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            if (searchVm.ApproveRejectFromDate == null && searchVm.ApproveRejectToDate == null)
            {
                searchVm.ApproveRejectFromDate = firstDayOfMonth;
                searchVm.ApproveRejectToDate = lastDayOfMonth;
            }
        }

        public async Task<IActionResult> EmployeeExpenseTracker(FinanceReportSearchVm searchVm, int? id, string download)
        {

            SetDefaultSearchVm(searchVm);
            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchEmployeeExpenseTracker(searchVm, emp_code);
            await PopulateSearchDropdowns(searchVm);
            return await GenerateReport("EmployeeExpenseTracker", model, searchVm, id, download);
            // return View();
        }

        public async Task<IActionResult> EmployeeExpenseTracker_1(FinanceReportSearchVm searchVm, int? id, string download)
        {
            // SetDefaultSearchVm(searchVm);
            // string emp_code = User.Identity.Name;
            //  var model = financeReportsServices.SearchEmployeeExpenseTracker(searchVm, emp_code);
            await PopulateSearchDropdowns(searchVm);
            //  return await GenerateReport("EmployeeExpenseTracker", model, searchVm, id, download);
            return View();
        }

        [HttpPost, ActionName("EmployeeExpenseTracker_1")]
        public List<EmployeeExpenseTrackerReportVm> EmployeeExpenseTracker_json(int? id, FinanceReportSearchVm searchVm)
        {
            try
            {



                string emp_code = User.Identity.Name;
                var model = financeReportsServices.SearchEmployeeExpenseTracker(searchVm, emp_code);
                return model.ToList();
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog("EmployeeExpenseTracker_json", ex.Message);

                string emp_code = User.Identity.Name;
                var model = financeReportsServices.SearchEmployeeExpenseTracker(searchVm, emp_code);
                return model.ToList();


            }
        }

        public async Task<IActionResult> EmployeeAdvanceTracker(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchEmployeeAdvanceTracker(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("EmployeeAdvanceTracker", model, searchVm, id, download);
            return View();
        }

        //mine
        [HttpPost, ActionName("EmployeeAdvanceTracker")]
        public List<EmployeeAdvanceTrackerReportVm> EmployeeAdvanceTracker_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchEmployeeAdvanceTracker(searchVm, emp_code);
            return model.ToList();
        }



        public async Task<IActionResult> ExpenseTransactionDetailReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchExpenseTransactionDetail(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("ExpenseTransactionDetailReport", model, searchVm, id, download);
            return View();
        }

        [HttpPost, ActionName("ExpenseTransactionDetailReport")]
        public List<ExpenseTransactionDetailReportVm> ExpenseTransactionDetailReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchExpenseTransactionDetail(searchVm, emp_code);
            return model.ToList();
        }
        public async Task<IActionResult> ExpenseTransactionSummaryReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchExpenseTransactionSummary(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("ExpenseTransactionSummaryReport", model, searchVm, id, download);
            return View();
        }

        [HttpPost, ActionName("ExpenseTransactionSummaryReport")]
        public List<ExpenseTransactionSummaryReportVm> ExpenseTransactionSummaryReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchExpenseTransactionSummary(searchVm, emp_code);
            return model.ToList();
        }
        public async Task<IActionResult> FNFPendingReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchFNFPending(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("FNFPendingReport", model, searchVm, id, download);
            return View();

        }

        [HttpPost, ActionName("FNFPendingReport")]
        public List<FNFAdvancesReportVm> FNFPendingReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchFNFPending(searchVm, emp_code);
            return model.ToList();
        }
        public async Task<IActionResult> HardcopyNotSubmittedReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            var model = financeReportsServices.SearchHardcopyNotSubmitted(searchVm);
            await PopulateSearchDropdowns(searchVm);
            return await GenerateReport("HardcopyNotSubmittedReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> RejectedAdvancesReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchRejectedAdvances(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("RejectedAdvancesReport", model, searchVm, id, download);
            return View();

        }

        [HttpPost, ActionName("RejectedAdvancesReport")]
        public List<RejectedAdvanceReportVm> RejectedAdvancesReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchRejectedAdvances(searchVm, emp_code);
            return model.ToList();
        }
        public async Task<IActionResult> RejectedExpensesReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchRejectedExpenses(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("RejectedExpensesReport", model, searchVm, id, download);

            return View();
        }
        [HttpPost, ActionName("RejectedExpensesReport")]
        public List<RejectedExpensesReportVm> RejectedExpensesReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchRejectedExpenses(searchVm, emp_code);
            return model.ToList();
        }
        public async Task<IActionResult> SettledAdvancesReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            var model = financeReportsServices.SearchSettledAdvances(searchVm);
            await PopulateSearchDropdowns(searchVm);
            return await GenerateReport("SettledAdvancesReport", model, searchVm, id, download);
        }

        public async Task<IActionResult> UnapprovedAdvanceExpensesReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchUnapprovedAdvanceExpenses(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("UnapprovedAdvanceExpensesReport", model, searchVm, id, download);
            return View();
        }
        [HttpPost, ActionName("UnapprovedAdvanceExpensesReport")]
        public List<UnapprovedAdvanceExpensesReportVm> UnapprovedAdvanceExpensesReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchUnapprovedAdvanceExpenses(searchVm, emp_code);
            return model.ToList();
        }

        public async Task<IActionResult> UnsettledAdvancesReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //if(searchVm.ApproveRejectFromDate == null && searchVm.ApproveRejectToDate != null)
            //{
            //    searchVm.ApproveRejectFromDate = paymentRequestServices.GetFinancialYear(searchVm.ApproveRejectToDate != null && searchVm.ApproveRejectToDate.HasValue ?
            //            searchVm.ApproveRejectToDate.Value : DateTime.Today)?.StartDate;
            //}

            //var model = financeReportsServices.SearchUnsettledAdvances(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("UnsettledAdvancesReport", model, searchVm, id, download);
            return View();
        }

        [HttpPost, ActionName("UnsettledAdvancesReport")]
        public List<UnsettledAdvanceReportVm> UnsettledAdvancesReport_json(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchUnsettledAdvances(searchVm, emp_code);
            return model.ToList();
        }

        public async Task<IActionResult> AdvanceTransactionDetailReport(FinanceReportSearchVm searchVm, int? id, string download)
        {
            //SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchAdvanceTransactionDetail(searchVm);
            await PopulateSearchDropdowns(searchVm);
            //return await GenerateReport("AdvanceTransactionDetailReport", model, searchVm, id, download);
            return View();
        }
        [HttpPost, ActionName("AdvanceTransactionDetailReport")]
        public List<AdvanceTransactionDetailReportVm> AdvanceTransactionDetailReport_json(FinanceReportSearchVm searchVm, int? id, string download)
        {
            string emp_code = User.Identity.Name;

            //SetDefaultSearchVm(searchVm);
            var model = financeReportsServices.SearchAdvanceTransactionDetail(searchVm, emp_code);

            //return await GenerateReport("AdvanceTransactionDetailReport", model, searchVm, id, download);
            return model.ToList();
        }

        [HttpPost, ActionName("BalancePayableOrReceivableReport1")]
        public List<BalancePayableOrReceivableReportVm> BalancePayableOrReceivableReport_json1(int? id, FinanceReportSearchVm searchVm)
        {

            string emp_code = User.Identity.Name;
            var model = financeReportsServices.SearchBalancePayableOrReceivableDetail_Emp(searchVm, emp_code);

            var model_list = new List<BalancePayableOrReceivableReportVm>();

            //foreach (var item in model)
            //{
            //    BalancePayableOrReceivableReportVm balancePayableOrReceivableReportVm = new BalancePayableOrReceivableReportVm();
            //    var employeeJson = GetEmployee_data(item.EmpId, 1);

            //    if (model_list.Where(x => x.EmpId == item.EmpId).Count() == 0)
            //    {
            //        if (employeeJson.Value.INRPendingAmount != 0)
            //        {
            //            model_list.Add(new BalancePayableOrReceivableReportVm
            //            {
            //                Id = item.Id,
            //                EmpId = item.EmpId,
            //                AccountNumber = item.AccountNumber,
            //                EmployeeCode = item.EmployeeCode,
            //                EmployeeName = item.EmployeeName,
            //                Branch = item.Branch,
            //                Balance = Convert.ToString(String.Format("{0:0.00}", employeeJson.Value.INRPendingAmount))
            //            });
            //        }
            //    }
            //}


            //await PopulateSearchDropdowns(searchVm);

            //   var employeeSelectList = new SelectList(await employeeServices.GetReportingEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);

            //ViewData["Employee"] = employeeSelectList;


            //return await GenerateReport("BalancePayableOrReceivableReport", model_list, searchVm, id, download);





            return model_list.ToList();
        }


        [HttpPost, ActionName("BalancePayableOrReceivableReport")]
        public List<BalancePayableOrReceivableReportVm> BalancePayableOrReceivableReport_json(int? id, FinanceReportSearchVm searchVm)
        {
            DataSet ds = new DataSet();

            // dt = (DataTable)financeReportsServices.GetBalancePayableOrReceivable_Report();

            ds = (DataSet)financeReportsServices.GetBalancePayableOrReceivable_Report();


            var model_list = new List<BalancePayableOrReceivableReportVm>();

            foreach (DataRow row in ds.Tables[0].Rows)
            {

                model_list.Add(new BalancePayableOrReceivableReportVm
                {
                    Id = Convert.ToInt32(row["Id"]),
                    // EmpId = item.EmpId,
                    AccountNumber = row["AccountNumber"].ToString(),
                    EmployeeCode = row["EmployeeCode"].ToString(),
                    EmployeeName = row["Name"].ToString(),
                     Branch = row["Branch"].ToString(),
                    Balance = row["Balance"].ToString()
                });

            }


            return model_list.ToList();
        }

        public async Task<IActionResult> BalancePayableOrReceivableReport(FinanceReportSearchVm searchVm, int? id, string download)
        {


            await PopulateSearchDropdowns(searchVm);
            return View();


            //  string emp_code = User.Identity.Name;
            ////SetDefaultSearchVm(searchVm);
            //var model = financeReportsServices.SearchBalancePayableOrReceivableDetail(searchVm,emp_code);

            //BalancePayableOrReceivableReportVm m1 = new BalancePayableOrReceivableReportVm();

            //var model_list = new List<BalancePayableOrReceivableReportVm>();

            //foreach (var item in model)
            //{
            //    BalancePayableOrReceivableReportVm balancePayableOrReceivableReportVm = new BalancePayableOrReceivableReportVm();
            //    var employeeJson = await GetEmployee(item.EmpId, 1);

            //   if( model_list.Where(x => x.EmpId == item.EmpId).Count() == 0)
            //    {
            //        if (employeeJson.Value.INRPendingAmount != 0)
            //        {
            //            model_list.Add(new BalancePayableOrReceivableReportVm
            //            {

            //                Id = item.Id,
            //                EmpId = item.EmpId,
            //                AccountNumber = item.AccountNumber,
            //                EmployeeCode = item.EmployeeCode,
            //                EmployeeName = item.EmployeeName,
            //                Branch = item.Branch,
            //                Balance = Convert.ToString(String.Format("{0:0.00}", employeeJson.Value.INRPendingAmount))



            //            });
            //        }
            //    }



            //}

            //await PopulateSearchDropdowns(searchVm);

            //var employeeSelectList = new SelectList(await employeeServices.GetReportingEmployeesAsync(User), "Id", "DisplayName", searchVm.Employee);

            //ViewData["Employee"] = employeeSelectList;


            //return await GenerateReport("BalancePayableOrReceivableReport", model_list, searchVm, id, download);

        }





        public async Task<ActionResult<EmployeeJson>> GetEmployee(int? id, int? currencyId)
        {
            var employee = await context.Employees
                .Include(e => e.Designation)
                .Include(e => e.Location)
                .Where(e => e.Id == id).FirstOrDefaultAsync<Employee>();
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            var currencyObj = await context.Currencies.Where(c => c.Id == currency).FirstOrDefaultAsync();

            if (employee == null)
            {
                return NotFound();
            }

            var paidAdvaces = GetPaidAdvances(employee, currencyId);
            var approvedReimbursements = GetApprovedReimbursements(employee, currencyId);
            var paidAmount = 0.0d;
            var reimbusedAmount = 0.0d;
            foreach (var advance in paidAdvaces)
            {
                paidAmount += advance.PaidAmount;
            }
            foreach (var reimb in approvedReimbursements)
            {
                reimbusedAmount += (reimb.Amount - reimb.PaidAmount);
            }
            EmployeeJson employeeJson = new EmployeeJson
            {
                Id = employee.Id,
                EmployeeCode = employee.EmployeeCode,
                Name = employee.Name,
                Designation = employee.Designation.Name,
                Location = employee.Location.Name,
                BasicSalary = await employeeServices.GetBasicSalary(employee.Id, DateTime.Today),
                INRAdvanceReceived = paidAmount,
                INRReimbursementApproved = reimbusedAmount,
                INRPendingAmount = paidAmount - reimbusedAmount,
                CanHoldCreditCard = employee.CanHoldCreditCard,
                CurrencyCode = currencyObj.Name
            };
            return employeeJson;
        }


        public ActionResult<EmployeeJson> GetEmployee_data(int? id, int? currencyId)
        {

            //  Employee employee = new Employee();

            var employee = new List<Employee>();


            employee = (List<Employee>)_context.Get<Employee>(p => p.Id == id);

            var currencyObj = new List<Currency>();


            var currency = currencyId.HasValue ? currencyId.Value : 1;
            //var currencyObj =  context.Currencies.Where(c => c.Id == currency).FirstOrDefaultAsync();



            currencyObj = (List<Currency>)_context.Get<Currency>(c => c.Id == currency);



            if (employee == null)
            {
                return NotFound();
            }



            //PaymentRequest paidAdvaces = new PaymentRequest();

            //paidAdvaces = (PaymentRequest)_context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
            //p.Type == PaymentRequestType.ADVANCE.ToString() &&
            // p.CurrencyId == currency &&
            // (p.Status == PaymentRequestStatus.PAID.ToString()));





            var paidAdvaces = GetPaidAdvances_data(employee[0], currencyId);
            var approvedReimbursements = GetApprovedReimbursements(employee[0], currencyId);
            var paidAmount = 0.0d;
            var reimbusedAmount = 0.0d;
            foreach (var advance in paidAdvaces)
            {
                paidAmount += advance.PaidAmount;
            }
            foreach (var reimb in approvedReimbursements)
            {
                reimbusedAmount += (reimb.Amount - reimb.PaidAmount);
            }

            Employee employee1 = new Employee();

            employee1 = employee[0];

            Currency currency1 = new Currency();

            currency1 = currencyObj[0];

            EmployeeJson employeeJson = new EmployeeJson
            {
                Id = employee1.Id,
                EmployeeCode = employee1.EmployeeCode,
                Name = employee1.Name,
                Designation = employee1.Designation.Name,
                Location = employee1.Location.Name,
                BasicSalary = employeeServices.GetBasicSalary_data(employee1.Id, DateTime.Today),
                INRAdvanceReceived = paidAmount,
                INRReimbursementApproved = reimbusedAmount,
                INRPendingAmount = paidAmount - reimbusedAmount,
                CanHoldCreditCard = employee1.CanHoldCreditCard,
                CurrencyCode = currency1.Name
            };
            return employeeJson;
        }



        public IEnumerable<PaymentRequest> GetPaidAdvances(Employee employee, int? currencyId)
        {
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            var query = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.CurrencyId == currency &&
                (p.Status == PaymentRequestStatus.PAID.ToString()));
            return query;
        }

        public IEnumerable<PaymentRequest> GetPaidAdvances_data(Employee employee, int? currencyId)
        {
            PaymentRequest paymentRequeste = new PaymentRequest();

            var currency = currencyId.HasValue ? currencyId.Value : 1;
            //paymentRequeste = _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
            //    p.Type == PaymentRequestType.ADVANCE.ToString() &&
            //    p.CurrencyId == currency &&
            //    (p.Status == PaymentRequestStatus.PAID.ToString()));


            //paymentRequeste = (PaymentRequest)_context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
            //   p.Type == PaymentRequestType.ADVANCE.ToString() &&
            //    p.CurrencyId == currency &&
            //    (p.Status == PaymentRequestStatus.PAID.ToString()));


            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
               p.Type == PaymentRequestType.ADVANCE.ToString() &&
                p.CurrencyId == currency &&
                (p.Status == PaymentRequestStatus.PAID.ToString()));
        }


        public IEnumerable<PaymentRequest> GetApprovedReimbursements(Employee employee, int? currencyId)
        { // Removed p.Status == PaymentRequestStatus.APPROVED.ToString() ||
            var currency = currencyId.HasValue ? currencyId.Value : 1;
            return _context.Get<PaymentRequest>(p => p.EmployeeId == employee.Id &&
                p.Type == PaymentRequestType.REIMBURSEMENT.ToString() && p.CurrencyId == currency &&
                (p.Status == PaymentRequestStatus.PAID.ToString() ||
                    p.Status == PaymentRequestStatus.POSTED.ToString()));
        }







        public override async Task UpdateSearchVmNames(object searchVm)
        {
            if (searchVm is FinanceReportSearchVm)
            {
                FinanceReportSearchVm financeSearchVm = (FinanceReportSearchVm)searchVm;
                if (financeSearchVm.Branch != null)
                {
                    var result = await repository.GetByIdAsync<Location>(financeSearchVm.Branch);
                    financeSearchVm.BranchName = result.Name;
                }
                if (financeSearchVm.BusinessActivity != null)
                {
                    var result = await repository.GetByIdAsync<BusinessActivity>(financeSearchVm.BusinessActivity);
                    financeSearchVm.BusinessActivityName = result.Name;
                }
                if (financeSearchVm.CustomerMarket != null)
                {
                    var result = await repository.GetByIdAsync<CustomerMarket>(financeSearchVm.CustomerMarket);
                    financeSearchVm.CustomerMarketName = result.Name;
                }
                if (financeSearchVm.Employee != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(financeSearchVm.Employee);
                    financeSearchVm.EmployeeName = result.Name;
                }
                if (financeSearchVm.SubmittedBy != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(financeSearchVm.SubmittedBy);
                    financeSearchVm.SubmittedByName = result.Name;
                }
                if (financeSearchVm.ApproveRejectedBy != null)
                {
                    var result = await repository.GetByIdAsync<Employee>(financeSearchVm.ApproveRejectedBy);
                    financeSearchVm.ApproveRejectedByName = result.Name;
                }
            }
        }
    }
}