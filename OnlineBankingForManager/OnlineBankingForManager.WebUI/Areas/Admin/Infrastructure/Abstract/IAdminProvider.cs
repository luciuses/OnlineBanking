using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OnlineBankingForManager.WebUI.Areas.Admin.Infrastructure.Abstract
{
    using OnlineBankingForManager.WebUI.Infrastructure;

    public interface IAdminProvider
    {
        IEnumerable<UserProfile> GetUsers();
    }
}
