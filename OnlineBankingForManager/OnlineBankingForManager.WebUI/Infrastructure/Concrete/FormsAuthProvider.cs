using System;
using System.Linq;
using System.Web.Providers.Entities;
using System.Web.Security;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
using WebMatrix.WebData;

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authenticate(string username, string password, bool rememberMe)
        {
            return WebSecurity.Login(username, password, persistCookie: rememberMe);
        }

        public string GetEmailUser(string username)
        {
            UsersContext usersContext=new UsersContext();
            var profile=usersContext.UserProfiles.FirstOrDefault(d=>d.UserName.Equals(username));
            return profile.UserEmail;
        }

        public int GetPasswordFailuresSinceLastSuccess(string username)
        {
            return WebSecurity.GetPasswordFailuresSinceLastSuccess(username);
        }

        public bool IsUserExist(string username)
        {
            return WebSecurity.UserExists(username);
        }

        public void LockUser(string username)
        {
            Roles.Provider.RemoveUsersFromRoles(new[] { username }, new[] { "ActiveUser" });
        }
       
        public void UnlockUser(string username)
        {
            Roles.Provider.AddUsersToRoles(new[] { username }, new[] { "ActiveUser" });
        }
        
        public bool IsActiveUser(string username)
        {
            return Roles.Provider.IsUserInRole(username, "ActiveUser");
        }
        public void Logout()
        {
            WebSecurity.Logout();
        }

        public int GetUserId(string username)
        {
            return WebSecurity.GetUserId(username);
        }
        public string GetUserNameById(int userId)
        {
                UsersContext usersContext = new UsersContext();
                var profile = usersContext.UserProfiles.FirstOrDefault(d => d.UserId.Equals(userId));
                return profile.UserName;
        }
    }
}