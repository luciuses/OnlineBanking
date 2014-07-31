// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IAuthProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The AuthProvider interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    /// <summary>
    /// The AuthProvider interface.
    /// </summary>
    public interface IAuthProvider
    {
        /// <summary>
        /// Gets the current user.
        /// </summary>
        string CurrentUser { get; }

        /// <summary>
        /// The authenticate.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <param name="password">
        /// The password.
        /// </param>
        /// <param name="rememberMe">
        /// The remember me.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool Authenticate(string username, string password, bool rememberMe);

        /// <summary>
        /// The lock user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        void LockUser(string username);

        /// <summary>
        /// The unlock user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        void UnlockUser(string username);

        /// <summary>
        /// The is active user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsActiveUser(string username);

        /// <summary>
        /// The is user exist.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsUserExist(string username);

        /// <summary>
        /// The get password failures since last success.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int GetPasswordFailuresSinceLastSuccess(string username);

        /// <summary>
        /// The get email user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetEmailUser(string username);

        /// <summary>
        /// The get user id.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        int GetUserId(string username);

        /// <summary>
        /// The get user name by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string GetUserNameById(int userId);

        /// <summary>
        /// The logout.
        /// </summary>
        void Logout();
    }
}