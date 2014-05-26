namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    public interface IAuthProvider
    {
        bool Authenticate(string username, string password, bool rememberMe);
        void Logout();
    }
}