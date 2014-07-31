// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsRegisterProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The forms register provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    using System.Web.Security;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
    using WebMatrix.WebData;

    /// <summary>
    /// The forms register provider.
    /// </summary>
    public class FormsRegisterProvider : IRegisterProvider
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
        public MembershipCreateStatus Register(string username, string password, string userEmail, string userAddress)
        {
            var createStatus = MembershipCreateStatus.Success;
            try
            {
                WebSecurity.CreateUserAndAccount(username, password, new { UserEmail = userEmail, UserAddress = userAddress });
            }
            catch (MembershipCreateUserException e)
            {
                createStatus = e.StatusCode;
            }

            return createStatus;
        }
    }
}