using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace TechParvaLEAO.Services
{
    public interface IAuditLogServices
    {
       void InsertLog(Auditlog_DM AL);
        void getAuditlog();

    }

    public class Auditlog_DM
    {
        public string module { get; set; }
        public string url { get; set; }
        public string comment { get; set; }
        public string userid { get; set; }
        public string line { get; set; }
        public string path { get; set; }
        public string exception { get; set; }
        public string reportingto { get; set; }
        public string details { get; set; }
        public string status { get; set; }

    }
    public class AuditLog: IAuditLogServices
    { 
        public void getAuditlog()
        {

        }
        public void InsertLog(Auditlog_DM AL)
        {      try
            {
                var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"];
                using (var con = new SqlConnection(AppName.ToString()))
                {
                    string query = "INSERT INTO tbl_audit (module, url,comment,userid,line,exception,reportingto,details,datetimestamp) VALUES (@module, @url,@comment,@userid,@line,@exception,@reportingto,@details,@datetimestamp)";
                    SqlCommand command = new SqlCommand(query, con);
                    command.Parameters.AddWithValue("@module", AL.module);
                    command.Parameters.AddWithValue("@url", AL.url);
                    command.Parameters.AddWithValue("@comment", AL.comment);
                    command.Parameters.AddWithValue("@userid", AL.userid);
                    command.Parameters.AddWithValue("@line", AL.line);
                    command.Parameters.AddWithValue("@exception", AL.exception);
                    command.Parameters.AddWithValue("@reportingto", AL.reportingto);
                    command.Parameters.AddWithValue("@details", AL.details);
                    command.Parameters.AddWithValue("@status", AL.status);
                    command.Parameters.AddWithValue("@datetimestamp", System.DateTime.Now);
                    con.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    con.Close();



                }
            } catch (Exception ex)
            {

            }

        }


        public static void WriteLog(string FunctionName, string logMessage)
        {
            string m_exePath = string.Empty;

            m_exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\wwwroot\\ErrorLog\\" + System.DateTime.Today.ToString("MM-dd-yyyy")+ "_log.txt"))
                {
                    Log(FunctionName,logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static void Log(string FunctionName,string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine(" Error in Function :{0}", FunctionName);
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
