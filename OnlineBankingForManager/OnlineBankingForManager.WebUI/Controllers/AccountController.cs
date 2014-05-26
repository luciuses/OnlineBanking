using System;
using System.Diagnostics.Contracts;
using System.Web.Security;
using System.Web.Mvc;
using OnlineBankingForManager.WebUI.Infrastructure;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
using WebMatrix.WebData;
using OnlineBankingForManager.WebUI.Models;

namespace OnlineBankingForManager.WebUI.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private IAuthProvider authProvider;
        private IRegisterProvider registerProvider;
        private ISendConfirmMailProvider sendMailProvider;
        public AccountController(IAuthProvider auth, IRegisterProvider register, ISendConfirmMailProvider sendConfirmMail)
        {
            authProvider = auth;
            registerProvider = register;
            sendMailProvider = sendConfirmMail;
        }
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (authProvider.Authenticate(model.UserName, model.Password, model.RememberMe)) return Redirect(returnUrl ?? Url.Action("List", "Manager"));
                ModelState.AddModelError(String.Empty, "Login or Password is incorrect.");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            authProvider.Logout();

            return RedirectToAction("List", "Manager");
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus = registerProvider.Register(model.UserName, model.Password, model.UserEmail, model.UserAddress);
                if (createStatus == MembershipCreateStatus.Success)
                {
                    sendMailProvider.Send(model.UserEmail, createStatus.ErrorCodeToString());
                    authProvider.Authenticate(model.UserName, model.Password,false);
                    return RedirectToAction("List", "Manager");
                }
                ModelState.AddModelError("", createStatus.ErrorCodeToString());
            }

            return View(model);
        }
    }

}
