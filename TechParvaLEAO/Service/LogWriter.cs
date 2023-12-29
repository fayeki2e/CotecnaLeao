using System;
using System.IO;
using System.Reflection;

namespace TechParvaLEAO.Services
{
    public class LogWriter
    { 
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
