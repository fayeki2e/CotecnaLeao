using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TechParvaLEAO.Data;
using TechParvaLEAO.Areas.Attendance.Models;
using TechParvaLEAO.Areas.Organization.Models;
using System.Data.SqlClient;
using System.Data;
using TechParvaLEAO.Service;
using TechParvaLEAO.Areas.BulkUploads.Models;
using TechParvaLEAO.Models;
using TechParvaLEAO.Notification;

namespace TechParvaLEAO.Areas.BulkUploads.Services
{
    public class UploadService
    {
        private IApplicationRepository _context;

        private readonly IDBConnectionEnhance dbConnection; private readonly ApplicationDbContext _dbContext;
        List<StatusViewModel> obj1 = new List<StatusViewModel>();


        public UploadService(IApplicationRepository context, IDBConnectionEnhance DBconnection, ApplicationDbContext dbContext)
        {
            _context = context;
            dbConnection = DBconnection;
            _dbContext = dbContext;
        }
        public string CreateTempTable(string constr)
        {
            string str = "";

            try
            {
               
                SqlConnection con = new SqlConnection(constr);
                
                SqlCommand cmd = new SqlCommand("sp_BulkInsert_EmployeeDetails", con);
                con.Open();
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Create";
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.ExecuteNonQuery();
                con.Close();
                str = "Created";
                return str;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public DataSet Insert_UpdateEmployeeDetails()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_BulkInsert_EmployeeDetails", dbConnection.Get_DB_Connection());
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Upload";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                adpt.Fill(dt);

                ds.Tables.Add(dt);

                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public IEnumerable<EmailNotificationConfiguration> get_NoticationConfiguration(NotificationEventModel notification)
        {
            var configuration = _dbContext.EmailNotificationConfiguration.
                         Where(n => n.Type == notification.Type).
                         Where(n => n.Name == notification.Event).ToList();


            return configuration;


        }
        public DataSet DownloadEmployeeDetails()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_BulkInsert_EmployeeDetails", dbConnection.Get_DB_Connection());
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Download";
                cmd.CommandType = CommandType.StoredProcedure;
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                adpt.Fill(ds);
                return ds;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }

        public List<StatusViewModel> BindStatus()
        {
            DataTable dt = new DataTable();

            try
            {
                SqlCommand cmd = new SqlCommand("Select * from [UploadStatus]", dbConnection.Get_DB_Connection());
                
                SqlDataAdapter adpt = new SqlDataAdapter(cmd);
                adpt.Fill(dt);
                if (dt.Rows.Count > 0)

                {

                    for (int i = 0; i < dt.Rows.Count; i++)

                    {

                        StatusViewModel info = new StatusViewModel();

                        info.Row_ID = Convert.ToInt32( dt.Rows[i]["Row_ID"]);
                        info.UploadedOn = Convert.ToDateTime(dt.Rows[i]["UploadedOn"]);
                        info.UploadedBy = dt.Rows[i]["UploadedBy"].ToString();
                        info.AddedRecords = Convert.ToInt32(dt.Rows[i]["AddedRecords"]);
                        info.UpdatedRecords = Convert.ToInt32(dt.Rows[i]["UpdatedRecords"]);
                        info.FailedRecords = Convert.ToInt32(dt.Rows[i]["FailedRecords"]);
                        obj1.Add(info);

                    }

                }
                return obj1;
            }
            catch (Exception ex)
            {
                throw ex;
            }


        }
    }
}
