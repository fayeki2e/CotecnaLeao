using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;
using Postal;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Threading.Tasks;

namespace TechParvaLEAO.Services
{
    public class EmailSenderOptions
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string FromAddress { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ButtonURL { get; set; }
    }

    public interface IEmailSenderEnhance : IEmailSender
    {
        Task SendEmailAsync(MailMessage mailMessage);
        Task SendEmailAsync(Postal.Email emailData);
    }

    public class EmailSenderImpl : IEmailSenderEnhance
    {
        // Our private configuration variables
        private readonly EmailSenderOptions emailOptions;
        private readonly IEmailService emailService;
        private readonly IHostingEnvironment env;
        private SmtpClient client;
        private readonly IAuditLogServices _auditlog;

        // Get our parameterized configuration
        public EmailSenderImpl(IEmailService emailService,
            IEmailViewRender emailViewRenderer,
            IHostingEnvironment env,
            IOptions<EmailSenderOptions> emailOptions,
            IAuditLogServices auditlog)
        {
            this.emailOptions = emailOptions.Value;
            this.emailService = emailService;
            this.env = env;
            ((EmailViewRender)emailViewRenderer).EmailViewDirectoryName = @"Emails";
            client = CreateSmtpClient();
            _auditlog = auditlog;
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

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            await client.SendMailAsync(new MailMessage(emailOptions.FromAddress, email, subject, htmlMessage) { IsBodyHtml = true });
        }

        public async Task SendEmailAsync(Postal.Email emailData)
        {
            //try
            //{
            //    var al = new Auditlog_DM();
            //    al.module = "EmailSender.cs";
            //    al.url = "";
            //    al.comment =  emailData.ViewName;
            //    al.userid = "";
            //    al.line = "Before Sending";
            //    al.path = "";
            //    al.exception = "";
            //    al.reportingto = "";
            //    al.details = "";
            //    al.status = "";
            //    _auditlog.InsertLog(al);
            //}
            //catch (Exception ex)
            //{

            //}

            try
            {
                MailMessage message = await emailService.CreateMailMessageAsync(emailData);
                message.From = new MailAddress(emailOptions.FromAddress);    
                await SendEmailAsync(message);
            }
            catch (Exception ex)
            {

            }
            //try
            //{
            //    var al = new Auditlog_DM();
            //    al.module = "HanfireEmailsender.cs";
            //    al.url = "";
            //    al.comment = emailData.ViewName;
            //    al.userid = "";
            //    al.line = "After Sending";
            //    al.path = "";
            //    al.exception = "";
            //    al.reportingto = "";
            //    al.details = "";
            //    al.status = "";
            //    _auditlog.InsertLog(al);
            //}
            //catch (Exception ex)
            //{

            //}
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
            }catch(SmtpException e)
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

    }
}