// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IEmailSettingsProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The EmailSettingsProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    /// <summary>
    /// The EmailSettingsProvider interface.
    /// </summary>
    public interface IEmailSettingsProvider
    {
        /// <summary>
        /// Gets or sets the mail from address.
        /// </summary>
        string MailFromAddress { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether use ssl.
        /// </summary>
        bool UseSsl { get; set; }

        /// <summary>
        /// Gets or sets the username.
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// Gets or sets the password.
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// Gets or sets the server name.
        /// </summary>
        string ServerName { get; set; }

        /// <summary>
        /// Gets or sets the server port.
        /// </summary>
        int ServerPort { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether write as file.
        /// </summary>
        bool WriteAsFile { get; set; }

        /// <summary>
        /// Gets or sets the file location.
        /// </summary>
        string FileLocation { get; set; }
    }
}