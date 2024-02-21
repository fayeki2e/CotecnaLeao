using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Authorization;
using TechParvaLEAO.Data;
using Microsoft.AspNetCore.Authorization;
using TechParvaLEAO.Areas.Organization.Services;
using System.Threading.Tasks;
using TechParvaLEAO.Areas.Attendance;
using TechParvaLEAO.Areas.Attendance.Services;
using TechParvaLEAO.Controllers;
using MediatR;
using CalendarUtilities;
using System.Globalization;
using TechParvaLEAO.Areas.Organization.Models;
using TechParvaLEAO.Areas.Leave.Services;
using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using TechParvaLEAO.Areas.BulkUploads.Models;
using TechParvaLEAO.Areas.BulkUploads.Services;
using ClosedXML.Excel;
using System.Data.OleDb;
using TechParvaLEAO.Notification;
using TechParvaLEAO.Services;
using TechParvaLEAO.Models;
using Microsoft.AspNetCore.Identity;
using TechParvaLEAO.Areas.Organization.Controllers.MasterData;
using System.Net.Mail;
using Postal;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http.Headers;

namespace Cotecna.Areas.BulkUploads.Controllers
{
    /*
     * Controller for Timesheets
     */
    [Area("BulkUploads")]
    public class UploadsController : BaseViewController
    {
        private readonly ILogger<UploadsController> _logger;
        private IConfiguration Configuration;
        private readonly UploadService uploadService;
        private readonly ApplicationDbContext _context;
        private readonly IEmployeeServices employeeServices;
        private readonly EmailSenderOptions emailOptions;
        private readonly IEmailSenderEnhance _emailSender;
        private readonly IHostingEnvironment env;
        private SmtpClient client;
        private readonly IAuditLogServices _auditlog;
        private  UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailService emailService;
        //private readonly LeaveRequestServices _leaveRequestServices;
        UploadStatusViewModel uploadViewModel = new UploadStatusViewModel();
        UploadStatusViewModel dashboardViewModel = new UploadStatusViewModel();
        //  private IWebHostEnvironment Environment;

        public UploadsController(IEmailService emailService,
            IEmailViewRender emailViewRenderer,
            IHostingEnvironment env,
            IOptions<EmailSenderOptions> emailOptions,
            IAuditLogServices auditlog, ILogger<UploadsController> logger, IConfiguration _configuration, UploadService uploadService, ApplicationDbContext context, IEmployeeServices employeeServices, IEmailSenderEnhance emailSender, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.emailOptions = emailOptions.Value;
            this.emailService = emailService;
            this.env = env;
            ((EmailViewRender)emailViewRenderer).EmailViewDirectoryName = @"Emails";
            client = CreateSmtpClient();
            _auditlog = auditlog;
            _logger = logger;
            Configuration = _configuration;
            this.uploadService = uploadService;
            this._context = context;
            this.employeeServices = employeeServices;
            this._emailSender = emailSender;
            this.userManager = userManager;
            this._roleManager = roleManager;
         
        }

        private SmtpClient CreateSmtpClient()
        {
            var client = new SmtpClient(emailOptions.Host, emailOptions.Port)
            {
                Credentials = new NetworkCredential(emailOptions.UserName, emailOptions.Password),
                EnableSsl = emailOptions.EnableSSL
            };
            return client;
        }

       
        // GET: BulkUploads
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult Index()
        {
            ViewData["CanApproveReject"] = false;
            
            List<StatusViewModel> obj = uploadService.BindStatus();
            dashboardViewModel.StatusListItems = obj;
            //var ownTimesheets = timeSheetServices.GetOwnTimeSheets(GetEmployee());
            //var employees = await employeeServices.GetOwnEnumerable(User);
            //var overview = CreateViewModel(timesheetMonth, ownTimesheets, employees);
            return View("Index", dashboardViewModel);
        }

        private string EmployeeCodeExists(string employeeCode)
        {
            if (employeeCode != null || employeeCode != "")
            {
                var Employee = _context.Employees.FirstOrDefault(e => e.EmployeeCode == employeeCode);
                if (Employee != null)
                {
                    string employeecode = Employee.EmployeeCode;
                    return employeecode;
                }
            }
            return "";
        }
        [HttpPost]
        public async Task<IActionResult> Index(IFormFile postedFile)
        {
            ViewData["CanApproveReject"] = false;
            if (postedFile != null)
            {
                //Create a Folder.
                string path = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                //Save the uploaded Excel file.
                string fileName = Path.GetFileName(postedFile.FileName);
                string filePath = Path.Combine(path, fileName);
                using (FileStream stream = new FileStream(filePath, FileMode.Create))
                {
                    postedFile.CopyTo(stream);
                }

                //Read the connection string for the Excel file.
                try
                {
                    var al = new Auditlog_DM();
                    al.module = "Upload.cs";
                    al.url = "http://124.153.86.163/BulkUploads/Uploads";
                    al.comment = "Before Excel Connection string";
                    al.userid = User.Identity.Name;
                    al.line = "Before Excel Connection string";
                    al.path = "";
                    al.exception = "";
                    al.reportingto = "";
                    al.details = "Before Excel Connection string";
                    al.status = "";
                    _auditlog.InsertLog(al);
                }
                catch (Exception ex)
                {

                }
                string conString = this.Configuration.GetConnectionString("ExcelConString");
                DataTable dt = new DataTable();
                conString = string.Format(conString, filePath);

                using (OleDbConnection connExcel = new OleDbConnection(conString))
                {
                    using (OleDbCommand cmdExcel = new OleDbCommand())
                    {
                        using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                        {
                            cmdExcel.Connection = connExcel;

                            //Get the name of First Sheet.
                            connExcel.Open();
                            DataTable dtExcelSchema;
                            dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);

                            string sheetName = "Employee Import$"; //dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            connExcel.Close();

                            //Read Data from First Sheet.
                            connExcel.Open();
                            cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                            odaExcel.SelectCommand = cmdExcel;
                            odaExcel.Fill(dt);
                            connExcel.Close();
                        }
                    }
                }
                conString = this.Configuration.GetConnectionString("DefaultConnection");
                //string Created= uploadService.CreateTempTable(conString);
                //if (Created == "Created") {
                    
                    //Insert the Data read from the Excel file to Database Table.
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlCommand command = new SqlCommand("", con))
                        {
                            command.CommandTimeout = 300;
                            con.Open();

                            // 1．Create temp table
                            command.CommandText = @"CREATE TABLE #TempExcelStructure
                                (
                                   [Employee Code] [nvarchar](max) NOT NULL,
	[Employee] [nvarchar](max) NULL,
	[Designation] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Authorization Profile] [nvarchar](max) NULL,
	[Expense Profile] [nvarchar](max) NULL,
	[Teams] [nvarchar](max) NULL,
	[Account Number] [bigint] NULL,
	[Reporting To] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[Date of Joining] datetime2(7) NULL,
	[Date of Birth] datetime2(7) NULL,
	[Overtime Rule] [nvarchar](max) NULL,
	[Can Apply Mission Leaves] [nvarchar](max) NULL,
	[Can Create Forex Requests] [nvarchar](max) NULL,
	[Can have Credit card] [nvarchar](max) NULL,
	[Is Hr] [nvarchar](max) NULL,
	[On Field Employee] [nvarchar](max) NULL,
	[Specific Weekly-Off] [nvarchar](max) NULL,
LastWorkingDate datetime2(7) null,
ResignationDate datetime2(7) null,
SettlementDate datetime2(7) null,
SettlementAmount float null,
Status nvarchar(25) null,
Deactivated nvarchar(20) null 
                                )";
                        command.ExecuteNonQuery();

                        command.CommandText = @"CREATE TABLE #TempExcelStructureerror
                                (
                                   [Employee Code] [nvarchar](max) NOT NULL,
	[Employee] [nvarchar](max) NULL,
	[Designation] [nvarchar](max) NULL,
	[Location] [nvarchar](max) NULL,
	[Authorization Profile] [nvarchar](max) NULL,
	[Expense Profile] [nvarchar](max) NULL,
	[Teams] [nvarchar](max) NULL,
	[Account Number] [bigint] NULL,
	[Reporting To] [nvarchar](max) NULL,
	[Email] [nvarchar](max) NULL,
	[Gender] [nvarchar](max) NULL,
	[Date of Joining] datetime2(7) NULL,
	[Date of Birth] datetime2(7) NULL,
	[Overtime Rule] [nvarchar](max) NULL,
	[Can Apply Mission Leaves] [nvarchar](max) NULL,
	[Can Create Forex Requests] [nvarchar](max) NULL,
	[Can have Credit card] [nvarchar](max) NULL,
	[Is Hr] [nvarchar](max) NULL,
	[On Field Employee] [nvarchar](max) NULL,
	[Specific Weekly-Off] [nvarchar](max) NULL,
    LastWorkingDate datetime2(7) null,
    ResignationDate datetime2(7) null,
    SettlementDate datetime2(7) null,
    SettlementAmount float null,
    Status nvarchar(25) null,
    Deactivated nvarchar(20) null ,
    [ErrorMessage] [nvarchar](max) NULL
                                )";
                        command.ExecuteNonQuery();
                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                        {
                            //Set the database table name.

                            //sqlBulkCopy.DestinationTableName =  "dbo.#TempExcelStructure";
                            sqlBulkCopy.DestinationTableName = "#TempExcelStructure";

                            //[OPTIONAL]: Map the Excel columns with that of the database table.
                            ExcelStructure excelst = new ExcelStructure();
                            //  sqlBulkCopy.ColumnMappings.Add("Row_ID", "Row_ID");
                            sqlBulkCopy.ColumnMappings.Add("Employee Code", "Employee Code");
                            sqlBulkCopy.ColumnMappings.Add("Employee", "Employee");
                            sqlBulkCopy.ColumnMappings.Add("Designation", "Designation");
                            sqlBulkCopy.ColumnMappings.Add("Location", "Location");
                            sqlBulkCopy.ColumnMappings.Add("Authorization Profile", "Authorization Profile");
                            sqlBulkCopy.ColumnMappings.Add("Expense Profile", "Expense Profile");
                            sqlBulkCopy.ColumnMappings.Add("Teams", "Teams");
                            sqlBulkCopy.ColumnMappings.Add("Account Number", "Account Number");
                            sqlBulkCopy.ColumnMappings.Add("Reporting To", "Reporting To");
                            sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                            sqlBulkCopy.ColumnMappings.Add("Gender", "Gender");
                            sqlBulkCopy.ColumnMappings.Add("Date of Joining", "Date of Joining");

                            sqlBulkCopy.ColumnMappings.Add("Date of Birth", "Date of Birth");
                            sqlBulkCopy.ColumnMappings.Add("Overtime Rule", "Overtime Rule");
                            sqlBulkCopy.ColumnMappings.Add("Can Apply Mission Leaves", "Can Apply Mission Leaves");
                            sqlBulkCopy.ColumnMappings.Add("Can Create Forex Requests", "Can Create Forex Requests");
                            sqlBulkCopy.ColumnMappings.Add("Can have Credit card", "Can have Credit card");
                            sqlBulkCopy.ColumnMappings.Add("Is Hr", "Is Hr");
                            sqlBulkCopy.ColumnMappings.Add("On Field Employee", "On Field Employee");
                            sqlBulkCopy.ColumnMappings.Add("Specific Weekly-Off", "Specific Weekly-Off");
                            sqlBulkCopy.ColumnMappings.Add("LastWorkingDate", "LastWorkingDate");
                            sqlBulkCopy.ColumnMappings.Add("ResignationDate", "ResignationDate");
                            sqlBulkCopy.ColumnMappings.Add("SettlementDate", "SettlementDate");
                            sqlBulkCopy.ColumnMappings.Add("SettlementAmount", "SettlementAmount");
                            sqlBulkCopy.ColumnMappings.Add("Status", "Status");
                            sqlBulkCopy.ColumnMappings.Add("Deactivated", "Deactivated");
                            // con.Open();
                            sqlBulkCopy.WriteToServer(dt);
                            //con.Close();
                            try
                            {
                                var al = new Auditlog_DM();
                                al.module = "Upload.cs";
                                al.url = "http://124.153.86.163/BulkUploads/Uploads";
                                al.comment = "After Bulk Upload";
                                al.userid = User.Identity.Name;
                                al.line = "After Bulk Upload";
                                al.path = "";
                                al.exception = "";
                                al.reportingto = "";
                                al.details = "After Bulk Upload";
                                al.status = "";
                                _auditlog.InsertLog(al);
                            }
                            catch (Exception ex)
                            {

                            }
                            int InsertCount = 0, UpdateCount = 0, FailedCount = 0;
                            foreach (DataRow row in dt.Rows)
                            {
                                string Ecode = EmployeeCodeExists(row["Employee Code"].ToString());
                                if (Ecode == "")
                                {
                                    try
                                    {
                                        command.CommandText = @"INSERT into Employees(EmployeeCode,Name,DesignationId,LocationId,AuthorizationProfileId,ExpenseProfileId,TeamId,AccountNumber,ReportingToId,Email,
Gender,DateOfJoining,DateOfBirth,OvertimeMultiplierId,CanApplyMissionLeaves,CanCreateForexRequests,CanHoldCreditCard,IsHr,OnFieldEmployee,SpecificWeeklyOff,LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,Created_Date)
select [Employee Code],Employee,d.Id,l.Id,ap.Id,ep.Id,t.Id,[Account Number],e.ReportingToId,es.Email,
es.Gender, convert(datetime,[Date Of Joining],105),convert(datetime,[Date Of Birth],105),o.OvertimeMultiplier,case when[Can Apply Mission Leaves]='yes' then 1 else 0 end,case when [Can Create Forex Requests]='yes' then 1 else 0 end,
case when [Can have Credit Card] ='yes' then 1 else 0 end,case when[Is Hr] ='yes' then 1 else 0 end,case when[On Field Employee]='yes' then 1 else 0 end, case when [Specific Weekly-Off] ='yes' then 1 else 0 end,es.LastWorkingDate,es.ResignationDate,es.SettlementDate,es.SettlementAmount
,case when es.Status ='Resigned' then 1  when es.Status ='Service Terminated' then 2 else 0 end,case when isnull(es.Deactivated,'no') ='yes' then 1 else 0 end,GETDATE()
  from Employees e  inner join #TempExcelStructure es on e.EmployeeCode=es.[Employee Code] inner join Employees m on es.[Reporting To] = m.Name  left join Designations d on es.Designation=d.[Name] left join Locations l on es.[Location]=l.[Name]
  left join ApprovalLimitProfiles ap on es.[Authorization Profile]= ap.[Name]  left join ExpenseProfiles ep on es.[Expense Profile]=ep.Name
  left join Team t on es.Teams=t.TeamName  left join OvertimeRule o on es.[Overtime Rule]=o.[Name]   where [Employee Code] ='" + row["Employee Code"] + "'";

                                        command.ExecuteNonQuery();
                                        InsertCount = InsertCount + 1;

                                       // Employee employee = new Employee();
                                        //var employee = await _context.Employees.Where(m => m.EmployeeCode =row["Employee Code"].ToString());
                                        var employee = _context.Employees.Where(m => m.EmployeeCode == row["Employee Code"].ToString()).FirstOrDefault();
                                      //  employee.EmployeeCode = row["Employee Code"].ToString();
                                        //employee.Name = row["Employee"].ToString();
                                        //employee.DesignationId = 1;
                                        //employee.LocationId = 1;
                                        //employee.AuthorizationProfileId = 1;
                                        //employee.ExpenseProfileId = 1;
                                        //employee.TeamId = 1;
                                        //employee.AccountNumber= row["Account Number"].ToString();
                                        //employee.ReportingToId = 12;
                                        //employee.Email = row["Email"].ToString();
                                        //employee.Gender = row["Gender"].ToString();
                                        //employee.DateOfBirth = Convert.ToDateTime(row["Date of Birth"]);
                                        //employee.DateOfJoining=Convert.ToDateTime(row["Date of Joining"]);
                                        //employee.OvertimeMultiplierId = 1;
                                        //employee.CanApplyMissionLeaves = (row["Can Apply Mission Leaves"].ToString() == "yes") ? true:false ;
                                        //employee.CanCreateForexRequests = (row["Can Create Forex Requests"].ToString() == "yes") ? true : false;
                                        //employee.CanHoldCreditCard = (row["Can have Credit card"].ToString() == "yes") ? true : false;
                                        //employee.IsHr = (row["Is Hr"].ToString() == "yes") ? true : false;
                                        //employee.OnFieldEmployee = (row["On Field Employee"].ToString() == "yes") ? true : false;
                                        //employee.SpecificWeeklyOff = (row["Specific Weekly-Off"].ToString() == "yes") ? true : false;
                                        //employee.Created_Date = DateTime.Now;
                                        //employee.Created_by = User.Identity.Name;

                                        //_context.Add(employee);
                                        //await _context.SaveChangesAsync();

                                        if (employee.Email != null && !"".Equals(employee.Email))
                                        {
                                            await userManager.CreateAsync(new ApplicationUser
                                            {
                                                Email = employee.Email,
                                                UserName = employee.EmployeeCode,
                                                EmployeeProfileId = employee.Id
                                            }, "Cotecna@123"); ;

                                            ApplicationUser user = await userManager.FindByNameAsync(employee.EmployeeCode);
                                           // var user = await _userManager.FindByIdAsync(userId);
                                            if (user == null)
                                            {
                                                ViewBag.ErrorMessage = $"User with Id = {user.UserName} cannot be found";
                                                return View("NotFound");
                                            }
                                            var roles = await userManager.GetRolesAsync(user);

                                            var result = await userManager.RemoveFromRolesAsync(user, roles);
                                            if (!result.Succeeded)
                                            {
                                                ModelState.AddModelError("", "Cannot remove user existing roles");
                                                //return View(model);
                                            }
                                            var model = new List<UserRolesViewModel>();
                                           
                                                var role = _roleManager.Roles.FirstOrDefault();
                                                var userRolesViewModel = new UserRolesViewModel
                                                {
                                                    RoleId = role.NormalizedName,
                                                    RoleName = role.NormalizedName
                                                };
                                               
                                                userRolesViewModel.IsSelected = true;
                                                model.Add(userRolesViewModel);
                                            
                                            result = await userManager.AddToRolesAsync(user,
                                                model.Where(x => x.IsSelected).Select(y => y.RoleId));
                                            if (!result.Succeeded)
                                            {
                                                ModelState.AddModelError("", "Cannot add selected roles to user");
                                                //return View(model);
                                            }
                                        }
                                        //string test = row["Email"].ToString();
                                        //string FNAme = test.Substring(0, test.IndexOf("."));
                                        //string LNAMe= test.Substring(test.IndexOf(".") + 1, test.IndexOf("@") - test.IndexOf(".") - 1);
                                       
                                        sendEmail(row["Email"].ToString(), row["Employee Code"].ToString(),employee);
                                    }
                                    catch (Exception ex)
                                    {
                                        string exception = ex.Message.ToString();
                                        exception = exception.Replace("'", "''");
                                        command.CommandText = @"INSERT into #TempExcelStructureerror([Employee Code],Employee,Designation,Location,[Authorization Profile],[Expense Profile],Teams,[Account Number],[Reporting To],Email,
Gender,[Date Of Joining],[Date Of Birth],[Overtime Rule],[Can Apply Mission Leaves],[Can Create Forex Requests],[Can have Credit Card],[Is Hr],[On Field Employee],[Specific Weekly-Off],LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,ErrorMessage)
select [Employee Code],Employee,Designation,Location,[Authorization Profile],[Expense Profile],Teams,[Account Number],[Reporting To],Email,
Gender,[Date Of Joining],[Date Of Birth],[Overtime Rule],[Can Apply Mission Leaves],[Can Create Forex Requests],[Can have Credit Card],[Is Hr],[On Field Employee],[Specific Weekly-Off],LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,'" + exception +
  "' from #TempExcelStructure  where [Employee Code] ='" + row["Employee Code"] + "'";

                                        command.ExecuteNonQuery();
                                        FailedCount = FailedCount + 1;
                                        // throw ex;
                                    }

                                }
                                else {
                                    try
                                    {
                                        command.CommandText = @"UPDATE e SET Name = isnull(es.Employee,e.Name),DesignationId=isnull(d.Id,e.DesignationId),
  LocationId=isnull(l.Id,e.LocationId),AuthorizationProfileId=isnull(ap.Id,e.AuthorizationProfileId),ExpenseProfileId=isnull(ep.Id,e.ExpenseProfileId),teamlist=isnull(t.Id,e.teamlist),
  AccountNumber=isnull(es.[Account Number],e.AccountNumber),ReportingToId=isnull(m.Id,e.ReportingToId),Email=isnull(es.Email,e.Email),Gender=isnull(es.Gender,e.Gender),DateOfJoining=isnull(convert(datetime,es.[Date Of Joining],105),e.DateOfJoining),
  DateOfBirth=isnull(convert(datetime,es.[Date Of Birth],105),e.DateOfBirth),OvertimeMultiplierId=isnull(o.OvertimeMultiplier,e.OvertimeMultiplierId),CanApplyMissionLeaves=case when isnull(es.[Can Apply Mission Leaves],e.CanApplyMissionLeaves)='yes' then 1 else 0 end,
  CanCreateForexRequests= case when isnull(es.[Can Create Forex Requests],e.CanCreateForexRequests)='yes' then 1 else 0 end,CanHoldCreditCard= case when isnull(es.[Can have Credit Card],e.CanHoldCreditCard)='yes' then 1 else 0 end ,
  ISHr=case when isnull(es.[Is Hr],e.IsHr)='yes' then 1 else 0 end,OnFieldEmployee=case when isnull(es.[On Field Employee],e.OnFieldEmployee)='yes' then 1 else 0 end,SpecificWeeklyOff= case when isnull(es.[Specific Weekly-Off],e.SpecificWeeklyOff)='yes' then 1 else 0 end,  Modified_Date=GETDATE()
,LastWorkingDate=es.LastWorkingDate,ResignationDate=es.ResignationDate,SettlementDate=es.SettlementDate,SettlementAmount=es.SettlementAmount,Status=case when es.Status ='Resigned' then 1  when es.Status ='Service Terminated' then 2 else 0 end,Deactivated =case when isnull(es.Deactivated,e.Deactivated)='yes' then 1 else 0 end
  from Employees e  inner join #TempExcelStructure es on e.EmployeeCode=es.[Employee Code] inner join Employees m on es.[Reporting To] = m.Name  left join Designations d on es.Designation=d.[Name] left join Locations l on es.[Location]=l.[Name]
  left join ApprovalLimitProfiles ap on es.[Authorization Profile]= ap.[Name]  left join ExpenseProfiles ep on es.[Expense Profile]=ep.Name
  left join Team t on es.Teams=t.TeamName  left join OvertimeRule o on es.[Overtime Rule]=o.[Name] where es.[Employee Code] ='" + row["Employee Code"] + "'";

                                        command.ExecuteNonQuery();
                                        UpdateCount = UpdateCount + 1;
                                    }
                                    catch (Exception ex)
                                    {
                                        string exception =  ex.Message.ToString();
                                        exception = exception.Replace("'","''");
                                        command.CommandText = @"INSERT into #TempExcelStructureerror([Employee Code],Employee,Designation,Location,[Authorization Profile],[Expense Profile],Teams,[Account Number],[Reporting To],Email,
Gender,[Date Of Joining],[Date Of Birth],[Overtime Rule],[Can Apply Mission Leaves],[Can Create Forex Requests],[Can have Credit Card],[Is Hr],[On Field Employee],[Specific Weekly-Off],LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,ErrorMessage)
select [Employee Code],Employee,Designation,Location,[Authorization Profile],[Expense Profile],Teams,[Account Number],[Reporting To],Email,
Gender,[Date Of Joining],[Date Of Birth],[Overtime Rule],[Can Apply Mission Leaves],[Can Create Forex Requests],[Can have Credit Card],[Is Hr],[On Field Employee],[Specific Weekly-Off],LastWorkingDate,ResignationDate,SettlementDate,SettlementAmount,Status,Deactivated,'" + exception +
  "' from #TempExcelStructure  where [Employee Code] ='" + row["Employee Code"] + "'";

                                        command.ExecuteNonQuery();
                                        FailedCount = FailedCount + 1;
                                        //throw ex;
                                    }
                                }
                            }
                            command.CommandText = @"select * from #TempExcelStructureerror";
                            using (SqlDataAdapter sda = new SqlDataAdapter(command))
                            {
                                DataTable dttemp = new DataTable();
                                sda.Fill(dttemp);

                                command.CommandText = @"insert into [UploadStatus](UploadedOn,UploadedBy,AddedRecords,UpdatedRecords,FailedREcords) values(GETDATE(),'"+ User.Identity.Name +"'," + InsertCount + "," + UpdateCount + "," + FailedCount + ")";
                                command.ExecuteNonQuery();
                                try
                                {
                                    var al = new Auditlog_DM();
                                    al.module = "Upload.cs";
                                    al.url = "http://124.153.86.163/BulkUploads/Uploads";
                                    al.comment = "After added in Upload Status";
                                    al.userid = User.Identity.Name;
                                    al.line = "After added in Upload Status";
                                    al.path = "";
                                    al.exception = "";
                                    al.reportingto = "";
                                    al.details = "After added in Upload Status";
                                    al.status = "";
                                    _auditlog.InsertLog(al);
                                }
                                catch (Exception ex)
                                {

                                }
                                dashboardViewModel = new UploadStatusViewModel();
                                List<StatusViewModel> obj = uploadService.BindStatus();
                                dashboardViewModel.StatusListItems = obj;
                                if (dttemp.Rows.Count > 0)
                                {
                                    using (XLWorkbook wb = new XLWorkbook())
                                    {
                                        wb.Worksheets.Add(dttemp, "Employee Import");

                                        using (MemoryStream stream = new MemoryStream())
                                        {
                                            wb.SaveAs(stream);
                                            return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "DownloadTemplateError.xlsx");
                                        }
                                    }
                                }
                            }

                       
                            //                            command.CommandText = @"
                            //MERGE  Employees as e
                            //USING #TempExcelStructure  as es
                            //   ON (e.EmployeeCode= es.ECode)
                            //WHEN MATCHED THEN UPDATE SET
                            //  Name = es.Employee,DesignationId=es.Designation,
                            //  LocationId=es.Location,AuthorizationProfileId=es.AuthorizationProfile,ExpenseProfileId=es.ExpenseProfile,TeamId=es.Teams,
                            //  AccountNumber=es.AccountNumber,ReportingToId=es.ReportingTo,Email=es.Email,Gender=es.Gender,DateOfJoining=convert(datetime,es.DateOfJoining,105),
                            //  DateOfBirth=convert(datetime,es.DateOfBirth,105),OvertimeMultiplierId=es.OvertimeRule,CanApplyMissionLeaves=es.CanApplyMissionLeaves,CanCreateForexRequests=es.CanCreateForexRequests,
                            //  CanHoldCreditCard=CanhaveCreditCard,ISHr=es.IsHr,OnFieldEmployee=es.OnFieldEmployee,SpecificWeeklyOff=es.SpecificWeeklyOff
                            //WHEN NOT MATCHED THEN INSERT (EmployeeCode,Name,DesignationId,LocationId,AuthorizationProfileId,ExpenseProfileId,TeamId,AccountNumber,ReportingToId,Email,
                            //Gender,DateOfJoining,DateOfBirth,OvertimeMultiplierId,CanApplyMissionLeaves,CanCreateForexRequests,CanHoldCreditCard,IsHr,OnFieldEmployee,SpecificWeeklyOff,SettlementAmount,Status,Deactivated)
                            //VALUES (
                            // ECode,Employee,Designation,Location,AuthorizationProfile,ExpenseProfile,Teams,AccountNumber,ReportingTo,Email,Gender,convert(datetime,DateOfJoining,105),
                            //convert(datetime,DateOfBirth,105),OvertimeRule,CanApplyMissionLeaves,CanCreateForexRequests,CanhaveCreditCard,IsHr,OnFieldEmployee,SpecificWeeklyOff,0,1,0
                            //);

                            //drop table #TempExcelStructure;
                            //                            ";

                            //                            command.ExecuteNonQuery();
                            //DataSet ds = new DataSet();

                            //try
                            //{
                            //    SqlCommand cmd = new SqlCommand("sp_BulkInsert_EmployeeDetails",con);
                            //    cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Upload";
                            //    cmd.CommandType = CommandType.StoredProcedure;
                            //    SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                            //    DataTable dt1 = new DataTable();
                            //    adpt.Fill(dt1);

                            //    ds.Tables.Add(dt1);

                            //   // return ds;
                            //}
                            //catch (Exception ex)
                            //{
                            //    throw ex;
                            //}

                            //ds = (DataSet)uploadService.Insert_UpdateEmployeeDetails();
                        }
                        }
                    }
                
                
            }

            return View("Index", dashboardViewModel);
        }

        public async Task SendEmailAsync(MailMessage mailMessage)
        {
            //try
            //{
            //    var al = new Auditlog_DM();
            //    al.module = "HanfireEmailsender.cs";
            //    al.url = "SendEmailAsync";
            //    al.comment = mailMessage.ToString();
            //    al.userid = "";
            //    al.line = "Before Sending";
            //    al.path = "";
            //    al.exception = "";
            //    al.reportingto = "";
            //    al.details = mailMessage.To.ToString();
            //    al.status = "";
            //    _auditlog.InsertLog(al);
            //}
            //catch (Exception ex)
            //{

            //}

            mailMessage.From = new MailAddress(emailOptions.FromAddress);
            try
            {
                await client.SendMailAsync(mailMessage);
            }
            catch (SmtpException e)
            {
                client = CreateSmtpClient();
            }

            //try
            //{
            //    var al = new Auditlog_DM();
            //    al.module = "HanfireEmailsender.cs";
            //    al.url = "SendEmailAsync";
            //    al.comment = mailMessage.ToString();
            //    al.userid = "";
            //    al.line = "After Sending";
            //    al.path = "";
            //    al.exception = "";
            //    al.reportingto = "";
            //    al.details = mailMessage.To.ToString();
            //    al.status = "";
            //    _auditlog.InsertLog(al);
            //}
            //catch (Exception ex)
            //{

            //}
        }

        public async Task sendEmail( string Email,string EmployeeCode, Employee employee)
        {
            var url = "";

            url = Request.Scheme + "://" + Request.Host.Host;
            if (Request.Host.Port.ToString() != "")
            {
                url = url + ":" + Request.Host.Port;
            }

            var notification = new NotificationEventModel
            {

                Type = "Employee",
                Event = "User Created"
            };

           var configuration = uploadService.get_NoticationConfiguration(notification).ToList();
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
                EmployeeDataList = null,
                AcceptButtonUrl = "",
                RejectButtonUrl = "",
                DetailButtonUrl = "",
                FromDate = DateTime.Now,
                ToDate = DateTime.Now,
                Receiver = employee
            };
            try
            {
                MailMessage message = await emailService.CreateMailMessageAsync(email);
                try
                {
                   
                    message.From = new MailAddress(emailOptions.FromAddress,configuration[0].SubjectLine);
                    message.To.Add(employee.Email);
                    message.Subject= (configuration[0].SubjectLine);
                    await SendEmailAsync(message);
                }
                catch (Exception e)
                {
                    await _emailSender.SendEmailAsync(email);
                }
                
            }
            catch (Exception e)
            {
              
            }
            
        }
        [HttpPost]
        public IActionResult Export()
        {
            ViewData["CanApproveReject"] = false;

            DataSet ds = new DataSet();
            ds = (DataSet)uploadService.DownloadEmployeeDetails();
            using (XLWorkbook wb = new XLWorkbook())
            {
                DataTable dt = ds.Tables[0];
                DataTable dt1 = ds.Tables[1];
                DataTable dt2 = ds.Tables[2];
                DataTable dt3 = ds.Tables[3];
                DataTable dt4 = ds.Tables[4];
                DataTable dt5 = ds.Tables[5];
               DataTable dt6 = ds.Tables[6];
                DataTable dt7 = ds.Tables[7];

                wb.Worksheets.Add(dt, "Employee Import");
                wb.Worksheets.Add(dt1,"Designation Master");
                wb.Worksheets.Add(dt2,"Location Master");
                wb.Worksheets.Add(dt3,"AuthorzationProfile Master");

                wb.Worksheets.Add(dt4, "ExpenseProfile Master");
                wb.Worksheets.Add(dt5, "Teams Master");
                wb.Worksheets.Add(dt6, "Employee Reporting Master");
                wb.Worksheets.Add(dt7,"OverTime Master");
                 

                // where "A1" to "G1" is your header range

                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    //            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
                    //var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                    //result.Content = new StreamContent(stream);
                    //result.Content.Headers.ContentType =
                    //    new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
                    string contentype = new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet").ToString();
                   // CreateExcelFile.CreateExcelDocument(ds, "C:\\Sample.xlsx");

                    return File(stream.ToArray(),contentype, "DownloadTemplate.xlsx");
                }
            }
        }


    }
}
