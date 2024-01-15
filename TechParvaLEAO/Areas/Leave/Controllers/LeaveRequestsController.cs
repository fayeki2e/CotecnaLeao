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
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Leave.Models.ViewModels;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Controllers;
using MediatR;
using System;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using X.PagedList;
using System.Linq;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Services;
using TechParvaLEAO.Models;
 

namespace TechParvaLEAO.Areas.Leave.Controllers
{
    /*
     * Controller for Leave Request
     */
    [Area("Leave")]
    public class LeaveRequestsController : BaseViewController
    {
        private readonly IApplicationRepository repository;
        private readonly IEmployeeServices employeeServices;
        private readonly IMediator mediator;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices;
        private readonly LocationWorkdaysService locationWorkdaysService;
        private readonly TimeSheetServices timesheetServices;
        private readonly IMapper mapper;
        private readonly ApplicationDbContext _context;

        private readonly IEmailSenderEnhance emailSender;



        public LeaveRequestsController(IApplicationRepository repository,
            IMapper mapper,
            IMediator mediator,
            IEmployeeServices employeeServices,
            LeaveRequestServices leaveRequestServices,
            LocationWorkdaysService locationWorkdaysService,
            LeaveCreditAndUtilizationServices leaveCreditAndUtilizationServices,
            TimeSheetServices timesheetServices,
              IEmailSenderEnhance emailSender
        )
        {
            this.repository = repository;
            this.mediator = mediator;
            this.mapper = mapper;
            this.employeeServices = employeeServices;
            this.leaveRequestServices = leaveRequestServices;
            this.locationWorkdaysService = locationWorkdaysService;
            this.leaveCreditAndUtilizationServices = leaveCreditAndUtilizationServices;
            this.timesheetServices = timesheetServices;
            this.emailSender = emailSender;
        }

        private IPagedList<LeaveRequest> ToPagedList(IEnumerable<LeaveRequest> list, int? page)
        {
            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return list.ToPagedList(pageNumber, pageSize);
        }

        /*
         * Show List of own Leaves
         */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult LeaveListOwn([Bind] LeaveRequestSearchViewModel leaveRequestSearchViewModel, int? id)
        {
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["CanApproveReject"] = false;


          //  return View(ToPagedList(leaveRequestServices.GetOwnLeaves(GetEmployee(), leaveRequestSearchViewModel),id));

            return View(leaveRequestServices.GetOwnLeaves(GetEmployee(), leaveRequestSearchViewModel));

        }

        /*
         * Show List of Leave Draft
         */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult LeaveListDraft(int id)
        {
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["CanApproveReject"] = false;
            var drafts = leaveRequestServices.GetDraftLeaves(User.Identity.Name);
            var leaveDrafts = new List<LeaveDraftListRow>();
            foreach (var draft in drafts)
            {
                var item = JsonConvert.DeserializeObject<LeaveDraftViewModel>(draft.FormData);
                var vm = new LeaveDraftListRow();
                vm.LeaveDraft = draft;
                vm.ViewModel = item;
                leaveDrafts.Add(vm);
            }
            return View(leaveDrafts);
        }

        /*
        * Delete a Leave Draft
        */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult DeleteLeaveDraft(int id)
        {
            leaveRequestServices.DeleteDraft(User.Identity.Name, id);
            return RedirectToAction("LeaveListDraft");
        }

        /*
        * Show List of Reporting To Leaves
        */
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult LeaveListReportingTo(int reportingTo, [Bind] LeaveRequestSearchViewModel leaveRequestSearchViewModel)
        {
            ViewData["LoggedInEmployeeId"] = GetEmployee().Id;
            ViewData["CanApproveReject"] = false;
            var employee = employeeServices.GetEmployee(reportingTo);
            return View(leaveRequestServices.GetOwnLeaves(GetEmployee(), leaveRequestSearchViewModel));
        }

        /*
        * Show List of Pending Approval Leaves
        */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult LeavesPendingApprovalList([Bind] LeaveRequestSearchViewModel leaveRequestSearchViewModel)
        {
            ViewData["CanApproveReject"] = true;
            return View("Index", leaveRequestServices.GetForMyApprovalLeaves(GetEmployee(), leaveRequestSearchViewModel));
        }

        /*
        * Show List of Leaves
        */
        public ActionResult Index([Bind] LeaveRequestSearchViewModel leaveRequestSearchViewModel)
        {
            ViewData["CanApproveReject"] = true;
            return View(leaveRequestServices.GetOnBehalfLeaves(GetEmployee(), leaveRequestSearchViewModel));
        }

        /*
        * Show List of On behalf Leaves
        */
        public ActionResult OnBehalfLeaves([Bind] LeaveRequestSearchViewModel leaveRequestSearchViewModel)
        {
            ViewData["CanApproveReject"] = false;
            return View("Index", leaveRequestServices.GetOnBehalfLeaves(GetEmployee(), leaveRequestSearchViewModel));
        }

        private async Task<NewLeaveViewModel> GetLeaveViewModel(int? id, bool init=true)
        {
            var viewModel =null as NewLeaveViewModel;
            if (id.HasValue)
            {
                viewModel = leaveRequestServices.GetLeaveDraft(User, id.Value);
            }
            else
            {
                viewModel = new NewLeaveViewModel();
            }

            var employee = await employeeServices.GetEmployee(User);
            var accountingPeriod = leaveRequestServices.GetAccountingPeriod(DateTime.Today);
            if (accountingPeriod != null)
            {
                var holidays = locationWorkdaysService.GetHolidays(employee.LocationId.Value,
                    accountingPeriod.StartDate, accountingPeriod.EndDate);
                foreach (var holiday in holidays)
                {
                    viewModel.Holidays.Add(new Models.ViewModels.HolidayVm
                    {
                        Date = holiday.HolidayDate,
                        Description = holiday.Reason,
                        Type = ""
                    });
                }
            }

            if (init)
            {
                viewModel.EmployeeId = employee.Id;
                viewModel.EmployeeName = employee.Name;
                viewModel.EmployeeCode = employee.EmployeeCode;
                viewModel.EmployeeLocation = employee.Location?.Name;
                viewModel.EmployeeDesignation = employee.Designation?.Name;
            }
            if(employee.Name != null)
            {
                viewModel.EmployeeName = employee.Name;
                viewModel.EmployeeCode = employee.EmployeeCode;
            }

            viewModel.LeaveApprover = employee.ReportingTo?.Name;
            viewModel.AnnualLeaveEligibility = leaveRequestServices.GetAnnaulLeaveEligibility(GetEmployee());
            viewModel.LeavesCarryForward = leaveRequestServices.GetAnnaulLeavesCarryForward(GetEmployee());


            //var prev_year = Convert.ToDateTime("12/31/"+ (DateTime.Today.Year - 1));
            //var prev_year = Convert.ToDateTime((DateTime.Today.Year - 1) + "/12/31");
            //var prev_utilize = leaveRequestServices.GetAnnaulLeaveUtilized(GetEmployee(), prev_year);

            //if(viewModel.LeavesCarryForward >0)
            //{
            //    viewModel.LeavesCarryForward = viewModel.LeavesCarryForward - (viewModel.AnnualLeaveEligibility-prev_utilize);
            //}
            viewModel.LeavesCarryForward = leaveRequestServices.GetAnnaulLeavesCarryForward(GetEmployee());

            viewModel.AnnualLeaves = viewModel.AnnualLeaveEligibility + viewModel.LeavesCarryForward;
            viewModel.LeavesUtilized = leaveRequestServices.GetAnnaulLeaveUtilized(GetEmployee());
            viewModel.LeavesPendingApproval = leaveRequestServices.GetAnnaulLeavePending(GetEmployee());
            viewModel.LeavesWithoutPay = leaveRequestServices.GetLWPDays(GetEmployee());
            viewModel.LeaveBalance = viewModel.AnnualLeaves - viewModel.LeavesUtilized;
            ViewBag.LeavesCarryForward = 0;



            return viewModel;
        }

        [Authorize]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> NewRequestLeave()
        {
            try
            {
                var viewModel = await GetLeaveViewModel(null);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog(" NewRequestLeave()", ex.Message.ToString());
                return View(ex);
            }
        }

        [Authorize]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> RequestLeaveFormOnBehalf(int? id)
        {
            return await RequestLeaveForm(id);
        }

        /*
         * Show form to creates a new Leave Request
         */
        [Authorize]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<ActionResult> RequestLeaveForm(int? id)
        {
            var viewModel = await GetLeaveViewModel(id, false);
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "DisplayName");
            if (viewModel.EmployeeId != 0)
            {
                ViewData["LeaveTypeId"] = new SelectList(await leaveRequestServices.GetLeaveTypesForEmployee(viewModel.EmployeeId), "Id", "Name");
            }
            else
            {
                ViewData["LeaveTypeId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            if (viewModel.LeaveTypeId.HasValue)
            {
                ViewData["LeaveCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveCategories(viewModel.LeaveTypeId.Value), "Id", "Text");
            }
            else
            {
                ViewData["LeaveCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            if(viewModel.LeaveCategoryId.HasValue && viewModel.LeaveTypeId.HasValue)
            {
                ViewData["LeaveSubCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveSubCategories(
                    viewModel.LeaveCategoryId.Value, viewModel.LeaveTypeId.Value), "Id", "Text");
            }
            else
            {
                ViewData["LeaveSubCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            return View("RequestLeaveForm", viewModel);
        }

        /*
         * Creates a new Leave request from submitted form data
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<IActionResult> RequestLeaveFormOnBehalf(List<IFormFile> documents, [Bind] NewLeaveViewModel leaveRequestVm)
        {
            return await RequestLeaveForm(documents, leaveRequestVm);
        }

        /*
         * Creates a new Leave request from submitted form data
         */
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        [Authorize(Roles = AuthorizationRoles.EMPLOYEE)]
        public async Task<IActionResult> RequestLeaveForm(List<IFormFile> documents, [Bind] NewLeaveViewModel leaveRequestVm)
        {
            if (!ModelState.IsValid)
            {
                ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name");
                if (leaveRequestVm.EmployeeId != 0)
                {
                    ViewData["LeaveTypeId"] = new SelectList(await leaveRequestServices.GetLeaveTypesForEmployee(leaveRequestVm.EmployeeId), "Id", "Name");
                }
                else
                {
                    ViewData["LeaveTypeId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
                if (leaveRequestVm.LeaveTypeId.HasValue)
                {
                    ViewData["LeaveCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveCategories(leaveRequestVm.LeaveTypeId.Value), "Id", "Text");
                }
                else
                {
                    ViewData["LeaveCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                if (leaveRequestVm.LeaveCategoryId.HasValue && leaveRequestVm.LeaveTypeId.HasValue)
                {
                    ViewData["LeaveSubCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveSubCategories(
                        leaveRequestVm.LeaveCategoryId.Value, leaveRequestVm.LeaveTypeId.Value), "Id", "Text");
                }
                else
                {
                    ViewData["LeaveSubCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }

                return View("RequestLeaveForm", leaveRequestVm);
            }

            var employee = await employeeServices.GetEmployee(leaveRequestVm.EmployeeId);
            if ((employee.Status==EmployeeStatus.RESIGNED|| employee.Status == EmployeeStatus.SERVICETERMINATED)
                && leaveRequestVm.EndDate > employee.LastWorkingDate){ 
                return View("EmployeeResigned", null);
            }

            leaveRequestVm.Documents = documents;
            var createdByEmployee = GetEmployee();
            leaveRequestVm.CreatedByEmployeeId = createdByEmployee.Id;
            var result = mediator.Send(leaveRequestVm).Result;
            if (result.Success)
            {
                var employeeLeaves = leaveRequestServices.Check_LeavesOfTeam(User, "",  Convert.ToDateTime(leaveRequestVm.StartDate),Convert.ToDateTime( leaveRequestVm.EndDate))
            .Select(p => new LeaveRequestModal
            {
                Id = p.Id,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status,
                Employee = p.Employee,    
            }).ToList();

                if (employeeLeaves.Count > 1)
                {
                   
                    var notification = new NotificationEventModel
                    {

                        Type = "Leave",
                        Event = "LeaveNotification_ToReportingManager"
                    };
                    var configuration = leaveRequestServices.get_NoticationConfiguration(notification).ToList();

                    var objEmp = new List<Employee>();
                    var emp = new Employee();
                    emp.Name = employee.ReportingTo.Name.ToString();                  
                    objEmp.Add(emp);

                    var email = new NotificationEmailViewModel
                    {
                        ViewName = configuration[0].TemplatePathHtml,
                        To = employee.Email,
                        Subject = configuration[0].SubjectLine,
                        PaymentRequestData = null,
                        LeaveRequestData = null,
                        TimesheetData = null,
                        PaymentRequestDataList = null,
                        LeaveRequestDataList = null,
                        TimesheetDataList = null,
                        obj_leaverequest = null,
                        obj_LeaveRequestModal = employeeLeaves,
                        EmployeeDataList = objEmp,
                        AcceptButtonUrl = "",
                        RejectButtonUrl = "",
                        DetailButtonUrl = "",
                        FromDate = DateTime.Now,
                        ToDate = DateTime.Now,
                        Receiver = null
                    };
                    try
                    {
                        await emailSender.SendEmailAsync(email);
                    }
                    catch (Exception ex)
                    {

                    }
              


                }



                if (string.Equals(leaveRequestVm.EmployeeId.ToString(), GetEmployee().Id.ToString()))
                {
                    return RedirectToAction("LeaveListOwn");
                }
                else
                {
                    return RedirectToAction("OnBehalfLeaves");
                }
            }
            else
            {
                foreach (var msg in result.ErrorMessage)
                {
                    ModelState.AddModelError(string.Empty, msg);
                }
            }
            ViewData["EmployeeId"] = new SelectList(await employeeServices.GetAvailableEmployeesAsync(User), "Id", "Name");
            if (leaveRequestVm.EmployeeId != 0)
            {
                ViewData["LeaveTypeId"] = new SelectList(await leaveRequestServices.GetLeaveTypesForEmployee(leaveRequestVm.EmployeeId), "Id", "Name");
            }
            else
            {
                ViewData["LeaveTypeId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
            if (leaveRequestVm.LeaveTypeId.HasValue)
            {
                ViewData["LeaveCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveCategories(leaveRequestVm.LeaveTypeId.Value), "Id", "Text");
            }
            else
            {
                ViewData["LeaveCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            if (leaveRequestVm.LeaveCategoryId.HasValue && leaveRequestVm.LeaveTypeId.HasValue)
            {
                ViewData["LeaveSubCategoryId"] = new SelectList(await leaveRequestServices.GetLeaveSubCategories(
                    leaveRequestVm.LeaveCategoryId.Value, leaveRequestVm.LeaveTypeId.Value), "Id", "Text");
            }
            else
            {
                ViewData["LeaveSubCategoryId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }

            return View("RequestLeaveForm", leaveRequestVm);
        }

        /*
         * Leave request approve confirmation
         */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public ActionResult Approve(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            return View();
        }

        /*
         * Approved a Leave Request
         */
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public async Task<IActionResult> ApproveConfirmed(int id)
        {
            LeaveRequestApproveRejectViewModel vm = new LeaveRequestApproveRejectViewModel
            {
                LeaveRequestId = id,
                ActionById = GetEmployee().Id
            };


            var leave = leaveRequestServices.Get_Leave(id);
            var workingDays = leaveRequestServices.GetBusinessDays(leave.FirstOrDefault().Employee,
                    leave.FirstOrDefault().LeaveRequestCreatedDate, DateTime.Today, false, false);
            if (workingDays > 4) //Working days considers day of application as one day, which is not the case
            {
                return View("AutoCancelConfirmation");
            }
            else
            {
                var result = await mediator.Send(vm);
                if (!result)
                {
                    return StatusCode(401);
                }
            }
            


          //  var leave = leaveRequestServices.Get_Leave(id);


            if(leave.ToList()[0].LeaveCategoryId !=1)
            {
                return RedirectToAction("LeavesPendingApprovalList");
            }

            var team_members = leaveRequestServices.Get_TeamMember_list(id).ToList();
            var obj_leave_record = leaveRequestServices.get_LeavesrecordOfEmployee(id).ToList();
            var notification = new NotificationEventModel
            {
             
                Type = "Leave",
                Event = "TeamNotification" 
            };

            var configuration = leaveRequestServices.get_NoticationConfiguration(notification).ToList();



            var teamlist = obj_leave_record[0].teamlist;
            var teams = "";

            if (string.IsNullOrEmpty(teamlist))
            {
                ViewData["teams"] = null;
            }
            else
            {
                ViewData["teams"] = employeeServices.GetTeam(teamlist).ToList();
            }

            if (teamlist != null)
            {
                for (int i = 0; i < ViewBag.teams.Count; i++)
                {

                    if (i == ViewBag.teams.Count - 1)
                    {
                        teams = teams + ViewBag.teams[i].TeamName;
                    }
                    else
                    {
                        teams = teams + ViewBag.teams[i].TeamName + ",";
                    }
                }
            }

            





            foreach (Employee employee in team_members)
            {
                var objEmp = new List<Employee>();
                var emp = new Employee();
                emp.Name = employee.Name;
                objEmp.Add(emp);

                var email = new NotificationEmailViewModel
                {
                    ViewName = configuration[0].TemplatePathHtml,
                    To = employee.Email,
                    Subject = configuration[0].SubjectLine,
                    PaymentRequestData = null,
                    LeaveRequestData = null,
                    TimesheetData = null,
                    PaymentRequestDataList = null,
                    LeaveRequestDataList = null,
                    TimesheetDataList = null,
                    obj_leaverequest = obj_leave_record,
                    EmployeeDataList = objEmp,
                    AcceptButtonUrl = "",
                    RejectButtonUrl = "",
                    DetailButtonUrl = teams,
                    FromDate = DateTime.Now,
                    ToDate = DateTime.Now,
                    Receiver = null
                };
                try
                {
                    await emailSender.SendEmailAsync(email);
                }
                catch (Exception ex)
                {

                }
                       
            }

            return RedirectToAction("LeavesPendingApprovalList");

        }

       
        [Authorize]
        public async Task<IActionResult> ApproveConfirmedAll(string s_id)
        {
            string[] ids = JsonConvert.DeserializeObject<string[]>(s_id);

            for(int i = 0; i < ids.Length; i++)
            {
                int id = Convert.ToInt32(ids[i]);
                LeaveRequestApproveRejectViewModel vm = new LeaveRequestApproveRejectViewModel
                {
                    LeaveRequestId = Convert.ToInt32(id),
                    ActionById = GetEmployee().Id
                };

                var result = await mediator.Send(vm);
                if (!result)
                {
                    return StatusCode(401);
                }
            }

        
               return RedirectToAction("LeavesPendingApprovalList");
           
        }
        /*
         * Method for can cancel leave request
         */
        private bool CanCancelLeaveRequest(LeaveRequest leaveRequest)
        {
            var timesheets = timesheetServices.GetTimeSheetsForEmployee(leaveRequest.Employee, 
                leaveRequest.StartDate, leaveRequest.EndDate);
            if (timesheets.Count() > 0)
            {
                return false;
            }

            return true;
        }

        /*
         * Confirmation for cancel leave request
         */
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public async Task<ActionResult> Cancel(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }
            var leaveRequest = await repository.GetFirstAsync<LeaveRequest>(p => p.Id == id);
            if (!CanCancelLeaveRequest(leaveRequest))
            {
                return View("CannotCancel");
            }
            var vm = new LeaveRequestCancelViewModel
            {
                LeaveRequestId = id.Value,
                ActionById = GetEmployee().Id,

            };    
            return View("Cancel", vm);
            
        }

        /*
         * Cancelled a leave request
         */
        [HttpPost, ActionName("Cancel")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public async Task<IActionResult> Cancel([Bind("LeaveRequestId, CancellationReason")] LeaveRequestCancelViewModel vm)
        {
            
                if (vm.LeaveRequestId == 0)
                {
                    return NotFound();
                }
                LeaveRequest leaveRequest = repository.GetFirst<LeaveRequest>(p => p.Id == vm.LeaveRequestId );
                leaveRequest.CancellationReason = vm.CancellationReason;
                vm.ActionById = GetEmployee().Id;
                var result = await mediator.Send(vm);
                if (!result)
                {
                    return StatusCode(401);
                }
                return RedirectToAction("LeavesPendingApprovalList");
            
        }

        /*
         * Confirmation for Documents Received date
         */
        [Authorize]
        public ActionResult DocumentsReceived(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            LeaveRequest leaveRequest = repository.GetFirst<LeaveRequest>(p => p.Id == id);
            DocumentReceivedViewModel vm = new DocumentReceivedViewModel
            {
                LeaveRequestId = id.Value,
                ActionById = GetEmployee().Id,
                Date = DateTime.Now
            };
            return View("DocumentReceived", vm);
        }

        /*
        * controller for Documents Received
        */
        [HttpPost, ActionName("DocumentReceived")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = AuthorizationRoles.FINANCE)]
        public ActionResult DocumentsReceived(int? id, [Bind("Date, LeaveRequestId")] DocumentReceivedViewModel vm)
        {
            if (vm.LeaveRequestId == 0)
            {
                return NotFound();
            }
            else
            {
                vm.LeaveRequestId = id.Value;
                vm.ActionById = GetEmployee().Id;
                vm.Date = DateTime.Now;
            }
            LeaveRequest leaveRequest = repository.GetFirst<LeaveRequest>(p => p.Id == vm.LeaveRequestId);
            if (ModelState.IsValid && vm.Date != null)
            {
                vm.ActionById = GetEmployee().Id;
                var result = mediator.Send(vm).Result;
                if (!result)
                {
                    return StatusCode(401);
                }
                
                return View("DocumentReceived", vm);
                
                
            }
            return View(vm);
        }

        /*
        * Details of Leave Request
        */
        // GET: Leave/LeaveRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await repository.GetByIdAsync<LeaveRequest>(id.Value);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        /*
        * Show form of leave request reject
        */
        //GET: Leave/LeaveRequests/Reject/5
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public async Task<ActionResult> Reject(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var leaveRequest = await repository.GetFirstAsync<LeaveRequest>(p => p.Id == id);
            var vm = new LeaveRequestApproveRejectViewModel
            {
                LeaveRequestId = id.Value,
                ActionById = GetEmployee().Id,
                
            };
            ViewData["RejectionReasonId"] = new SelectList(repository.Get<LeaveRejectionReason>(r=>r.Deactivated==false),
                "Id", "Text");
            return View("Reject", vm);
        }

        /*
        * Leave Request rejected
        */
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = AuthorizationRoles.MANAGER)]
        [Authorize]
        public async Task<ActionResult> Reject([Bind("RejectionReasonId, LeaveRequestId, RejectionReasonOther")] LeaveRequestApproveRejectViewModel vm)
        {
            if (vm.LeaveRequestId == 0)
            {
                return NotFound();
            }
            LeaveRequest leaveRequest = repository.GetFirst<LeaveRequest>(p => p.Id == vm.LeaveRequestId);
            if (ModelState.IsValid && vm.RejectionReasonId != 0)
            {
                var workingDays = leaveRequestServices.GetBusinessDays(leaveRequest.Employee,
                    leaveRequest.LeaveRequestCreatedDate, DateTime.Today, false, false);
                if (workingDays > 4) //Working days considers day of application as one day, which is not the case
                {
                    return View("AutoCancelConfirmation");
                }
                else
                {

                    vm.ActionById = GetEmployee().Id;
                    var result = await mediator.Send(vm);
                    if (!result)
                    {
                        return StatusCode(401);
                    }
                    return RedirectToAction("LeavesPendingApprovalList");
                }
            }
            ViewData["RejectionReasonId"] = new SelectList(repository.GetAll<LeaveRejectionReason>(),
                "Id", "Text");
            return View(vm);
        }

        [HttpPost]
        public ActionResult GetLeavesData()
        {
            LeaveRequestVm dashboardViewModel = new LeaveRequestVm();
            var employeeLeaves = leaveRequestServices.LeavesOfTeamMember(User,"")
               .Select(p => new LeaveRequestModal
               {
                   Id = p.Id ,
                   StartDate = p.StartDate,
                   EndDate=p.EndDate,
                   Status = p.Status,
                   //Amount = p.Amount,
                   Employee =p.Employee,
                   //EmployeeName = p.Employee.Name,
                   //RequestNumber = p.RequestNumber,
                   //ClaimDate = p.PaymentRequestCreatedDate,
                   //Status = p.Status,
                   //Role = p.Employee.Designation.Name,
                   //HardCopyReceived = p.SupportingDocumentsReceivedDate.HasValue,
                   //ReceivedDate = p.SupportingDocumentsReceivedDate,
                   //CurrencyName = p.Currency.Name,
               }).ToList();

          

            return Json(employeeLeaves);
        }

        [HttpPost]
        public ActionResult Check_Team_Leaves(DateTime fromdate, DateTime todate)
        {
            LeaveRequestVm dashboardViewModel = new LeaveRequestVm();
            var employeeLeaves = leaveRequestServices.Check_LeavesOfTeam(User, "", fromdate, todate)
               .Select(p => new LeaveRequestModal
               {
                   Id = p.Id,
                   StartDate = p.StartDate,
                   EndDate = p.EndDate,
                   Status = p.Status,
                   //Amount = p.Amount,
                   Employee = p.Employee,
                   //EmployeeName = p.Employee.Name,
                   //RequestNumber = p.RequestNumber,
                   //ClaimDate = p.PaymentRequestCreatedDate,
                   //Status = p.Status,
                   //Role = p.Employee.Designation.Name,
                   //HardCopyReceived = p.SupportingDocumentsReceivedDate.HasValue,
                   //ReceivedDate = p.SupportingDocumentsReceivedDate,
                   //CurrencyName = p.Currency.Name,
               }).ToList();
            return Json(employeeLeaves);
        }


        [HttpPost]
        public ActionResult GetLeaves_employees(string sdate)
        {
            LeaveRequestVm dashboardViewModel = new LeaveRequestVm();
            var employeeLeaves = leaveRequestServices.LeavesEmployee(User, sdate)
               .Select(p => new Employee
               {
                   Id = p.Id,
                   Name = p.Name,
                   EmployeeCode = p.EmployeeCode,   

                   //StartDate = p.StartDate,
                   //EndDate = p.EndDate,
                   //Status = p.Status,
                   //Amount = p.Amount,
                   //EmployeeName = p.Employee.Name,
                   //RequestNumber = p.RequestNumber,
                   //ClaimDate = p.PaymentRequestCreatedDate,
                   //Status = p.Status,
                   //Role = p.Employee.Designation.Name,
                   //HardCopyReceived = p.SupportingDocumentsReceivedDate.HasValue,
                   //ReceivedDate = p.SupportingDocumentsReceivedDate,
                   //CurrencyName = p.Currency.Name,
               }).ToList();



            return Json(employeeLeaves);
        }


    }
}
