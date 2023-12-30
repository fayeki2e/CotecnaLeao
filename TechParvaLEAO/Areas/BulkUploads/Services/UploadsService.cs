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

namespace TechParvaLEAO.Areas.BulkUploads.Services
{
    public class UploadService
    {
        private IApplicationRepository _context;

        private readonly IDBConnectionEnhance dbConnection;
        public UploadService(IApplicationRepository context, IDBConnectionEnhance DBconnection)
        {
            _context = context;
            dbConnection = DBconnection;
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

        public DataSet DownloadEmployeeDetails()
        {
            DataSet ds = new DataSet();

            try
            {
                SqlCommand cmd = new SqlCommand("sp_BulkInsert_EmployeeDetails", dbConnection.Get_DB_Connection());
                cmd.Parameters.Add("@Action", SqlDbType.NVarChar).Value = "Download";
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

    }
}
