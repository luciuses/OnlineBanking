using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    public class FormsEmailSettingsProvider : IEmailSettingsProvider
    {
        public FormsEmailSettingsProvider()
        {
            MailFromAddress = "onlinebanking@example.com";
            UseSsl = true;
            Username = "MySmtpUsername";
            Password = "MySmtpPassword";
            ServerName = "smtp.example.com";
            ServerPort = 587;
            WriteAsFile = true;
            FileLocation = HostingEnvironment.ApplicationPhysicalPath + "emails";
        }

        public string MailFromAddress { get; set; }
        public bool UseSsl { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string ServerName { get; set; }
        public int ServerPort { get; set; }
        public bool WriteAsFile { get; set; }
        public string FileLocation { get; set; }
    }
}