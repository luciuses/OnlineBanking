// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IRegisterProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The RegisterProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    using System.Web.Security;

    /// <summary>
    /// The RegisterProvider interface.
    /// </summary>
    public interface IRegisterProvider
    {
        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="userEmail">
        /// The user email.
        /// </param>
        /// <param name="userAddress">
        /// The user address.
        /// </param>
        /// <returns>
        /// The <see cref="MembershipCreateStatus"/>.
        /// </returns>
        MembershipCreateStatus Register(string username, string password, string userEmail, string userAddress);
    }
}