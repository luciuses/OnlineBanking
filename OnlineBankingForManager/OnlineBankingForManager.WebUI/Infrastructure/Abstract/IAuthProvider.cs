using System.Dynamic;

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Authenticate(string username, string password, bool rememberMe);
        void LockUser(string username);
        void UnlockUser(string username);
        bool IsActiveUser(string username);
        bool IsUserExist(string username);
        int GetPasswordFailuresSinceLastSuccess(string username);
        string GetEmailUser(string username);
        int GetUserId(string username);
        string GetUserNameById(int userId);
        void Logout();
        string CurrentUser { get ; }
    }
}