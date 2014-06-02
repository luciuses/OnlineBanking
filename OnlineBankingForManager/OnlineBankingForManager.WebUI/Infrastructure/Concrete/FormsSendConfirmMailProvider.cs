using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    
    public class FormsSendConfirmMailProvider : ISendConfirmMailProvider
    {
        private IEmailSettingsProvider emailSettings;

        public FormsSendConfirmMailProvider(IEmailSettingsProvider settings)
        {
            emailSettings = settings;
        }

        public bool Send(string mailToAddress, string bodyText)
        {
            using (var smtpClient = new SmtpClient())
            {
                smtpClient.EnableSsl = emailSettings.UseSsl;
                smtpClient.Host = emailSettings.ServerName;
                smtpClient.Port = emailSettings.ServerPort;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password);
                if (emailSettings.WriteAsFile)
                {
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtpClient.PickupDirectoryLocation = emailSettings.FileLocation;
                    smtpClient.EnableSsl = false;
                }
                
                    MailMessage mailMessage = new MailMessage(
                        emailSettings.MailFromAddress, // From
                        mailToAddress, // To
                        "Online banking registration!", // Subject
                        bodyText); // Body

                    if (emailSettings.WriteAsFile)
                    {
                        mailMessage.BodyEncoding = Encoding.ASCII;
                    }
                    smtpClient.Send(mailMessage);
                
            }
            return true;
        }
    }
}