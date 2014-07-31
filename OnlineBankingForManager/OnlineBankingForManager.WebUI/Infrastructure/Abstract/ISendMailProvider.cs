// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISendMailProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The SendMailProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    /// <summary>
    /// The SendMailProvider interface.
    /// </summary>
    public interface ISendMailProvider
    {
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
        bool Send(string mailToAddress, string bodyText);
    }
}