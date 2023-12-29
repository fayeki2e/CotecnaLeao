using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using TechParvaLEAO.Areas.Leave.Services;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Data;
using TechParvaLEAO.Models;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Service;
using TechParvaLEAO.Services;

namespace TechParvaLEAO.Areas.Identity.Pages.Account.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly LeaveRequestServices leaveRequestServices;
        private readonly IEmailSenderEnhance emailSender;
        private readonly ApplicationDbContext _dbContext;

        private readonly IAccountServices accountServices;
        private readonly IsharepointEnhance sharepointSender;

        IConfiguration _configuration;
        public IActionResult Index()
        {
            return View();
        }

        public AccountController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, 
            ILogger<LoginModel> logger, LeaveRequestServices leaveRequestServices, 
            IEmailSenderEnhance emailSender,IAccountServices accountServices,
            IConfiguration configuration, IsharepointEnhance sharepointsender,
              ApplicationDbContext dbContext)
        {
            _signInManager = signInManager;
            _logger = logger;
            _userManager = userManager;
            this.leaveRequestServices = leaveRequestServices;
            this.emailSender = emailSender;
            this.accountServices = accountServices;
            _configuration = configuration;
            sharepointSender = sharepointsender;
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult>  TestSharepoint()
        {

            string file_to_sharepoint = "";
            file_to_sharepoint = "C:/toupload/a4.pdf";

            var SPUD = new SharePointUploadData
            {

                filename = "EXP_MH_C2855_27_2022-23a4.pdf",
                foldername = DateTime.Now.Year.ToString(),
                filePath= file_to_sharepoint
            };

            //await sharepointSender.Upload_file_sharepoint_Async(SPUD);
     
            await sharepointSender.DownloadFile(SPUD);

            // LogWriter.WriteLog(" TestSharepoint()","test");
            return View("Index");
        }




        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string emailid = "";
                emailid = model.Email.ToString();

                //var user = await _userManager.(userName);

                var user =   _userManager.Users.Where(a => a.Email == emailid).ToList();
                //var user = await _userManager.FindByEmailAsync(emailid);

                //var user = _dbContext.Employees.Where(a => a.Email == emailid).ToList()[0];

                //var employee = await accountServices.GetEmployee(user.EmployeeCode);

                if(user.Count > 0)
                {
                    var employee = await accountServices.GetEmployee(user[0].UserName);

                    var objEmp = new List<Employee>();
                    var emp = new Employee();
                    emp.Name = employee.Name;
                    objEmp.Add(emp);
                    if (user != null)
                    {
                        // Generate the reset password token
                        //  var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        string token = HttpUtility.UrlEncode(await _userManager.GeneratePasswordResetTokenAsync(user[0]));
                        var url = "";

                        url = Request.Scheme + "://" + Request.Host.Host;
                        if (Request.Host.Port.ToString() != "")
                        {
                            url = url + ":" + Request.Host.Port;
                        }


                        var passwordResetLink = url + $"/Identity/Account/ResetPassword?email={user[0].Email}&code={token}";
                        var notification = new NotificationEventModel
                        {

                            Type = "Employee",
                            Event = "ForgotPassword"
                        };

                        var configuration = leaveRequestServices.get_NoticationConfiguration(notification).ToList();
                        var email = new NotificationEmailViewModel
                        {
                            ViewName = configuration[0].TemplatePathHtml,
                            To = model.Email,
                            Subject = configuration[0].SubjectLine,
                            PaymentRequestData = null,
                            LeaveRequestData = null,
                            TimesheetData = null,
                            PaymentRequestDataList = null,
                            LeaveRequestDataList = null,
                            TimesheetDataList = null,
                            obj_leaverequest = null,
                            EmployeeDataList = objEmp,
                            AcceptButtonUrl = passwordResetLink,
                            RejectButtonUrl = "",
                            DetailButtonUrl = "",
                            FromDate = DateTime.Now,
                            ToDate = DateTime.Now,
                            Receiver = null
                        };
                        await emailSender.SendEmailAsync(email);

                        return View("ForgotPasswordConfirmation");
                    }
                    return View("ForgotPasswordConfirmation");
                }
                else
                {
                    return View("ForgotPasswordConfirmation");
                }
              
            }

            return View(model);
        }

        [HttpPost, ActionName("validateempdetails")]
        public string validateempdetails(string employeecode,DateTime dateofbirth)
        {
            var employee =   accountServices.validateempdetails(employeecode, dateofbirth);


            if (employee == null)
            {
                return "false";
            }
            else
            {
                return "true";
            }
            
        }


    }
}
