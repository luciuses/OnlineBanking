// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsEmailSettingsProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The forms email settings provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    using System.Web.Hosting;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;

    /// <summary>
    /// The forms email settings provider.
    /// </summary>
    public class FormsEmailSettingsProvider : IEmailSettingsProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FormsEmailSettingsProvider"/> class.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the mail from address.
        /// </summary>
        public string MailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use ssl.
        /// </summary>
        public bool UseSsl { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        public int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether write as file.
        /// </summary>
        public bool WriteAsFile { get; set; }

        /// <summary>
        /// Gets or sets the file location.
        /// </summary>
        public string FileLocation { get; set; }
    }
}