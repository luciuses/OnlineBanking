// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="">
//   
// </copyright>
// <summary>
//   The account controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// using System.Data.Entity;

namespace OnlineBankingForManager.WebUI.Controllers
{
    using System;
    using System.Data.SqlClient;
    using System.Web.Mvc;
    using System.Web.Security;
    using OnlineBankingForManager.WebUI.HtmlHelpers;
    using OnlineBankingForManager.WebUI.Infrastructure;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
    using OnlineBankingForManager.WebUI.Models;

    /// <summary>
    /// The account controller.
    /// </summary>
    [Authorize(Roles = "ActiveUser")]
    public partial class AccountController : Controller
    {
        /// <summary>
        /// The auth provider.
        /// </summary>
        private readonly IAuthProvider _authProvider;

        /// <summary>
        /// The register provider.
        /// </summary>
        private readonly IRegisterProvider _registerProvider;

        /// <summary>
        /// The send mail provider.
        /// </summary>
        private readonly ISendMailProvider _sendMailProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="auth">
        /// The auth.
        /// </param>
        /// <param name="register">
        /// The register.
        /// </param>
        /// <param name="sendConfirmMail">
        /// The send confirm mail.
        /// </param>
        public AccountController(IAuthProvider auth, IRegisterProvider register, ISendMailProvider sendConfirmMail)
        {
            _authProvider = auth;
            _registerProvider = register;
            _sendMailProvider = sendConfirmMail;
        }

        // GET: /Account/Login

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public virtual ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: /Account/Login

        /// <summary>
        /// The login.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        public virtual ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (_authProvider.Authenticate(model.UserName, model.Password, model.RememberMe))
                    {
                        if (_authProvider.IsActiveUser(model.UserName))
                        {
                            return Redirect(returnUrl ?? Url.Action(MVC.Manager.List()));
                        }

                        ModelState.AddModelError(string.Empty, "Login locked. Email for unlock account sended.");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Login or Password is incorrect.");
                        if (_authProvider.IsUserExist(model.UserName) && _authProvider.GetPasswordFailuresSinceLastSuccess(model.UserName) > 5)
                        {
                            if (_authProvider.IsActiveUser(model.UserName))
                            {
                                try
                                {
                                    string text = string.Format(
                                        "{0}, your account locked, for unlock it go to the link: {1}/{2}/{3}?userId={4}", 
                                        model.UserName,
                                        (Request != null && Request.Url != null) ? Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty, 
                                        "Account", 
                                        "Unlock", 
                                        _authProvider.GetUserId(model.UserName));

                                    _sendMailProvider.Send(_authProvider.GetEmailUser(model.UserName), text);
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError(
                                        string.Empty, 
                                        string.Format("Mail service error when was locking login: {0}.", model.UserName));
                                    Logger.Log.Error(
                                        string.Format(
                                            "Mail service error {0} - {1}, when was locking login: {2}.", 
                                            ex.GetType(), 
                                            ex.Message, 
                                            model.UserName));
                                }

                                try
                                {
                                    _authProvider.LockUser(model.UserName);
                                    Logger.Log.Info(string.Format("User {0} locked)", model.UserName));
                                }
                                catch (Exception ex)
                                {
                                    ModelState.AddModelError(string.Empty, string.Format("{0}, when locking login:{1}.", ex.Message, model.UserName));
                                    Logger.Log.Error(string.Format("Authentication service error:{0} \r\n LockUser({1})", ex, model.UserName), ex);
                                }
                            }
                        }
                    }
                }
                catch (SqlException ex)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                    Logger.Log.Error(
                        string.Format(
                            "Authentication service error:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", 
                            ex, 
                            model.UserName, 
                            model.Password, 
                            model.RememberMe), 
                        ex);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                    Logger.Log.Error(
                        string.Format(
                            "Authentication service error:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", 
                            ex, 
                            model.UserName, 
                            model.Password, 
                            model.RememberMe), 
                        ex);
                }
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //// GET: /Account/LogOff

        /// <summary>
        /// The log off.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ExportModelStateToTempData]
        public virtual ActionResult LogOff()
        {
            try
            {
                _authProvider.Logout();
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(string.Format("Authenticate service error:{0} \r\n Logout()", ex), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                Logger.Log.Error(string.Format("Authenticate service error:{0} \r\n Logout()", ex), ex);
            }

            return RedirectToAction(MVC.Manager.List());
        }

        // GET: /Account/Register

        /// <summary>
        /// The register.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public virtual ActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register

        /// <summary>
        /// The register.
        /// </summary>
        /// <param name="model">
        /// The model.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        [AllowAnonymous]
        [ExportModelStateToTempData]
        public virtual ActionResult Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus createStatus;
                try
                {
                    createStatus = _registerProvider.Register(model.UserName, model.Password, model.UserEmail, model.UserAddress);
                }
                catch (SqlException ex)
                {
                    ModelState.AddModelError(string.Empty, string.Format("Registration service error:{0}, try again later.", ex.Message));
                    Logger.Log.Error(
                        string.Format(
                            "Registration service error:{0} \r\n Register() with params: Name {1}, Pass {2}, Email {3}, Address{4}", 
                            ex, 
                            model.UserName, 
                            model.Password, 
                            model.UserEmail, 
                            model.UserAddress), 
                        ex);
                    return View(model);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                    Logger.Log.Error(
                        string.Format(
                            "Registration service error:{0} \r\n Register() with params: Name {1}, Pass {2}, Email {3}, Address{4}", 
                            ex, 
                            model.UserName, 
                            model.Password, 
                            model.UserEmail, 
                            model.UserAddress), 
                        ex);
                    return View(model);
                }

                if (createStatus == MembershipCreateStatus.Success)
                {
                    try
                    {
                        _sendMailProvider.Send(model.UserEmail, string.Format("{0}, you registered at Online Banking successfully.", model.UserName));
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(
                            string.Empty, 
                            string.Format("Mail sender service error:{0} - {1},confirm email not sended.", ex.GetType(), ex.Message));
                        Logger.Log.Error(
                            string.Format(
                                "Mail sender service error when registering:{0} \r\n Send() with params: Email {1}, Text {2}", 
                                ex, 
                                model.UserEmail, 
                                createStatus.ErrorCodeToString()), 
                            ex);
                    }

                    try
                    {
                        _authProvider.UnlockUser(model.UserName);
                        _authProvider.Authenticate(model.UserName, model.Password, false);
                    }
                    catch (SqlException ex)
                    {
                        ModelState.AddModelError(string.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                        Logger.Log.Error(
                            string.Format(
                                "Authentication service error when registering:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", 
                                ex, 
                                model.UserName, 
                                model.Password, 
                                false), 
                            ex);
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                        Logger.Log.Error(
                            string.Format(
                                "Authentication service error when registering:{0} \r\n Authenticate() with params: Name {1}, Pass {2},Remember {3}", 
                                ex, 
                                model.UserName, 
                                model.Password, 
                                false), 
                            ex);
                    }

                    return RedirectToAction(MVC.Manager.List());
                }

                ModelState.AddModelError(string.Empty, createStatus.ErrorCodeToString());
            }

            return View(model);
        }

        // GET: /Account/Unlock

        /// <summary>
        /// The unlock.
        /// </summary>
        /// <param name="userId">
        /// The user id.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [AllowAnonymous]
        public virtual ActionResult Unlock(int userId)
        {
            string username = "no user";
            try
            {
                username = _authProvider.GetUserNameById(userId);
                _authProvider.UnlockUser(username);
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Operation error:{0}.", ex.Message));
                Logger.Log.Error(string.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(string.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                Logger.Log.Error(string.Format(ex.Message + " when unlocking user:{0},id={1}", username, userId));
            }

            ViewBag.Username = username;
            return View();
        }

        /// <summary>
        /// The user info.
        /// </summary>
        /// <returns>
        /// The <see cref="PartialViewResult"/>.
        /// </returns>
        public virtual PartialViewResult UserInfo()
        {
            var model = new UserInfo();
            try
            {
                model.CurrentUser = _authProvider.CurrentUser;
            }
            catch (SqlException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Authentication service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(ex.Message + " when geting current username");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}.", ex.GetType(), ex.Message));
                Logger.Log.Error(ex.Message + " when geting current username");
            }

            return PartialView(model);
        }
    }
}