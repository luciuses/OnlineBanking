// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsSendMailProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The forms send mail provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// using Ninject.Activation;

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;

    /// <summary>
    /// The forms send mail provider.
    /// </summary>
    public class FormsSendMailProvider : ISendMailProvider
    {
        /// <summary>
        /// The email settings.
        /// </summary>
        private readonly IEmailSettingsProvider emailSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="FormsSendMailProvider"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public FormsSendMailProvider(IEmailSettingsProvider settings)
        {
            emailSettings = settings;
        }

        /// <summary>
        /// The send.
        /// </summary>
        /// <param name="mailToAddress">
        /// The mail to address.
        /// </param>
        /// <param name="bodyText">
        /// The body text.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
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

                var mailMessage = new MailMessage(
                    emailSettings.MailFromAddress, 
                    // From
                    mailToAddress, 
                    // To
                    "Online banking message!", 
                    // Subject
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