using System.Net.Http;
using System.Net;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Xml.Linq;
using System.Net.Http.Headers;
using Microsoft.SharePoint.Client;
using System.Text;
using System.Configuration;
using Microsoft.Extensions.Configuration;

using TechParvaLEAO.Services;
using Microsoft.Extensions.Options;
using System.Reflection;
using Postal;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Data.SqlClient;

namespace TechParvaLEAO.Service
{
    public class DBConnectionOptions
    {
        public string DefaultConnection { get; set; }


    }



    public interface IDBConnectionEnhance
    {
        SqlConnection Get_DB_Connection();

    }



    public class DBConnection_service : IDBConnectionEnhance
    {
        private readonly DBConnectionOptions DBConnectionOptions;


        public DBConnection_service(IOptions<DBConnectionOptions> DBconnectionOptions)
        {
            this.DBConnectionOptions = DBconnectionOptions.Value;


        }
        public SqlConnection Get_DB_Connection()
        {
            //   SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings[DBConnectionOptions.DefaultConnection].ConnectionString);
            SqlConnection connection = new SqlConnection(DBConnectionOptions.DefaultConnection);
            return connection;
        }

    }

}
