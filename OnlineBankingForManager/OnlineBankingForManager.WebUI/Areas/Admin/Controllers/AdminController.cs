using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace OnlineBankingForManager.WebUI.Areas.Admin.Controllers
{
    using System.Web.Mvc;
    using OnlineBankingForManager.WebUI.Areas.Admin.Infrastructure.Abstract;
    using OnlineBankingForManager.WebUI.Areas.Admin.Models;

    public class AdminController : Controller
    {
        private IAdminProvider _adminProvider;
        public AdminController(IAdminProvider adminProvider)
        {
            _adminProvider = adminProvider;
        }

        public ViewResult Users()
        {
            return View(new UsersViewModel { Users = _adminProvider.GetUsers() });
        }
    }
}
