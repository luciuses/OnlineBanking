using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    public interface IRegisterProvider
    {
        MembershipCreateStatus Register(string username, string password, string userEmail, string userAddress);
    }
}
