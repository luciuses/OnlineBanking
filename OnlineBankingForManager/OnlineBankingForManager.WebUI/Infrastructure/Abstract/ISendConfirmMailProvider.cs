using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace OnlineBankingForManager.WebUI.Infrastructure.Abstract
{
    public interface ISendConfirmMailProvider
    {
        bool Send(string mailToAddress, string bodyText);
    }
}
