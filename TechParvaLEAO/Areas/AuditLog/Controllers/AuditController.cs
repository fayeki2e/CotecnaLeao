using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Text;
using System;
using TechParvaLEAO.Controllers;

namespace TechParvaLEAO.Areas.AuditLog.Controllers
{
    [Area("AuditLog")]
    public class IndexController : BaseViewController
    {
        public IActionResult Index()
        {
            string query = "";
            query = "  select * from tbl_audit orderby id desc";
            var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"];


            if (!string.IsNullOrWhiteSpace(query))
            {
                try
                {
                    var table = new StringBuilder();

                    using (var con = new SqlConnection(AppName.ToString()))
                    {
                        con.Open();
                        var cmd = con.CreateCommand();

                        cmd.CommandText = query;

                        var reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            table.AppendLine("<table class=\"table table-bordered\">");
                            table.AppendLine("<thead><tr>");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.AppendLine("<th>");
                                table.AppendLine(reader.GetName(i));
                                table.AppendLine("</th>");
                            }
                            table.AppendLine("</tr></thead>");
                            table.AppendLine("<tbody>");
                            while (reader.Read())
                            {
                                table.AppendLine("<tr>");

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    table.AppendLine("<td>");
                                    table.AppendLine(reader[i].ToString());
                                    table.AppendLine("</td>");
                                }
                                table.AppendLine("</tr>");
                            }
                            table.AppendLine("</tbody>");
                            table.AppendLine("</table>");
                            ViewBag.Result = table.ToString();
                        }
                        else
                        {
                            ViewBag.Result = string.Format("{0} records affected", reader.RecordsAffected);
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Result = ex.Message;
                }
            }

           // return View("Index");

            return View();
        }

        public ActionResult Editor()
        {
            string query = "";
            query = "  select * from tbl_audit order by id desc";

            var AppName = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("ConnectionStrings")["DefaultConnection"];


            if (!string.IsNullOrWhiteSpace(query))
            {
                try
                {
                    var table = new StringBuilder();

                    using (var con = new SqlConnection(AppName.ToString()))
                    {
                        con.Open();
                        var cmd = con.CreateCommand();

                        cmd.CommandText = query;

                        var reader = cmd.ExecuteReader();

                        if (reader.HasRows)
                        {
                            table.AppendLine("<table class=\"table table-bordered\">");
                            table.AppendLine("<thead><tr>");
                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                table.AppendLine("<th>");
                                table.AppendLine(reader.GetName(i));
                                table.AppendLine("</th>");
                            }
                            table.AppendLine("</tr></thead>");
                            table.AppendLine("<tbody>");
                            while (reader.Read())
                            {
                                table.AppendLine("<tr>");

                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    table.AppendLine("<td>");
                                    table.AppendLine(reader[i].ToString());
                                    table.AppendLine("</td>");
                                }
                                table.AppendLine("</tr>");
                            }
                            table.AppendLine("</tbody>");
                            table.AppendLine("</table>");
                            ViewBag.Result = table.ToString();
                        }
                        else
                        {
                            ViewBag.Result = string.Format("{0} records affected", reader.RecordsAffected);
                        }

                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    ViewBag.Result = ex.Message;
                }
            }

            return View();
        }

    }
}
