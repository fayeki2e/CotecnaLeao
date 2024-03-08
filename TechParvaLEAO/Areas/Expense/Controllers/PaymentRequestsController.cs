using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TechParvaLEAO.Areas.Expense.Models;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Areas.Organization.Services;
using TechParvaLEAO.Areas.Expense.Services;
using TechParvaLEAO.Controllers;
using TechParvaLEAO.Areas.Expense.Models.ViewModels;
using MediatR;
using CsvHelper;
using System.IO;
using System.Text;
using TechParvaLEAO.Areas.Organization.Models;
using Microsoft.AspNetCore.Http;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Models;
using Microsoft.AspNetCore.Identity; 
using X.PagedList;
using Newtonsoft.Json;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Expense.Controllers.MasterData;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using TechParvaLEAO.Service;
using TechParvaLEAO.Services;
using System.Globalization;
using System.Data.SqlClient;
using DocumentFormat.OpenXml.Bibliography;

namespace TechParvaLEAO.Areas.Expense.Controllers
{
    /*
   * Controller for Payment Requests
   */
    [Area("Expense")]
    public class PaymentRequestsController : BaseViewController
    {
        private readonly IApplicationRepository repository;
        private readonly IEmployeeServices employeeServices;
        private readonly IMediator mediator;
        private readonly PaymentRequestService paymentRequestService;
        private readonly LeaveRequestServices leaveRequestService;
        private readonly ApplicationDbContext context;
        private IHostingEnvironment Environment;
        private readonly IsharepointEnhance sharepointSender;


        public PaymentRequestsController(IApplicationRepository repository,
            IEmployeeServices employeeServices,
            PaymentRequestService paymentRequestService,
            LeaveRequestServices leaveRequestService,
            IMediator mediator,
            ApplicationDbContext context,
            IHostingEnvironment _environment, IsharepointEnhance sharepointsender)
        {
            this.repository = repository;
            this.employeeServices = employeeServices;
            this.mediator = mediator;
            this.paymentRequestService = paymentRequestService;
            this.leaveRequestService = leaveRequestService;
            this.context = context;
            this.Environment = _environment;
            sharepointSender = sharepointsender;
        }

        private IPagedList<PaymentRequest> ToPagedList(IEnumerable<PaymentRequest> list, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return list.ToPagedList(pageNumber, pageSize);

            //return list.ToPagedList();

          //  return (IPagedList<PaymentRequest>)list;

        }
        private IPagedList<PendingDocumentViewModel> ToPagedListPendingDocument(IEnumerable<PendingDocumentViewModel> list, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return list.ToPagedList(pageNumber, pageSize);
        }

        /*
        * Controller for Own Advance List
        */
        // GET: Expense/PaymentRequests/AdvanceRequest
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult AdvanceList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {   
            ViewData["CanApproveReject"] = false;
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["LinkURL"] = "ExpenseList";
            ViewData["PageLinkURL"] = "AdvanceList";
            return View(ToPagedList(paymentRequestService.GetOwnAdvances(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
        * Controller for Advance Pending Approval List
        */
        //[Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        [Authorize]
        public ActionResult AdvancePendingApprovalList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["CanApproveReject"] = true;
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["LinkURL"] = "ExpensePendingApprovalList";
            ViewData["PageLinkURL"] = "AdvancePendingApprovalList";
           //  return View("AdvanceList", ToPagedList(paymentRequestService.GetForMyApprovalAdvances(GetEmployee(), paymentRequestSearchViewModel), id));

            // return View("AdvanceList", paymentRequestService.GetForMyApprovalAdvances(GetEmployee(), paymentRequestSearchViewModel));
            var model = paymentRequestService.GetForMyApprovalAdvances(GetEmployee(), paymentRequestSearchViewModel);

            return View("AdvanceList", model);

        }

        /*
        * Controller for On Behalf Advance List
        */
        [Authorize]
        public ActionResult AdvanceOnBehalfList(string sortOrder, [Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["CanApproveReject"] = false;
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["LinkURL"] = "ExpenseOnBehalfList";
            ViewData["EmployeeId"] = new SelectList(repository.Get<Employee>(e=>e.Deactivated==false), "Id", "Name");
            ViewData["PageLinkURL"] = "AdvanceOnBehalfList";
            return View("AdvanceList", ToPagedList(paymentRequestService.GetOnBehalfAdvances(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
        * Controller for On Behalf Expense List
        */
        [Authorize]
        public ActionResult ExpenseOnBehalfList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["CanApproveReject"] = false;
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["LinkURL"] = "AdvanceOnBehalfList";
            ViewData["EmployeeId"] = new SelectList(repository.Get<Employee>(e => e.Deactivated == false), "Id", "Name");
            ViewData["PageLinkURL"] = "ExpenseOnBehalfList";
            return View("ExpenseList", ToPagedList(paymentRequestService.GetOnBehalfExpenses(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
        * Controller for Expense Pending Approval List
        */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult ExpensePendingApprovalList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["CanApproveReject"] = true;
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["LinkURL"] = "AdvancePendingApprovalList";
            ViewData["PageLinkURL"] = "ExpensePendingApprovalList";
            return View("ExpenseList", ToPagedList(paymentRequestService.GetForMyApprovalExpenses(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
        * Controller for Own Expense List
        */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult ExpenseList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["CanApproveReject"] = false;
            ViewData["LinkURL"] = "AdvanceList";
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["PageLinkURL"] = "ExpenseList";
            return View(ToPagedList(paymentRequestService.GetOwnExpenses(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
        * Show form to create a new Advance Request
        */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> AdvanceOnBehalf(int? id)
        {
            return await Advance(id);
        }

        /*
        * Show form to create a new Advance Request
        */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> Advance(int? id)
        {
            var employee = await employeeServices.GetEmployee(User);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            ViewData["BusinessActivityId"] = new SelectList(repository.Get<BusinessActivity>(e => e.Deactivated == false), "Id", "Name");
            ViewData["CustomerMarketId"] = new SelectList(repository.Get<CustomerMarket>(e => e.Deactivated == false), "Id", "Name");
            ViewData["CurrencyId"] = new SelectList(repository.Get<Currency>(e => e.Deactivated == false && e.IsForex==false), "Id", "Name");
            ViewData["Employee_CanApplyOnBehalf"] = Request.Path.Value.Contains("OnBehalf");
            return View("Advance", new AdvanceViewModel());
        }

        /*
        * Method for Can Create Advance Request
        */
        private IEnumerable<PendingDocumentPaymentRequests> CanCreateAdvanceRequest(int employeeId)
        {
            var employee = repository.GetById<Employee>(employeeId);

            return paymentRequestService.GetDocumentNotSubmittedExpenseReports(employee);
        }

        /*
        * Method for Can Create Advance Request
        */
        private IEnumerable<PendingDocumentPaymentRequests> CanCreateExpenseRequest(
            int employeeId, DateTime startDate, DateTime endDate)
        {
            var employee = repository.GetById<Employee>(employeeId);
            return paymentRequestService.GetDocumentNotSubmittedExpenseReports(employee);
        }


        /*
        * Method for Validate Pending Advances
        */
        private IEnumerable<PaymentRequest> ValidatePendingAdvances(int employeeId)
        {
            var employee = repository.GetById<Employee>(employeeId);
            return paymentRequestService.GetUnsettledPaymentRequests(employee, null);
        }

        /*
        * Method for Validate Employemnt Status
        */
        private bool ValidateEmployemntStatus(int employeeId)
        {
            var employee = repository.GetById<Employee>(employeeId);
            if (employee.LastWorkingDate==null || employee.LastWorkingDate >= DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        /*
       *  Creates a new Advance request from submitted form data
       */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> Advance([Bind("Amount,CurrencyId,Comment,EmployeeId, Type, BusinessActivityId, CustomerMarketId, DraftId")] AdvanceViewModel paymentRequest)
        {
            if (ModelState.IsValid)
            {
                var pendingDocuments = CanCreateAdvanceRequest(Int32.Parse(paymentRequest.EmployeeId));
                var noAdvanceRequest = ValidatePendingAdvances(Int32.Parse(paymentRequest.EmployeeId));
                var employmentStatus = ValidateEmployemntStatus(Int32.Parse(paymentRequest.EmployeeId));
                if (pendingDocuments.Count() >= 3)
                {
                    return View("AdvanceNotAllowed", pendingDocuments);
                }
                if (noAdvanceRequest.Count() >= 3)
                {
                    return View("AdvanceCanNotCreated", noAdvanceRequest);
                }
                if (!employmentStatus)
                {
                    return View("EmployeeResigned", null);
                }

                var createdByEmployee = GetEmployee();
                paymentRequest.CreatedByEmployeeId = createdByEmployee.Id;
                bool result = await mediator.Send(paymentRequest);
                if (result)
                {
                    if (string.Equals(paymentRequest.EmployeeId, GetEmployee().Id.ToString()))
                    {
                        return Redirect("AdvanceList");
                    }
                    else
                    {
                        return Redirect("AdvanceOnBehalfList");
                    }
                }
            }
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name","Designation");
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(GetEmployee().Id.ToString(), paymentRequest.EmployeeId);
            return View(paymentRequest);
        }

        /*
       * Controller for Approved Payment Request
       */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult Approve(int? id, int[] paymentRequestId)
        {
            if (id == null && (paymentRequestId == null || paymentRequestId.Length==0))
            {
                return NotFound();
            }
            PaymentRequestApproveRejectViewModel vm = new PaymentRequestApproveRejectViewModel
            {
                PaymentRequestId = id.HasValue?id.Value:0,
                PaymentRequestsId = paymentRequestId,
                ActionById = GetEmployee().Id,
            };

            int prId = 0;
            if (id.HasValue)
            {
                prId = id.Value;
            }
            else
            {
                prId = paymentRequestId[0];
            }
            var paymentRequest = repository.GetById<PaymentRequest>(prId);
            //var workingDays = leaveRequestService.GetBusinessDays(paymentRequest.Employee,
            //        paymentRequest.PaymentRequestCreatedDate, DateTime.Today, false, false);
            //if (workingDays > 4) //Working days considers day of application as one day, which is not the case
            //{
            //    return View("AutoCancelConfirmation");
            //}
            //else
            //{

                var result = mediator.Send(vm).Result;

                if (!result)
                {
                    return StatusCode(401);
                }
           // }

            
            if (string.Equals(paymentRequest.Type, PaymentRequestType.ADVANCE.ToString())){
                return RedirectToAction("AdvancePendingApprovalList");
            }
            else
            {
                return RedirectToAction("ExpensePendingApprovalList");
            }
        }

        /*
       * Controller for Confirmed Supporting Received
       */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult Confirm(int[] paymentRequestId)
        {
            if (paymentRequestId == null)
            {
                return NotFound();
            }
            foreach (int prId in paymentRequestId)
            {
                SupportingReceivedViewModel vm = new SupportingReceivedViewModel
                {
                    ActionById = GetEmployee().Id,
                    Date = DateTime.Now,
                    PaymentRequestId = prId
                };
                vm.ActionById = GetEmployee().Id;
                var result = mediator.Send(vm).Result;
                if (!result)
                {
                    return StatusCode(401);
                }
            }
            return RedirectToAction("PendingDocumentSearchModelList");
        }

        /*
      * Show form to reject payment request
      */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p=>p.Id==id);
            PaymentRequestApproveRejectViewModel vm = new PaymentRequestApproveRejectViewModel
            {
                PaymentRequestId = id.Value,
                ActionById = GetEmployee().Id
            };
            ViewBag.RejectionReasonId = new SelectList(repository.Get<PaymentRequestRejectionReason>().Where(r=>r.Type==paymentRequest.Type), 
                "Id", "Reason", paymentRequest.RejectionReasonsId);
            return View("Reject", vm);
        }

        /*
      * Payment request rejected
      */
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult Reject([Bind("RejectionReasonId, PaymentRequestId, RejectionReasonOther")] PaymentRequestApproveRejectViewModel vm)
        {
            if (vm.PaymentRequestId == 0)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == vm.PaymentRequestId);
            //var workingDays = leaveRequestService.GetBusinessDays(paymentRequest.Employee,
            //        paymentRequest.PaymentRequestCreatedDate, DateTime.Today, false, false);
            //if (workingDays > 4) //Working days considers day of application as one day, which is not the case
            //{
            //    return View("AutoCancelConfirmation");
            //}
            //else
            //{
                if (ModelState.IsValid && vm.RejectionReasonId != 0)
                {
                    vm.ActionById = GetEmployee().Id;
                    var result = mediator.Send(vm).Result;
                    if (!result)
                    {
                        return StatusCode(401);
                    }
                    if (string.Equals(paymentRequest.Type, PaymentRequestType.ADVANCE.ToString()))
                    {
                        return RedirectToAction("AdvancePendingApprovalList");
                    }
                    else
                    {
                        return RedirectToAction("ExpensePendingApprovalList");
                    }
                }
           // }
            ViewBag.RejectionReasonId = new SelectList(repository.Get<PaymentRequestRejectionReason>().Where(r => r.Type == paymentRequest.Type),
                "Id", "Reason", paymentRequest.RejectionReasonsId);
            return View(vm);
        }

        /*
          * Show form to create a new expense request
          */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> ExpenseOnBehalf(int? id)
        {
            return await Expense(id);
        }

        /*
      * Show form to create a new expense request
      */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> Expense(int? id)
        {
            ExpenseViewModel viewModel = new ExpenseViewModel();
            viewModel.Amount = 1;
            if (id.HasValue)
            {
                var previousExpense = await repository.GetByIdAsync<PaymentRequest>(id);
                foreach (var lineItem in previousExpense.LineItems)
                {
                    viewModel.ExpenseLineItems.Add(new ExpenseLineItemsViewModel {
                        ExpenseHead = lineItem.ExpenseHeadId.ToString(),
                        BusinessActivity = lineItem.BusinessActivityId.ToString(),
                        CustomerMarket = lineItem.CustomerMarketId.ToString(),
                        ExpenseVoucherReferenceNo = lineItem.ExpenseVoucherReferenceNumber,
                        Description = lineItem.VoucherDescription,
                        Amount = lineItem.Amount,
                        Date = lineItem.ExpenseDate
                    });
                }
                viewModel.EmployeeId = previousExpense.EmployeeId.ToString();
                viewModel.CurrencyId = previousExpense.CurrencyId.ToString();
            }
            else
            {
                viewModel.ExpenseLineItems.Add(new ExpenseLineItemsViewModel { Amount = 0.0f });
            };
            var employee = await employeeServices.GetEmployee(User);

            if (Request.Path.Value.Contains("OnBehalf"))
            {
                ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            }
            else
            {
                ViewData["EmployeeId"] = new SelectList(await employeeServices.GetselfEmployee(User), "Id", "DisplayName");
            }

            
            GenerateViewData(viewModel.ExpenseLineItems);
            ViewData["EmployeeLoggedin"] = employee.Id;
            ViewData["CanHoldCreditCard"] = employee.CanHoldCreditCard;
            ViewData["AdvancePaymentRequestId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["CurrencyId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            if(viewModel.EmployeeId == null)
            {
                ViewData["Employee_CanApplyOnBehalf"] = Request.Path.Value.Contains("OnBehalf");
            }
            else
            {
                ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), viewModel.EmployeeId);
            }
            return View("Expense", viewModel);
        }

        /*
         * Controller for expense draft
         */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> ExpenseDraft(int? id)
        {
            ExpenseViewModel viewModel = paymentRequestService.GetExpenseDraft(User, id.Value);
            var employee = await employeeServices.GetEmployee(User);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName", viewModel.EmployeeId);
            GenerateViewData(viewModel.ExpenseLineItems);
            ViewData["EmployeeLoggedin"] = employee.Id;
            ViewData["CanHoldCreditCard"] = employee.CanHoldCreditCard;
            ViewData["AdvancePaymentRequestId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["CurrencyId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), viewModel.EmployeeId);
            return View("Expense", viewModel);
        }

        /*
      * Controller for Advance draft
      */
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> AdvanceDraft(int? id)
        {
            AdvanceViewModel viewModel = paymentRequestService.GetAdvanceDraft(User, id.Value);
            var employee = await employeeServices.GetEmployee(User);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName", viewModel.EmployeeId);
            ViewData["BusinessActivityId"] = new SelectList(repository.Get<BusinessActivity>(e => e.Deactivated == false), "Id", "Name", viewModel.BusinessActivityId);
            ViewData["CustomerMarketId"] = new SelectList(repository.Get<CustomerMarket>(e => e.Deactivated == false), "Id", "Name", viewModel.CustomerMarketId);
            ViewData["CurrencyId"] = new SelectList(repository.Get<Currency>(e => e.Deactivated == false), "Id", "Name", viewModel.CurrencyId);
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), viewModel.EmployeeId);
            return View("Advance", viewModel);
        }

        /*
         * Creates a new Expense Request from submitted form data
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> Expense(List<IFormFile> supportings, [Bind] ExpenseViewModel paymentRequest)
        {
            //  string wwwPath = this.Environment.WebRootPath;
            //string contentPath = this.Environment.ContentRootPath;
           // TempData["ButtonDisabled"] = true;

            string domain = Request.Scheme + "://" + Request.Host;



            // LogWriter.WriteLog(" TestSharepoint()", domain);
            paymentRequest.fake_path = domain;

            //SharePoint_service sps = new SharePoint_service();
            //await sps.Upload_file_sharepoint_Async("name","2022", paymentRequest.fake_path);


            //  var filePath = Path.GetTempFileName();

            paymentRequest.Supportings = supportings;
            var createdByEmployee = GetEmployee();
            TryValidateModel(paymentRequest, nameof(ExpenseViewModel));
            paymentRequest.CreatedByEmployeeId = createdByEmployee.Id;
            var employee = GetEmployee();
            if (ModelState.IsValid && paymentRequest.ExpenseLineItems.Count() > 0)
            {
                bool leaveExists = false;
                foreach(var lineItem in paymentRequest.ExpenseLineItems)
                {
                    if (leaveRequestService.LeaveExists(Int32.Parse(paymentRequest.EmployeeId), lineItem.Date.Value))
                    {
                        ModelState.AddModelError("", "You have approved leave for date " + lineItem.Date.Value.ToString("dd-MM-yyyy"));
                    leaveExists = true;
                    }
                }
                if (!leaveExists)
                {
                    bool result = mediator.Send(paymentRequest).Result;
                    if (result)
                    {
                        if (string.Equals(paymentRequest.EmployeeId, GetEmployee().Id.ToString()))
                        {
                            return RedirectToAction("ExpenseList");
                        }
                        else
                        {
                            return RedirectToAction("ExpenseOnBehalfList");
                        }
                    }
                }
            }
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            GenerateViewData(paymentRequest.ExpenseLineItems);
            ViewData["AdvancePaymentRequestId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["CurrencyId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), paymentRequest.EmployeeId);
            return View("Expense", paymentRequest);
        }

        /*
         * Creates Line Item
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> AddLineItem([Bind] ExpenseViewModel paymentRequest)
        {
            paymentRequest.ExpenseLineItems.Add(new ExpenseLineItemsViewModel
            {
                Amount = 0.0f,
            });
            var employee = await employeeServices.GetEmployee(User);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            if (paymentRequest.EmployeeId != null)
            {
                var paymentEmployee = repository.GetById<Employee>(Int32.Parse(paymentRequest.EmployeeId));
                ViewData["AdvancePaymentRequestId"] = new SelectList(paymentRequestService.GetOwnUnsettledAdvances(paymentEmployee), "Id", "RequestNumber");
            }
            ViewData["CurrencyId"] = new SelectList(context.Currencies, "Id", "Name");
            ViewData["EmployeeLoggedin"] = employee.Id;
            ViewData["CanHoldCreditCard"] = employee.CanHoldCreditCard;
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), paymentRequest.EmployeeId);
            GenerateViewData(paymentRequest.ExpenseLineItems);
      

            return View("Expense", paymentRequest);

          
        }

        //[Authorize]
        ////  public async Task<IActionResult> ApproveConfirmedAll(string s_id)
        //public async Task<IActionResult> ApproveConfirmedAll(ExpenseViewModel paymentRequest)
        //{
        //    //  string[] ids = JsonConvert.DeserializeObject<string[]>(paymentRequest);
        //    string empid = JsonConvert.DeserializeObject<string>(paymentRequest.EmployeeId);
        //    paymentRequest.EmployeeId = empid;

        //    paymentRequest.ExpenseLineItems.Add(new ExpenseLineItemsViewModel
        //    {
        //        Amount = 0.0f,
        //    });
        //    var employee = await employeeServices.GetEmployee(User);
        //    ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
        //    if (paymentRequest.EmployeeId != null)
        //    {
        //        var paymentEmployee = repository.GetById<Employee>(Int32.Parse(paymentRequest.EmployeeId));
        //        ViewData["AdvancePaymentRequestId"] = new SelectList(paymentRequestService.GetOwnUnsettledAdvances(paymentEmployee), "Id", "RequestNumber");
        //    }
        //    ViewData["CurrencyId"] = new SelectList(context.Currencies, "Id", "Name");
        //    ViewData["EmployeeLoggedin"] = employee.Id;
        //    ViewData["CanHoldCreditCard"] = employee.CanHoldCreditCard;
        //    ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), paymentRequest.EmployeeId);
        //    GenerateViewData(paymentRequest.ExpenseLineItems);
        //    ExpenseViewModel viewModel = new ExpenseViewModel();
        //    viewModel.ExpenseLineItems.Add(new ExpenseLineItemsViewModel { Amount = 0.0f });


        //    return View("Expense", paymentRequest);
   
        //}



            /*
             * Delete Line Items
             */
            [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> RemoveLineItem([Bind] ExpenseViewModel paymentRequest, int id)
        {
            ModelState.Clear();
            var employee = await employeeServices.GetEmployee(User);
            ViewBag.EmployeeId = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            if (paymentRequest.EmployeeId != null)
            {
                var paymentEmployee = repository.GetById<Employee>(Int32.Parse(paymentRequest.EmployeeId));
                ViewBag.AdvancePaymentRequestId = new SelectList(paymentRequestService.GetOwnUnsettledAdvances(paymentEmployee), "Id", "RequestNumber");
            }
            ViewBag.EmployeeLoggedin = employee.Id;
            ViewData["CanHoldCreditCard"] = employee.CanHoldCreditCard;
            paymentRequest.ExpenseLineItems.RemoveAt(id);
            ViewData["CurrencyId"] = new SelectList(context.Currencies, "Id", "Name");
            GenerateViewData(paymentRequest.ExpenseLineItems);
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), paymentRequest.EmployeeId);
            return View("Expense", paymentRequest);
        }

        private void GenerateViewData(IList<ExpenseLineItemsViewModel> items)
        {
            int i = 0;
            foreach (var item in items)
            {
                ViewData["ExpenseHead"]= new SelectList(repository.Get<ExpenseHead>(e => e.Deactivated == false), "Id", "ExpenseHeadDesc");
                ViewData["BusinessActivity"]= new SelectList(repository.Get<BusinessActivity>(e => e.Deactivated == false), "Id", "DisplayBusinessName");
                ViewData["CustomerMarket"] = new SelectList(repository.Get<CustomerMarket>(e => e.Deactivated == false), "Id", "Name");
                i++;
            }
        }

        /*
         * Show form to edit Expense Request
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public async Task<IActionResult> EditExpense(int? id)
        {
            EditExpenseViewModel viewModel = new EditExpenseViewModel();
            if (id.HasValue)
            {
                if (id == null)
                {
                    return NotFound();
                }
                var previousExpense = await repository.GetByIdAsync<PaymentRequest>(id);
                foreach (var lineItem in previousExpense.LineItems)
                {
                    viewModel.ExpenseLineItems.Add(new ExpenseLineItemsViewModel
                    {
                        Id = lineItem.Id,
                        ExpenseHead = lineItem.ExpenseHeadId.ToString(),
                        BusinessActivity = lineItem.BusinessActivityId.ToString(),
                        CustomerMarket = lineItem.CustomerMarketId.ToString(),
                        ExpenseVoucherReferenceNo = lineItem.ExpenseVoucherReferenceNumber,
                        Description = lineItem.VoucherDescription,
                        Amount = lineItem.Amount,
                        Date = lineItem.ExpenseDate
                    });
                }
                viewModel.EmployeeId = previousExpense.EmployeeId.ToString();
                viewModel.CurrencyId = previousExpense.CurrencyId.ToString();
                viewModel.CurrencyName = previousExpense.Currency.Name.ToString();
                viewModel.EmployeeName = previousExpense.Employee.Name.ToString();
                viewModel.Comment = previousExpense.Comment.ToString();
                viewModel.AdvancePaymentRequest = previousExpense.RequestNumber;
                viewModel.FromDate = previousExpense.FromDate.Value;
                viewModel.ToDate = previousExpense.ToDate.Value;
            }
            else
            {
                viewModel.ExpenseLineItems.Add(new ExpenseLineItemsViewModel { Amount = 0.0f });
            };
            var employee = await employeeServices.GetEmployee(User);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            GenerateViewData(viewModel.ExpenseLineItems);
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(employee.Id.ToString(), viewModel.EmployeeId);
            return View(viewModel);

        }

        /*
         * Updates Expense Request from submitted form data
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> EditExpense(List<IFormFile> supportings, [Bind] EditExpenseViewModel paymentRequest)
        {
            paymentRequest.Supportings = supportings;

            var createdByEmployee = GetEmployee();
            paymentRequest.CreatedByEmployeeId = createdByEmployee.Id;
            if (ModelState.IsValid && paymentRequest.ExpenseLineItems.Count() > 0)
            {
                foreach(var lineItemViewModel in paymentRequest.ExpenseLineItems)
                {
                    var lineItem = repository.GetById<PaymentRequestLineItems>(lineItemViewModel.Id);
                    lineItem.ExpenseHeadId = Int32.Parse(lineItemViewModel.ExpenseHead);
                    lineItem.CustomerMarketId = Int32.Parse(lineItemViewModel.CustomerMarket);
                    lineItem.BusinessActivityId = Int32.Parse(lineItemViewModel.BusinessActivity);
                    repository.Save();
                }
                return RedirectToAction("FinancePendingExpenseList");
            }
            
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            GenerateViewData(paymentRequest.ExpenseLineItems);
            ViewData["Employee_CanApplyOnBehalf"] = !string.Equals(createdByEmployee.Id.ToString(), paymentRequest.EmployeeId);
            return View(paymentRequest);
        }

        /*
         * Show Finance Pending Advance List
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult FinancePendingAdvanceList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            ViewData["Currency"] = new SelectList(repository.Get<Currency>(e => e.Deactivated == false), "Name", "Name");
            return View("FinancePendingAdvanceList", ToPagedList(paymentRequestService.GetFinanceApprovedAdvanceList(GetEmployee(), paymentRequestSearchViewModel), id));
        }

        /*
         * Show Finance Pending Expense List
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult FinancePendingExpenseList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {

            ViewData["Currency"] = new SelectList(repository.Get<Currency>(e => e.Deactivated == false), "Name", "Name");
            var loggedInEmployeeId = GetEmployee().Id;
            //return View("FinancePendingExpenseList", ToPagedList(paymentRequestService.GetFinanceApprovedExpenseList(GetEmployee(), paymentRequestSearchViewModel), id));
            //added by Priya
            SqlCommand cmd = new SqlCommand();
            SqlParameter Action = new SqlParameter("@Action", "Search");
            // cmd.Parameters.AddWithValue("@Action", "Search"); SqlParameter Action = new SqlParameter("@Action", "Search");
            SqlParameter Status = new SqlParameter("@Status", paymentRequestSearchViewModel.Status ?? (object)DBNull.Value);
            SqlParameter logInEmployeeId = new SqlParameter("@loggedInEmployeeId", loggedInEmployeeId);
            SqlParameter RNUmber = new SqlParameter("@RequestNumber", paymentRequestSearchViewModel.RequestNumber ?? (object)DBNull.Value);
            SqlParameter EName = new SqlParameter("@EName", paymentRequestSearchViewModel.EmployeeName ?? (object)DBNull.Value);
            SqlParameter EmployeeCode = new SqlParameter("@EmployeeCode", paymentRequestSearchViewModel.EmployeeCode ?? (object)DBNull.Value);
            SqlParameter FromAmount = new SqlParameter("@FromAmount", paymentRequestSearchViewModel.FromAmount ?? (object)DBNull.Value);
            SqlParameter ToAmount = new SqlParameter("@ToAmount", paymentRequestSearchViewModel.ToAmount ?? (object)DBNull.Value);
            SqlParameter FromDate = new SqlParameter("@FromDate", paymentRequestSearchViewModel.FromDate ?? (object)DBNull.Value);
            SqlParameter ToDate = new SqlParameter("@ToDate", paymentRequestSearchViewModel.ToDate ?? (object)DBNull.Value);
            SqlParameter Currency = new SqlParameter("@Currency", paymentRequestSearchViewModel.Currency ?? (object)DBNull.Value);
            //IEnumerable<PaymentRequest> result = context.PaymentRequests.FromSql("Execute GetFinanceApprovedExpenseList @Action,@Status,@RequestNumber",Action,Status,RNUmber,EName,EmployeeCode,FromAmount,ToAmount,FromDate,ToDate,Currency);
            var result = context.PaymentRequests.FromSql("Execute GetFinanceApprovedExpenseList @Action,@Status,@loggedInEmployeeId,@RequestNumber,@EName,@EmployeeCode,@FromAmount,@ToAmount,@FromDate,@ToDate,@Currency", Action, Status, logInEmployeeId, RNUmber, EName, EmployeeCode, FromAmount, ToAmount, FromDate, ToDate, Currency);
            return View("FinancePendingExpenseList", ToPagedList(result, id));


        }

        /*
         * Show Finance Settlement List
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult FinanceSettlementList([Bind] PaymentRequestSearchViewModel paymentRequestSearchViewModel, int? id)
        {
            return View("FinanceSettlementList", paymentRequestService.GetForSettlementEmployees(GetEmployee(), paymentRequestSearchViewModel));
        }

        /*
         * Show Supporting Received confirmation form
         */
        [Authorize]
        public ActionResult SupportingReceived(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == id);
            SupportingReceivedViewModel vm = new SupportingReceivedViewModel
            {
                PaymentRequestId = id.Value,
                ActionById = GetEmployee().Id,
                Date = DateTime.Now
            };
            return View("SupportingReceived", vm);
        }


        [Authorize]
        [RequestSizeLimit(40000000)]
        public ActionResult SupportingConfirmedAll(string s_id)
        {
            string[] ids = JsonConvert.DeserializeObject<string[]>(s_id);

            for (int i = 0; i < ids.Length; i++)
            {
                try
                {

               
                int id = Convert.ToInt32(ids[i]);

                PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == id);
                SupportingReceivedViewModel vm = new SupportingReceivedViewModel
                {
                    PaymentRequestId = id,
                    ActionById = GetEmployee().Id,
                    Date = DateTime.Now
                };


                if (vm.PaymentRequestId == 0)
                {
                    return NotFound();
                }
                else
                {
                    vm.PaymentRequestId = id;
                    vm.ActionById = GetEmployee().Id;
                    vm.Date = DateTime.Now;
                }
                PaymentRequest paymentRequest1 = repository.GetFirst<PaymentRequest>(p => p.Id == vm.PaymentRequestId);
                if (ModelState.IsValid && vm.Date != null)
                {
                    vm.ActionById = GetEmployee().Id;
                    var result = mediator.Send(vm).Result;
                    if (!result)
                    {
                        return StatusCode(401);
                    }
                    //if (string.Equals(paymentRequest1.Type, PaymentRequestType.ADVANCE.ToString()))
                    //{
                    //    return RedirectToAction("PendingDocumentSearchModelList");
                    //}
                    //else
                    //{
                    //    return RedirectToAction("PendingDocumentSearchModelList");
                    //}
                }
                    //  return View(vm);

                }
                catch (Exception ex)
                {

                }

            }

            

            return RedirectToAction("PendingDocumentSearchModelList");

            // return View("SupportingReceived");

        }

        /*
         * Confirmed Supporting Received date
         */
        [HttpPost, ActionName("SupportingReceived")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult SupportingReceived(int? id, [Bind("Date, PaymentRequestId")] SupportingReceivedViewModel vm)
        {
            if (vm.PaymentRequestId == 0)
            {
                return NotFound();
            }
            else {
                vm.PaymentRequestId = id.Value;
                vm.ActionById = GetEmployee().Id;
                vm.Date = DateTime.Now;
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == vm.PaymentRequestId);
            if (ModelState.IsValid && vm.Date != null)
            {
                vm.ActionById = GetEmployee().Id;
                var result = mediator.Send(vm).Result;
                if (!result)
                {
                    return StatusCode(401);
                }
                if (string.Equals(paymentRequest.Type, PaymentRequestType.ADVANCE.ToString())){
                    return RedirectToAction("PendingDocumentSearchModelList");
                }
                else
                {
                    return RedirectToAction("PendingDocumentSearchModelList");                    
                }
            }
            return View(vm);
        }

        /*
         * Show Update Forex Rate form
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult UpdateForexRate(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == id);
            ForexSettlementViewModel vm = new ForexSettlementViewModel
            {
                PaymentRequestId = id.Value,
                ActionById = GetEmployee().Id,
                ForexRate = paymentRequest.ExchangeRate
            };
            return View("UpdateForexRate", vm);
        }

        /*
         * Updates Forex Rate
         */
        [HttpPost, ActionName("UpdateForexRate")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        [Authorize]
        public ActionResult UpdateForexRate([Bind("ForexRate, PaymentRequestId")] ForexSettlementViewModel vm)
        {
            if (vm.PaymentRequestId == 0)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == vm.PaymentRequestId);
            if (ModelState.IsValid && vm.ForexRate != 0)
            {
                vm.ActionById = GetEmployee().Id;
                var result = mediator.Send(vm).Result;
                if (!result)
                {
                    return StatusCode(401);
                }
                return RedirectToAction("FinancePendingExpenseList");
            }
            return View(vm);
        }

        /*
         * Show form to update payment mode
         */
        [Authorize]
        public ActionResult UpdatePaymentMode(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == id);
            PaymentModeViewModel vm = new PaymentModeViewModel
            {
                PaymentRequestId = id.Value,
                ActionById = GetEmployee().Id,
            };
            return View("UpdatePaymentMode", vm);
        }

        /*
         * Updates Payment Mode
         */
        [HttpPost, ActionName("UpdatePaymentMode")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult UpdatePaymentMode([Bind("PaymentMode, PaymentRequestId")] PaymentModeViewModel vm)
        {
            if (vm.PaymentRequestId == 0)
            {
                return NotFound();
            }
            PaymentRequest paymentRequest = repository.GetFirst<PaymentRequest>(p => p.Id == vm.PaymentRequestId);
            if (ModelState.IsValid && vm.PaymentMode != null)
            {
                vm.ActionById = GetEmployee().Id;
                var result = mediator.Send(vm).Result;
                if (!result)
                {
                    return StatusCode(401);
                }
                return RedirectToAction("FinancePendingExpenseList");
            }
            return View(vm);
        }

        /*
        * Show form to update Employee Settlement
        */
        [Authorize]
        public ActionResult UpdateEmployeeSettlement(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Employee employee = repository.GetFirst<Employee>(e => e.Id == id);
            EmployeeSettlementViewModel vm = new EmployeeSettlementViewModel
            {
                Employee = employee,
                ActionById = GetEmployee().Id,
                EmployeeId = id.Value,
                SettlementDate = DateTime.Now.Date
            };
            return View("UpdateEmployeeSettlement", vm);
        }

        /*
         * Updates Employee Settlement
         */
        [HttpPost, ActionName("UpdateEmployeeSettlement")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult UpdateEmployeeSettlement(
            [Bind("EmployeeId, SettlementDate, SettlementAmount")]  EmployeeSettlementViewModel vm)
        {
            if (vm.EmployeeId == 0)
            {
                return NotFound();
            }
            Employee employee = repository.GetFirst<Employee>(p => p.Id == vm.EmployeeId);
            if (ModelState.IsValid )
            {
                //employee.SettlementById = GetEmployee().Id;
                employee.SettlementAmount = vm.SettlementAmount;
                employee.SettlementDate = vm.SettlementDate;
                context.Entry<Employee>(employee).State = EntityState.Modified;
                context.SaveChanges();
                return RedirectToAction("FinanceSettlementList");
            }
            return View(vm);
        }



        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public FileContentResult FinanceAdvanceExportPaymentsCSV (int[] id, [Bind]PaymentRequestSearchViewModel paymentRequestSearchViewModel)
        {
            var result = paymentRequestService.GetFinanceApprovedAdvanceList(GetEmployee(), paymentRequestSearchViewModel);
            var loggedInEmployeeId = GetEmployee().Id;
            var data = new List<AdvanceDataExportViewModel>();
            int recordNumber = 1;

            foreach (var item in result)
            {
                if (item.PostedById == null)
                {
                    item.PostedById = loggedInEmployeeId;
                    item.PostedOn = DateTime.Now;
                    item.Status = PaymentRequestStatus.POSTED.ToString();
                    PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
                    {
                        ActionById = loggedInEmployeeId,
                        PaymentRequest = item,
                        Timestamp = DateTime.Now,
                        Action = PaymentRequestActions.POSTED.ToString(),
                        Type = PaymentRequestType.ADVANCE.ToString()
                    };
                    context.Entry<PaymentRequest>(item).State = EntityState.Modified;
                    context.Entry<PaymentRequestApprovalAction>(approvalActions).State = EntityState.Added;
                }
                data.Add(new AdvanceDataExportViewModel
                {
                    SerialNumber = recordNumber,
                    AccountNumber = item.Employee.AccountNumber,
                    AdvanceRequestNumber = item.RequestNumber,
                    CreateDate = item.PaymentRequestCreatedDate,
                    EmployeeCode = item.Employee.EmployeeCode,
                    EmployeeName = item.Employee.Name,
                    LocationCode = item.Employee.Location.Code,
                    Amount = item.Amount,
                    Currency = item.Currency.Name,
                    AmountInINR = item.Currency.Id == 1 ? item.Amount : 0.00,
                    Detail = item.Comment,
                    Status = string.Equals(item.Status, PaymentRequestStatus.POSTED.ToString()) ? "Approved,pending for Finance action" : item.Status
                });
            }
            context.SaveChanges();
            var writer = new StringWriter();
            var csv = new CsvWriter(writer);
            csv.WriteRecords(data);
            return File(Encoding.ASCII.GetBytes(writer.ToString()), "text/csv", "FinanceAdvanceToNavigen.csv");
        }

        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public FileContentResult FinanceExpenseExportPaymentsCSV(int[] id, [Bind]PaymentRequestSearchViewModel paymentRequestSearchViewModel)
        {
            //commneted below line by Priya
            //   var result = paymentRequestService.GetFinanceApprovedExpenseList(GetEmployee(), paymentRequestSearchViewModel);
            //  result = paymentRequestService.ApplySearch(result, paymentRequestSearchViewModel);
            try
            {
                var loggedInEmployeeId = GetEmployee().Id;
                SqlCommand cmd = new SqlCommand();
                SqlParameter Action = new SqlParameter("@Action", "Download");
                // cmd.Parameters.AddWithValue("@Action", "Search"); SqlParameter Action = new SqlParameter("@Action", "Search");
                SqlParameter Status = new SqlParameter("@Status", paymentRequestSearchViewModel.Status ?? (object)DBNull.Value);
                SqlParameter logInEmployeeId = new SqlParameter("@loggedInEmployeeId", loggedInEmployeeId);
                SqlParameter RNUmber = new SqlParameter("@RequestNumber", paymentRequestSearchViewModel.RequestNumber ?? (object)DBNull.Value);
                SqlParameter EName = new SqlParameter("@EName", paymentRequestSearchViewModel.EmployeeName ?? (object)DBNull.Value);
                SqlParameter EmployeeCode = new SqlParameter("@EmployeeCode", paymentRequestSearchViewModel.EmployeeCode ?? (object)DBNull.Value);
                SqlParameter FromAmount = new SqlParameter("@FromAmount", paymentRequestSearchViewModel.FromAmount ?? (object)DBNull.Value);
                SqlParameter ToAmount = new SqlParameter("@ToAmount", paymentRequestSearchViewModel.ToAmount ?? (object)DBNull.Value);
                SqlParameter FromDate = new SqlParameter("@FromDate", paymentRequestSearchViewModel.FromDate ?? (object)DBNull.Value);
                SqlParameter ToDate = new SqlParameter("@ToDate", paymentRequestSearchViewModel.ToDate ?? (object)DBNull.Value);
                SqlParameter Currency = new SqlParameter("@Currency", paymentRequestSearchViewModel.Currency ?? (object)DBNull.Value);
                var result = context.PaymentRequests.FromSql("Execute GetFinanceApprovedExpenseList @Action,@Status,@loggedInEmployeeId,@RequestNumber,@EName,@EmployeeCode,@FromAmount,@ToAmount,@FromDate,@ToDate,@Currency", Action, Status, logInEmployeeId, RNUmber, EName, EmployeeCode, FromAmount, ToAmount, FromDate, ToDate, Currency);

                var data = new List<ExpenseDataExportViewModel>();
                int recordNumber = 1;
                foreach (var item in result)
                {
                    if (item.LineItems.Count() > 0)
                    {
                        //if (item.PostedById == null && paymentRequestSearchViewModel.Status== "APPROVED")
                        //{
                        //    item.PostedById = loggedInEmployeeId;
                        //    item.PostedOn = DateTime.Now;
                        //    item.Status = PaymentRequestStatus.POSTED.ToString();
                        //    item.DownloadedDate = DateTime.Now;//.ToString("dd-MM-yyyy");
                        //    PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
                        //    {
                        //        ActionById = loggedInEmployeeId,
                        //        PaymentRequest = item,
                        //        Timestamp = DateTime.Now,
                        //        Action = PaymentRequestActions.POSTED.ToString(),
                        //        Type = PaymentRequestType.REIMBURSEMENT.ToString()
                        //    };
                        //    context.Entry<PaymentRequest>(item).State = EntityState.Modified;
                        //    context.Entry<PaymentRequestApprovalAction>(approvalActions).State = EntityState.Added;
                        //}
                        foreach (var l in item.LineItems)
                        {

                            data.Add(new ExpenseDataExportViewModel
                            {
                                EmployeeCode = item.Employee.EmployeeCode,
                                PostingDate = (item.ActionDate.HasValue) ? item.ActionDate.Value.ToString("dd-MM-yyyy") : "",
                                DocumentDate = item.PaymentRequestCreatedDate.ToString("dd-MM-yyyy"),
                                //   PostingDate = DateTime.ParseExact(item.ActionDate.Value.ToString("dd-MMM-yyyy"), "dd-MMM-yyyy", CultureInfo.InvariantCulture),
                                DocumentNumber = recordNumber,
                                ForeignAmount = item.CurrencyId == 1 ? l.Amount : l.Amount,
                                Currency = item.Currency.Name,
                                FxRate = item.ExchangeRate,
                                Amount = item.CurrencyId == 1 ? l.Amount : l.Amount * item.ExchangeRate,
                                AccountNumber = item.Employee.AccountNumber,
                                ExpenseHead = l.ExpenseHead.AccountNumber,
                                BillNumber = item.RequestNumber,
                                LocationCode = item.Employee.Location.Code,
                                BusinessMarket = l.BusinessActivity.Code,
                                CustomerMarket = l.CustomerMarket.Code,
                                PostingDescription = item.Comment,
                                LineDescription = l.VoucherDescription,
                                Dimension3 = item.CreditCard ? "CREDITCARD" : "",
                                Dimension4 = PaymentRequestStatus.PAID.ToString().Equals(item.Status) ? "PAID" : "",
                                Dimension7 = (item.DownloadedDate.HasValue) ? item.DownloadedDate.ToString() : "",
                                Dimension8 = item.CurrencyId != 1 ? (item.AdvancePaymentRequest !=null)?item.AdvancePaymentRequest.RequestNumber:"" : ""
                            });
                        }
                    }
                    recordNumber++;
                }

                var writer = new StringWriter();
                var csv = new CsvWriter(writer);
                csv.WriteRecords(data);
                //  context.SaveChanges();
                return File(Encoding.ASCII.GetBytes(writer.ToString()), "text/csv", "FinanceExpenseToNavigen.csv");
            }
            catch (Exception ex )
            {
                return File(Encoding.ASCII.GetBytes(ex.ToString()), "text/csv", "FinanceExpenseToNavigen.csv");

                // throw ex;
            }

        }

        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public FileContentResult PendingDocumentExportCSV(int[] id, [Bind]PaymentRequestSearchViewModel paymentRequestSearchViewModel)
        {
            var result = paymentRequestService.GetFinancePendingDocumentsList(GetEmployee(), paymentRequestSearchViewModel);
            var loggedInEmployeeId = GetEmployee().Id;
            foreach (var item in result)
            {
                if (item.PostedById == null)
                {
                    item.PostedById = loggedInEmployeeId;
                    item.PostedOn = DateTime.Now;
                    item.Status = PaymentRequestStatus.POSTED.ToString();
                    PaymentRequestApprovalAction approvalActions = new PaymentRequestApprovalAction
                    {
                        ActionById = loggedInEmployeeId,
                        PaymentRequest = item,
                        Timestamp = DateTime.Now,
                        Action = PaymentRequestActions.POSTED.ToString(),
                        Type = PaymentRequestType.REIMBURSEMENT.ToString()
                    };
                    context.Entry<PaymentRequest>(item).State = EntityState.Modified;
                    context.Entry<PaymentRequestApprovalAction>(approvalActions).State = EntityState.Added;
                }
            }
            context.SaveChanges();
            var data = new List<PendingDocumentExportViewModel>();

            int recordNumber = 1;
            foreach (var p in result)
            {
                if (string.Equals(PaymentRequestType.REIMBURSEMENT.ToString(), p.Type))
                {
                    data.Add(new PendingDocumentExportViewModel
                    {
                        EmployeeCode = p.Employee.EmployeeCode,
                        ApprovedDate = p.ActionDate,
                        PostingDate = p.ActionDate.Value,
                        DocumentDate = p.PaymentRequestCreatedDate,
                        DocumentNumber = recordNumber,
                        ForeignAmount = p.Currency.Id!=1 ? p.Amount:0,
                        Currency = p.Currency.Name,
                        FxRate = p.ExchangeRate,
                        Amount = p.Currency.Id != 1 ? p.Amount * p.ExchangeRate: p.Amount,
                        AccountNumber = p.Employee.AccountNumber,
                        BillNumber = p.RequestNumber,
                        LocationCode = p.Employee.Location.Code,
                        PostingDescription = p.Comment,
                        ApprovedBy = p.PaymentRequestActionedBy.Name,
                        PostedBy = p.PostedBy.Name,
                    });               
                    recordNumber++;
                }
            }
            var writer = new StringWriter();
            var csv = new CsvWriter(writer);
            csv.WriteRecords(data);
            return File(Encoding.ASCII.GetBytes(writer.ToString()), "text/csv", "PendingDocumentListData.csv");
        }


        public byte[] WriteCsvToMemory(IEnumerable<PaymentRequest> records)
        {
            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        [HttpGet]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public IActionResult UploadPaymentFile()
        {
            return View("UploadPaymentFile");
        }

        [HttpPost]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public async Task<IActionResult> UploadPaymentFileAsync(IFormFile file, string operation)
        {
            if (file == null || file.Length == 0)
                return Content("file not selected");

            CsvReader csvReader = new CsvReader(new StreamReader(file.OpenReadStream()));
            csvReader.Configuration.HasHeaderRecord = true;
            csvReader.Configuration.MissingFieldFound = null;
            csvReader.Configuration.BadDataFound = null;

            var records = csvReader.GetRecords<FinancePaymentDataImportViewModel>().ToList<FinancePaymentDataImportViewModel>();

            double upload_record_amount = 0.00;

            foreach (var record in records)
            {
                foreach (PropertyInfo property in record.GetType().GetProperties())
                {

                    if (property.Name.Contains("PaymentDetail"))
                    {
                        var payment_detail_data = property.GetValue(record, null);

                        if (payment_detail_data != null && payment_detail_data != "")
                        {
                            var paymentRequest = context.PaymentRequests.Where(p => p.RequestNumber == payment_detail_data.ToString()  && (p.Status == PaymentRequestStatus.PENDING.ToString() || p.Status == PaymentRequestStatus.POSTED.ToString()) ).FirstOrDefault();

                            if (paymentRequest != null)
                            {
                                double res = Double.Parse(upload_record_amount.ToString());
                                double payment_amount = Double.Parse(paymentRequest.Amount.ToString());

                                upload_record_amount = res + payment_amount;
                            }
                        }
                    }
                }

                if(Double.Parse(record.TransactionAmount) > Double.Parse(upload_record_amount.ToString()))
                {
                    return Content("Please upload file with correct transaction amount.");
                }
            }

            var loggedInEmployeeId = GetEmployee().Id;
            var lastPaymentType = PaymentRequestType.ADVANCE.ToString();
            var result = new List<FinancePaymentDataImportResult>();

            foreach (var record in records)
            {              
                foreach (PropertyInfo property in record.GetType().GetProperties())
                {
                    
                    if (property.Name.Contains("PaymentDetail"))
                    {
                        var payment_detail_data = property.GetValue(record, null);

                        if (payment_detail_data != null && payment_detail_data != "")
                        {
                var procResult = new FinancePaymentDataImportResult(record, payment_detail_data.ToString());
                var paymentRequest = context.PaymentRequests.Where(p => p.RequestNumber == payment_detail_data.ToString()).FirstOrDefault();
                if (paymentRequest == null)
                {
                    procResult.IsRecordUpdatedSuccessfully = "NO_RECORD_FOUND";
                    procResult.TransactionAmount = 0.ToString();
                   result.Add(procResult);
                    continue;
                }
                          
                procResult.IsPaymentRequestFound = true;

                procResult.TransactionAmount = paymentRequest.Amount.ToString();

                if (string.Equals(paymentRequest.Status, PaymentRequestStatus.POSTED.ToString())||
                    (string.Equals(paymentRequest.Status, PaymentRequestStatus.PAID.ToString()) 
                        && Double.Parse(record.TransactionAmount) < 0))
                {
                    var paymentRecord = new PaymentRequestPaymentRecord {
                        PaymentRequestId = paymentRequest.Id,
                        TransactionType = record.TransactionType,
                        EmployeeCode = record.EmployeeCode,
                        BenificiaryCode = record.BenificiaryCode,
                      //  TransactionAmount = Double.Parse(record.TransactionAmount),
                        TransactionAmount = Double.Parse(paymentRequest.Amount.ToString()),
                        BenificiaryName = record.BenificiaryName,
                        PaymentDetail1 = payment_detail_data.ToString(),
                        ChqTime = record.ChqTime,
                        TimeStamp = DateTime.Now
                    };
                    var approvalActions = new PaymentRequestApprovalAction
                    {
                        ActionById = loggedInEmployeeId,
                        PaymentRequest = paymentRequest,
                        Timestamp = DateTime.Now,
                        Action = PaymentRequestActions.PAID.ToString(),
                        Type = paymentRequest.Type
                    };
                    paymentRequest.Status = PaymentRequestStatus.PAID.ToString();
                    paymentRequest.PaidAmount = paymentRequest.PaidAmount + Double.Parse(paymentRequest.Amount.ToString());
                    paymentRequest.PaidAmount = 0;

                     paymentRequest.PaidDate = DateTime.Now;
                    if(paymentRequest.CurrencyId != 1 && paymentRequest.AdvancePaymentRequestId !=null && 
                        string.Equals(paymentRequest.Type, PaymentRequestTypes.REIMBURSEMENT.ToString()))
                    {
                        paymentRequest.AdvancePaymentRequest.Settled = true;
                        context.Entry<PaymentRequest>(paymentRequest.AdvancePaymentRequest).State = EntityState.Modified;
                    }
                    context.Entry<PaymentRequestPaymentRecord>(paymentRecord).State = EntityState.Added;
                    context.Entry<PaymentRequestApprovalAction>(approvalActions).State = EntityState.Added;
                    context.Entry<PaymentRequest>(paymentRequest).State = EntityState.Modified;
                    
                    procResult.IsRecordUpdatedSuccessfully = "PROCESSED";

                    var notification = new NotificationEventModel
                    {
                        Type = string.Equals(paymentRequest.Type, PaymentRequestType.ADVANCE.ToString())
                                    ?EmailNotification.TYPE_ADVANCE: EmailNotification.TYPE_EXPENSE,
                        Event = string.Equals(paymentRequest.Type, PaymentRequestType.ADVANCE.ToString())
                                    ? EmailNotification.STATUS_ADVANCE_PAID : EmailNotification.STATUS_EXPENSE_PAID,
                        ModelType = typeof(PaymentRequest),
                        ObjectId = paymentRequest.Id
                    };
                    await mediator.Publish<NotificationEventModel>(notification);
                    lastPaymentType = paymentRequest.Type;
                    procResult.IsRecordUpdatedSuccessfully = "PAID";
                }
                else if (string.Equals(paymentRequest.Status, PaymentRequestStatus.PAID.ToString()))
                {
                    procResult.IsRecordUpdatedSuccessfully = "ALREADY PAID";
                }
                else
                {
                    procResult.IsRecordUpdatedSuccessfully = "NOT PROCESSED";
                }
                context.SaveChanges();
                result.Add(procResult);
            }
                    }
                }
            }


            // Advance entry

            foreach (var record in records)
            {
                //if (Convert.ToDouble(record.TransactionAmount) > 0)
                //{

                    var employee = await repository.GetOneAsync<Employee>(e => e.EmployeeCode == record.EmployeeCode);


                    // var paymentRequest = mapper.Map<AdvanceViewModel, PaymentRequest>(paymentRequestVm);

                    PaymentRequest paymentRequest = new PaymentRequest();

                    paymentRequest.PaymentRequestCreatedById = employee.Id;
                  //  paymentRequest.Type = PaymentRequestType.REIMBURSEMENT.ToString();
                    paymentRequest.Status = PaymentRequestStatus.PAID.ToString();
                    paymentRequest.PaymentRequestActionedById = employee.ReportingToId;
                    paymentRequest.LocationId = employee.LocationId.Value;
                    //  paymentRequest.BalanceAmount =  Convert.ToInt32(record.TransactionAmount);

                    //paymentRequest.Amount = Convert.ToInt32(record.TransactionAmount);
                    paymentRequest.Amount = 0;

                    paymentRequest.PaidAmount = Convert.ToDouble(record.TransactionAmount);
                    paymentRequest.EmployeeId = employee.Id;
                    paymentRequest.Comment = "Advance entry from upload file to settle expenses";
                    paymentRequest.CurrencyId = 1;
                    paymentRequest.PaidDate = DateTime.Now;
                    paymentRequest.SupportingDocumentsReceivedDate = DateTime.Now;  

                    await paymentRequestService.MapReimursementEntry(paymentRequest, employee);

                    await paymentRequestService.MapAdvanceExpenses();

              //  }

            }



            return View("UploadPaymentResult", result);
            /*
            if (string.Equals(lastPaymentType, PaymentRequestType.ADVANCE.ToString()))
            {
                return RedirectToAction("FinancePendingAdvanceList");
            }
            else
            {
                return RedirectToAction("FinancePendingExpenseList");
            }
            */
        }


        public async Task<IActionResult> ExpenseDetailsPreview(int? id, ExpenseViewModel paymentRequestVm)
        {
            if (id == null)
            {
                return View();
              //  return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }

            return View(paymentRequest);
        }


        /*
         * Expense Request details
         */
        // GET: Expense/PaymentRequests/ExpenseDetails/5
        public async Task<IActionResult> ExpenseDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }

            return View(paymentRequest);
        }


        public async Task<IActionResult> DownloadSupporting(int? id)
        {
           



            if (id == null )
            {
                return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
            .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }

            var SPUD = new SharePointUploadData
            {

                filename = paymentRequest.RequestNumber,
                foldername = paymentRequest.RequestNumber.Split("/")[4].Split("-")[0],
                filePath = "",
            };

            //await sharepointSender.Upload_file_sharepoint_Async(SPUD);

            try
            {
                await sharepointSender.DownloadFile(SPUD);
            }
            catch (Exception ex)
            {

            }

      



            //var paymentRequest = await context.PaymentRequests
            //    .FirstOrDefaultAsync(m => m.Id == id);
            //if (paymentRequest == null)
            //{
            //    return NotFound();
            //}

            return View();
        }




        /*
         * Advanc Request details
         */
        // GET: Expense/PaymentRequests/AdvanceDetails/5
        public async Task<IActionResult> AdvanceDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }
            return View(paymentRequest);
        }

        /*
         * Finance Pending Advance In Detail
         */
        // GET: Expense/PaymentRequests/FinancePendingAdvanceInDetail/5
        public async Task<IActionResult> FinancePendingAdvanceInDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }
            return View(paymentRequest);
        }

        /*
         * Finance Pending Expense In Detail
         */
        // GET: Expense/PaymentRequests/FinancePendingAdvanceInDetail/5
        public async Task<IActionResult> FinancePendingExpenseInDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paymentRequest = await context.PaymentRequests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (paymentRequest == null)
            {
                return NotFound();
            }
            return View(paymentRequest);
        }

        /*
         * Show list of Pending Document Search Model List
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult PendingDocumentSearchModelList(int? id, PaymentRequestSearchViewModel paymentRequestSearchViewModel)
        {
            var viewModel = PendingDocumentSearchModel(id, paymentRequestSearchViewModel);
            return View("PendingDocumentSearchModelList", viewModel);
        }

        /*
         * Controller for Pending Document Search Model
         */
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        private DashboardViewModel PendingDocumentSearchModel(int? id, [Bind]PaymentRequestSearchViewModel paymentRequestSearchViewModel)
        {
            DashboardViewModel dashboardViewModel = new DashboardViewModel();
            var financePendingDocuments = paymentRequestService.GetFinancePendingDocumentsList(GetEmployee(), paymentRequestSearchViewModel)
               .Select(p => new PendingDocumentViewModel
               {
                   Id = p.Id,
                   Amount = p.Amount,
                   EmployeeName = p.Employee.Name,
                   RequestNumber = p.RequestNumber,
                   ClaimDate = p.PaymentRequestCreatedDate,
                   Status = p.Status,
                   Role = p.Employee.Designation.Name,
                   HardCopyReceived = p.SupportingDocumentsReceivedDate.HasValue,
                   ReceivedDate = p.SupportingDocumentsReceivedDate,
                   CurrencyName = p.Currency.Name,
               }).ToList();
            dashboardViewModel.PendingDocuments = financePendingDocuments;
            return dashboardViewModel;
        }

        /*
     * Controller for Expense List Draft
     */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult ExpenseListDraft()
        {
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["CanApproveReject"] = false;
            var drafts = paymentRequestService.GetDraftExpenseList(User.Identity.Name);
            var draftsList = new List<ExpenseDraftListRow>();
            foreach (var draft in drafts)
            {
                var item = JsonConvert.DeserializeObject<ExpenseViewModel>(draft.FormData);
                var vm = new ExpenseDraftListRow();
                vm.Draft = draft;
                vm.ViewModel = item;
                draftsList.Add(vm);
            }
            return View(draftsList);
        }

        /*
    * Controller for Advance List Draft
    */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult AdvanceListDraft()
        {
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["CanApproveReject"] = false;
            var drafts = paymentRequestService.GetDraftAdvanceList(User.Identity.Name);
            var draftsList = new List<AdvanceDraftListRow>();
            foreach (var draft in drafts)
            {
                var item = JsonConvert.DeserializeObject<AdvanceViewModel>(draft.FormData);
                var vm = new AdvanceDraftListRow();
                vm.Draft = draft;
                vm.ViewModel = item;
                draftsList.Add(vm);
            }
            return View(draftsList);
        }

        /*
    * Delete Advance Draft
    */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult DeleteAdvanceDraft(int id)
        {
            paymentRequestService.DeleteDraft(User.Identity.Name, id);
            return RedirectToAction("AdvanceListDraft");
        }

        /*
    * Delete Expense Draft
    */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult DeleteExpenseDraft(int id)
        {
            paymentRequestService.DeleteDraft(User.Identity.Name, id);
            return RedirectToAction("ExpenseListDraft");
        }
    }
}
