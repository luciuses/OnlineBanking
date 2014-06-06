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
                try
                {
                    if (authProvider.Authenticate(model.UserName, model.Password, model.RememberMe))
                        return Redirect(returnUrl ?? Url.Action("List", "Manager"));
                    ModelState.AddModelError(String.Empty, "Login or Password is incorrect.");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Authentication service error, try again later.");
                    Logger.Log.Error(String.Format("Authentication service error:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, model.RememberMe), ex);
                }
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
            try{
            authProvider.Logout();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Authentication service error, try again later.");
                Logger.Log.Error(String.Format("Authenticate service error:{0} \r\n Logout()", ex.ToString()), ex);
            }
            TempData["ModelState"] = ModelState;
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
                MembershipCreateStatus createStatus;
                try{
                        createStatus = registerProvider.Register(model.UserName, model.Password, model.UserEmail, model.UserAddress);
                    }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Registration service error, try again later.");
                    Logger.Log.Error(String.Format("Registration service error:{0} \r\n Register() with params: Name {1}, Pass {2}, Email {3}, Address{4}", ex.ToString(), model.UserName, model.Password, model.UserEmail, model.UserAddress), ex);
                    return View(model);
                }
                if (createStatus == MembershipCreateStatus.Success)
                {
                    try{
                        bool res=sendMailProvider.Send(model.UserEmail, "Registration message - "+createStatus.ErrorCodeToString());
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(String.Empty, "Mail sender service error, email not sended.");
                        Logger.Log.Error(String.Format("Mail sender service error when registering:{0} \r\n Send() with params: Email {1}, Text {2}", ex.ToString(), model.UserEmail, createStatus.ErrorCodeToString()), ex);
                    }
                    try
                    {
                        authProvider.Authenticate(model.UserName, model.Password,false);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(String.Empty, "Authentication service error, try again later.");
                        Logger.Log.Error(String.Format("Authentication service error when registering:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, "false"), ex);
                    }
                    TempData["ModelState"] = ModelState; 
                    return RedirectToAction("List", "Manager");
                }
                ModelState.AddModelError("", createStatus.ErrorCodeToString());
                
            }

            return View(model);
        }
    }

}
