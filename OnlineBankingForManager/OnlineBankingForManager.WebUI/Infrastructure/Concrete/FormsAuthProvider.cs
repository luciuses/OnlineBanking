// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FormsAuthProvider.cs" company="">
//   
// </copyright>
// <summary>
//   The forms auth provider.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    using System.Linq;
    using System.Web.Security;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
    using WebMatrix.WebData;

    /// <summary>
    /// The forms auth provider.
    /// </summary>
    public class FormsAuthProvider : IAuthProvider
    {
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
        public bool Authenticate(string username, string password, bool rememberMe)
        {
            return WebSecurity.Login(username, password, rememberMe);
        }

        /// <summary>
        /// The get email user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetEmailUser(string username)
        {
            var usersContext = new UsersContext();
            UserProfile profile = usersContext.UserProfiles.FirstOrDefault(d => d.UserName.Equals(username));
            return profile.UserEmail;
        }

        /// <summary>
        /// The get password failures since last success.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetPasswordFailuresSinceLastSuccess(string username)
        {
            return WebSecurity.GetPasswordFailuresSinceLastSuccess(username);
        }

        /// <summary>
        /// The is user exist.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsUserExist(string username)
        {
            return WebSecurity.UserExists(username);
        }

        /// <summary>
        /// The lock user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        public void LockUser(string username)
        {
            Roles.Provider.RemoveUsersFromRoles(new[] { username }, new[] { "ActiveUser" });
        }

        /// <summary>
        /// The unlock user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        public void UnlockUser(string username)
        {
            Roles.Provider.AddUsersToRoles(new[] { username }, new[] { "ActiveUser" });
        }

        /// <summary>
        /// The is active user.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsActiveUser(string username)
        {
            return Roles.Provider.IsUserInRole(username, "ActiveUser");
        }

        /// <summary>
        /// The logout.
        /// </summary>
        public void Logout()
        {
            WebSecurity.Logout();
        }

        /// <summary>
        /// The get user id.
        /// </summary>
        /// <param name="username">
        /// The username.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public int GetUserId(string username)
        {
            return WebSecurity.GetUserId(username);
        }

        /// <summary>
        /// The get user name by id.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public string GetUserNameById(int userId)
        {
            var usersContext = new UsersContext();
            UserProfile profile = usersContext.UserProfiles.FirstOrDefault(d => d.UserId.Equals(userId));
            return profile.UserName;
        }

        /// <summary>
        /// Gets the current user.
        /// </summary>
        public string CurrentUser
        {
            get
            {
                return WebSecurity.CurrentUserName;
            }
        }
    }
}