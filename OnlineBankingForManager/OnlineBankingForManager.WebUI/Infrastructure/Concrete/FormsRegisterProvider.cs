using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
using WebMatrix.WebData;

namespace OnlineBankingForManager.WebUI.Infrastructure.Concrete
{
    public class FormsRegisterProvider : IRegisterProvider
    {
        
     public MembershipCreateStatus Register(string username, string password, string userEmail, string userAddress)
        {
            MembershipCreateStatus createStatus = MembershipCreateStatus.Success;
            try
            {
                WebSecurity.CreateUserAndAccount(username, password, new
                {
                    UserEmail = userEmail,
                    UserAddress = userAddress
                });
                
            }
            catch (MembershipCreateUserException e)
            {
                createStatus = e.StatusCode;
            }
            return createStatus;
        }
    }
}