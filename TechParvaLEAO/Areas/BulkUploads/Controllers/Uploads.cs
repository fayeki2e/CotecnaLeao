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
        //  private IWebHostEnvironment Environment;

        public UploadsController(ILogger<UploadsController> logger, IConfiguration _configuration, UploadService uploadService)
        {
            _logger = logger;
            Configuration = _configuration;
            this.uploadService = uploadService;
        }


        /*
         * Show List of own timesheets
         */
        // GET: BulkUploads
        [Authorize(Roles = AuthorizationRoles.MANAGER + "," + AuthorizationRoles.EMPLOYEE)]
        public ActionResult Index()
        {
            ViewData["CanApproveReject"] = false;
            //var ownTimesheets = timeSheetServices.GetOwnTimeSheets(GetEmployee());
            //var employees = await employeeServices.GetOwnEnumerable(User);
            //var overview = CreateViewModel(timesheetMonth, ownTimesheets, employees);
            return View("Index");
        }

        [HttpPost]
        public IActionResult Index(IFormFile postedFile)
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
                            string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
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

                //Insert the Data read from the Excel file to Database Table.
                conString = this.Configuration.GetConnectionString("DefaultConnection");
                using (SqlConnection con = new SqlConnection(conString))
                {
                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                    {
                        //Set the database table name.
                        sqlBulkCopy.DestinationTableName = "dbo.ExcelStructure";

                        //[OPTIONAL]: Map the Excel columns with that of the database table.
                        ExcelStructure excelst = new ExcelStructure();
                        sqlBulkCopy.ColumnMappings.Add("Row_ID", "Row_ID");
                        sqlBulkCopy.ColumnMappings.Add("Employee Code", "ECode");
                        sqlBulkCopy.ColumnMappings.Add("Employee", "Employee");
                        sqlBulkCopy.ColumnMappings.Add("Designation","Designation");
                        sqlBulkCopy.ColumnMappings.Add("Location", "Location");
                        sqlBulkCopy.ColumnMappings.Add("Authorization Profile","AuthorizationProfile");
                        sqlBulkCopy.ColumnMappings.Add("Expense Profile", "ExpenseProfile");
                        sqlBulkCopy.ColumnMappings.Add("Teams", "Teams");
                        sqlBulkCopy.ColumnMappings.Add("Account Number", "AccountNumber");
                        sqlBulkCopy.ColumnMappings.Add("Reporting To", "ReportingTo");
                        sqlBulkCopy.ColumnMappings.Add("Email", "Email");
                        sqlBulkCopy.ColumnMappings.Add("Gender", "Gender");
                        sqlBulkCopy.ColumnMappings.Add("Date of Joining", "DateofJoining");
                        
                        sqlBulkCopy.ColumnMappings.Add("Date of Birth", "DateofBirth");
                        sqlBulkCopy.ColumnMappings.Add("Overtime Rule", "OvertimeRule");
                        sqlBulkCopy.ColumnMappings.Add("Can Apply Mission Leaves", "CanApplyMissionLeaves");
                        sqlBulkCopy.ColumnMappings.Add("Can Create Forex Requests", "CanCreateForexRequests");
                        sqlBulkCopy.ColumnMappings.Add("Can have Credit card", "CanhaveCreditcard");
                        sqlBulkCopy.ColumnMappings.Add("Is Hr", "IsHr");
                        sqlBulkCopy.ColumnMappings.Add("On Field Employee", "OnFieldEmployee");
                        sqlBulkCopy.ColumnMappings.Add("Specific Weekly-Off", "SpecificWeeklyOff");
                        con.Open();
                        sqlBulkCopy.WriteToServer(dt);
                        con.Close();

                        DataSet ds = new DataSet();

                        // dt = (DataTable)financeReportsServices.GetBalancePayableOrReceivable_Report();

                        ds = (DataSet)uploadService.Insert_UpdateEmployeeDetails();
                    }
                }
            }

            return View("Index");
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
                //DataTable dt1 = ds.Tables[1];
                //DataTable dt2 = ds.Tables[2];
                //DataTable dt3 = ds.Tables[3];
                //DataTable dt4 = ds.Tables[4];
                //DataTable dt5 = ds.Tables[5];
               // DataTable dt6 = ds.Tables[6];
                wb.Worksheets.Add(dt);
                //wb.Worksheets.Add(dt1);
                //wb.Worksheets.Add(dt2);
                //wb.Worksheets.Add(dt3);

                //wb.Worksheets.Add(dt4);
                //wb.Worksheets.Add(dt5);
                using (MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Grid.xlsx");
                }
            }
        }


    }
}
