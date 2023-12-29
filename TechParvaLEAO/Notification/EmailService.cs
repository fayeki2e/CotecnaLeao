using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Net.Smtp;
using System.Configuration;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace Cotecna.Email_Process
{
    public sealed class EmailSender
    {
        //private static readonly Lazy<EmailService> lazy = new Lazy<EmailService>(() => new EmailService());
        //public static EmailService Instance { get { return lazy.Value; } }
        private SmtpClient _smtpClient;
        IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
            //ConfigureSmtpClient();
        }

        private void ConfigureSmtpClient()
        {
            _smtpClient = new SmtpClient();
            //SMTP Client
            _smtpClient.ServerCertificateValidationCallback = (s, c, h, e) => true;
            var smtpHost = _config.GetValue<string>("SmtpHost");
            var smtpPort = _config.GetValue<int>("SmtpPort");
            var smtpSecurity = _config.GetValue<bool>("SmtpUseSecurity");
            var smtpUsername = _config.GetValue<string>("SmtpUsername");
            var smtpPassword = _config.GetValue<string>("SmtpPassword");
            if (smtpSecurity)
            {
                _smtpClient.Connect(smtpHost, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                _smtpClient.Authenticate(smtpUsername, smtpPassword);
            }
            else
            {
                _smtpClient.Connect(smtpHost, smtpPort);
            }

        }

        public void SendEmail(MimeMessage message)
        {
            WaitFor3Seconds();
            //if (!_smtpClient.IsConnected) ConfigureSmtpClient();
            //_smtpClient.Send(message);
        }

        private void WaitFor3Seconds()
        {
            Thread.Sleep(3000);
        }
    }
}