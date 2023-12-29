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


namespace TechParvaLEAO.Service
{
     public class SharePointOptions
    {
        public string domain { get; set; }
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string site { get; set; }
        public string upload_folder { get; set; }

    }

    public class SharePointUploadData
    {
        public string filename { get; set; }
        public string foldername { get; set; }
        public string filePath { get; set; }
 

    }


    public interface IsharepointEnhance 
    {
        Task Upload_file_sharepoint_Async(SharePointUploadData SPUD);

        Task DownloadFile(SharePointUploadData SPUD);
    }

    public class SharePoint_service : IsharepointEnhance
    {
        private readonly SharePointOptions sharepointOptions;

        public readonly SharePointOptions SPO;
        public SharePoint_service(IOptions<SharePointOptions> sharepointOptions)
        {
            this.sharepointOptions = sharepointOptions.Value;
             

        }
        public SharePointOptions get_sharepointOption()
        {
           var SPO = new SharePointOptions
            {
                domain = sharepointOptions.domain,
               client_id = sharepointOptions.client_id,
               client_secret = sharepointOptions.client_secret,
               site = sharepointOptions.site,
               upload_folder = sharepointOptions.upload_folder,

           };
   

            return SPO;
        }
        public SharePoint_service()
        {
            

        }
       
        public async Task<string> get_tanent()
        {

           var SPO=get_sharepointOption();

            string domain = SPO.domain;


        var tanent_id = "";
            try
            {
                //const string SITE_URL = "https://i2ec.sharepoint.com/_vti_bin/client.svc/";
                string SITE_URL = "https://" + domain + "/_vti_bin/client.svc/";

                var httpclient = new HttpClient();
                var client = new WebClient();
                // httpclient.DefaultRequestHeaders.Add("Authorization", "application/json;odata=nometadata");
                httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer");
                var responseMessage = await httpclient.PostAsync(SITE_URL, null);

                var authenticate = responseMessage.Headers.WwwAuthenticate.ToString();
                 tanent_id = authenticate.Split(" ")[1].Split("=")[1].Split(",")[0].ToString().Replace("\"", "").ToString();                 
         
            }catch (Exception ex)
            {
                LogWriter.WriteLog(" get_tanent()", ex.Message);
            }
            return tanent_id;
        }

        public async Task<string> get_authtoken()
        {
            var SPO = get_sharepointOption();

            string domain = SPO.domain;
            string client_id = SPO.client_id;
            string client_secret = SPO.client_secret;


            var auth_token = "";
            try
            { 
          
            var tanent_id = await get_tanent();
            //const string SITE_URL = "https://accounts.accesscontrol.windows.net/44060820-ecc9-4366-b97e-0db725e356db/tokens/OAuth/2";
            string SITE_URL = "https://accounts.accesscontrol.windows.net/" + tanent_id + "/tokens/OAuth/2";
            var httpclient = new HttpClient();
            var client = new WebClient();
            // httpclient.DefaultRequestHeaders.Add("Authorization", "application/json;odata=nometadata");
            // httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer");
            var content = new FormUrlEncodedContent(new[]
          {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", client_id + "@" + tanent_id),
                new KeyValuePair<string, string>("client_secret", client_secret),
                new KeyValuePair<string, string>("resource", "00000003-0000-0ff1-ce00-000000000000/"+domain +"@"+tanent_id),
            });
            var responseMessage = await httpclient.PostAsync(SITE_URL, content);
            var response = await responseMessage.Content.ReadAsStringAsync();
            JObject strjson = JObject.Parse(response);
            auth_token = strjson.Last.First.ToString();
            
            }
            catch (Exception ex)
            {
                LogWriter.WriteLog(" get_authtoken()", ex.Message);
            }
            return auth_token;


        }
        public async Task Upload_file_sharepoint_Async(SharePointUploadData SPUD )
        {
            var SPO = get_sharepointOption();

            string filename = SPUD.filename;
            string foldername = SPUD.foldername;
            string filePath = SPUD.filePath;

            string domain = SPO.domain;
            string client_id = SPO.client_id;
            string client_secret = SPO.client_secret;
            string site = SPO.site;
            string upload_folder = SPO.upload_folder;

            try
            {         
                string fileNameNotFullPAth = "";
                fileNameNotFullPAth = filename;
                var SITE_URL = "https://" + domain + "/" + site + "/_api/web/GetFolderByServerRelativeUrl('/" + site + "/" + upload_folder + "/" + foldername + "')/Files/add(url='" + fileNameNotFullPAth + "',overwrite=true)";
                string AUTH_TOKEN = "Bearer " + await get_authtoken();
                var httpclient = new HttpClient();
                httpclient.DefaultRequestHeaders.Add("Accept", "application/json;odata=nometadata");
                httpclient.DefaultRequestHeaders.Add("Authorization", AUTH_TOKEN);
                //  var responseMessage = await httpclient.PostAsync(SITE_URL, null);
                using (var contect = new MultipartFormDataContent("boundry ---"))
                {
                    

                    using (WebClient webClient = new WebClient())
                    {
                        Uri myUri = new Uri(filePath, UriKind.Absolute);
                        Uri SITE_URL_myUri = new Uri(SITE_URL, UriKind.Absolute);                  
                        contect.Add(new StreamContent(new MemoryStream(System.IO.File.ReadAllBytes(filePath))), "file", "tst.txt");
                        using (var msg = await httpclient.PostAsync(SITE_URL_myUri, contect))
                        {
                          //  LogWriter.WriteLog(" StatusCode", msg.StatusCode.ToString());
                           // Console.WriteLine(msg.StatusCode);
                           // Console.WriteLine(msg.IsSuccessStatusCode);
                            var input = await msg.Content.ReadAsStringAsync();
                            try
                            {
                                XDocument doc = XDocument.Parse(input);
                               // Console.WriteLine(doc.ToString());
                            }
                            catch (Exception ex) {
                               // Console.WriteLine("XML Parse error");
                              //  LogWriter.WriteLog(" Upload_file_sharepoint_Async()", ex.Message);
                            }
                        }
                    }
                }
            }
            catch(Exception ex) 
                {
                    LogWriter.WriteLog(" Upload_file_sharepoint_Async()", ex.Message);
                }
            }


        public async Task DownloadFile(SharePointUploadData SPUD)
        {

            string path = "c:\\";
                  var SPO = get_sharepointOption();

            string filename = SPUD.filename;
            string foldername = SPUD.foldername;
            string filePath = SPUD.filePath;

            string domain = SPO.domain;
            string client_id = SPO.client_id;
            string client_secret = SPO.client_secret;
            string site = SPO.site;
            string upload_folder = SPO.upload_folder;


          string  webUrl = "https://"+ domain +"/" + site;

            string webRelativeUrl =  "/"+ site +"/" + upload_folder+"/"+filename;
            string AUTH_TOKEN = "Bearer " + await get_authtoken();
                using (WebClient webClient = new WebClient())
                {
                    webClient.Headers.Add("Accept", "application/json;odata=nometadata");
                    webClient.Headers.Add("Authorization", AUTH_TOKEN);

                    Uri endpointUri = new Uri(webUrl + "/_api/web/GetFileByServerRelativeUrl('" + webRelativeUrl   + "')/$value");

                  
                        byte[] data = webClient.DownloadData(endpointUri);
                string downloadFolder = Path.Combine(Environment.ExpandEnvironmentVariables("%USERPROFILE%"), "Downloads\\");
                filename = downloadFolder + filename;
                ByteArrayToFile(filename, data);

            }

        }

        public bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }

    }

}
