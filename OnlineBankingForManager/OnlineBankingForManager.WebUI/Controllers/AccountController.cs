using System;
//using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics.Contracts;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.Mvc;
using Microsoft.SqlServer.Server;
using OnlineBankingForManager.WebUI.HtmlHelpers;
using OnlineBankingForManager.WebUI.Infrastructure;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;

using WebMatrix.WebData;
using OnlineBankingForManager.WebUI.Models;

namespace OnlineBankingForManager.WebUI.Controllers
{
    [Authorize(Roles = "ActiveUser")]
    public class AccountController : Controller
    {
        private IAuthProvider authProvider;
        private IRegisterProvider registerProvider;
        private ISendMailProvider sendMailProvider;
        public AccountController(IAuthProvider auth, IRegisterProvider register, ISendMailProvider sendConfirmMail)
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
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (authProvider.Authenticate(model.UserName, model.Password, model.RememberMe))
                    {
                        if (authProvider.IsActiveUser(model.UserName))
                        return Redirect(returnUrl ?? Url.Action("List", "Manager"));
                        ModelState.AddModelError(String.Empty, "Login locked. Email for unlock account sended.");
                    }
                    else
                    {
                        ModelState.AddModelError(String.Empty, "Login or Password is incorrect.");
                        if (authProvider.IsUserExist(model.UserName) && authProvider.GetPasswordFailuresSinceLastSuccess(model.UserName) > 5)
                        {
                            if (authProvider.IsActiveUser(model.UserName))
                            {
                                try
                                {
                                    string text =
                                        string.Format(
                                            "{0}, your account locked, for unlock it go to the link: {1}/{2}/{3}?userId={4}",
                                            model.UserName,
                                            (Request != null) ? Request.Url.GetLeftPart(UriPartial.Authority) : "",
                                            "Account",
                                            "Unlock",
                                            authProvider.GetUserId(model.UserName)
                                            );

                                    sendMailProvider.Send(authProvider.GetEmailUser(model.UserName), text);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError(String.Empty, string.Format("Mail service error when was locking login: {0}.",model.UserName));
                                    Logger.Log.Error(String.Format("Mail service error {0} - {1}, when was locking login: {2}.",ex.GetType(),ex.Message, model.UserName));
                                }
                                try
                                {
                                    authProvider.LockUser(model.UserName);
                                    Logger.Log.Info(String.Format("User {0} locked)", model.UserName));
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError(String.Empty, string.Format("{0}, when locking login:{1}.", ex.Message,model.UserName));
                                    Logger.Log.Error(String.Format("Authentication service error:{0} \r\n LockUser({1})", ex, model.UserName), ex);
                                }
                            }
                        }
                    }
                    
                }
                catch (SqlException ex)
                {
                    ModelState.AddModelError(String.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                    Logger.Log.Error(String.Format("Authentication service error:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, model.RememberMe), ex);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                    Logger.Log.Error(String.Format("Authentication service error:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, model.RememberMe), ex);
                }
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        //// GET: /Account/LogOff
        [ExportModelStateToTempData]
        public ActionResult LogOff()
        {
            try{
            authProvider.Logout();
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(String.Format("Authenticate service error:{0} \r\n Logout()", ex.ToString()), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                Logger.Log.Error(String.Format("Authenticate service error:{0} \r\n Logout()", ex.ToString()), ex);
            }
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
        [ExportModelStateToTempData]
        public ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                try{
                        createStatus = registerProvider.Register(model.UserName, model.Password, model.UserEmail, model.UserAddress);
                    }
                catch (SqlException ex)
                {
                    ModelState.AddModelError(String.Empty, string.Format("Registration service error:{0}, try again later.",ex.Message));
                    Logger.Log.Error(String.Format("Registration service error:{0} \r\n Register() with params: Name {1}, Pass {2}, Email {3}, Address{4}", ex.ToString(), model.UserName, model.Password, model.UserEmail, model.UserAddress), ex);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                    Logger.Log.Error(String.Format("Registration service error:{0} \r\n Register() with params: Name {1}, Pass {2}, Email {3}, Address{4}", ex.ToString(), model.UserName, model.Password, model.UserEmail, model.UserAddress), ex);
                    return View(model);
                }
                if (createStatus == MembershipCreateStatus.Success)
                {
                    try{
                        sendMailProvider.Send(model.UserEmail, string.Format("{0}, you registered at Online Banking successfully.",model.UserName));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(String.Empty, string.Format("Mail sender service error:{0} - {1},confirm email not sended.", ex.GetType().ToString(), ex.Message));
                        Logger.Log.Error(String.Format("Mail sender service error when registering:{0} \r\n Send() with params: Email {1}, Text {2}", ex.ToString(), model.UserEmail, createStatus.ErrorCodeToString()), ex);
                    }
                    try
                    {
                        authProvider.UnlockUser(model.UserName);
                        authProvider.Authenticate(model.UserName, model.Password,false);
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError(String.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                        Logger.Log.Error(String.Format("Authentication service error when registering:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, false), ex);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                        Logger.Log.Error(String.Format("Authentication service error when registering:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", ex.ToString(), model.UserName, model.Password, false), ex);
                    }
                    return RedirectToAction("List", "Manager");
                }
                ModelState.AddModelError("", createStatus.ErrorCodeToString());
                
            }

            return View(model);
        }
        //
        // GET: /Account/Unlock
        [AllowAnonymous]
        public ActionResult Unlock(int userId)
        {
            string username = "no user";
            try
            {
                username = authProvider.GetUserNameById(userId);
                authProvider.UnlockUser(username);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Operation error:{0}.", ex.Message));
                Logger.Log.Error(String.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Authentication service error:{0}, try again later.",ex.Message));
                Logger.Log.Error(String.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                Logger.Log.Error(String.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }
            ViewBag.Username = username;
            return View();
        }
        public PartialViewResult UserInfo()
        {
            
            var model = new UserInfo();
            try
            {
                model.CurrentUser = authProvider.CurrentUser;
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(ex.Message + " when geting current username");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}.", ex.GetType().ToString(), ex.Message));
                Logger.Log.Error(ex.Message + " when geting current username");
            }
            return this.PartialView(model);
        }
    }

}
