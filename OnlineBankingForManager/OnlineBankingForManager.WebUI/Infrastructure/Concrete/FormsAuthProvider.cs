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

        public void Logout()
        {
            WebSecurity.Logout();
        }
    }
}