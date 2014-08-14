using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineBankingForManager.WebUI.Areas.Admin.Models
{
    using OnlineBankingForManager.WebUI.Infrastructure;

    public class UsersViewModel
    {
        public IEnumerable<UserProfile> Users { get; set; }
    }
}