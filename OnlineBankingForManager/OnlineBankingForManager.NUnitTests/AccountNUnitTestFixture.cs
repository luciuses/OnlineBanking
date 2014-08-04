// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountNUnitTestFixture.cs" company="">
//   
// </copyright>
// <summary>
//   The account n unit test fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.NUnitTests
{
    using System;
    using System.Data.SqlClient;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Web.Mvc;
    using System.Web.Security;
    using Moq;
    using NUnit.Framework;
    using OnlineBankingForManager.WebUI.Controllers;
    using OnlineBankingForManager.WebUI.Infrastructure;
    using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
    using OnlineBankingForManager.WebUI.Models;

    /// <summary>
    /// The account n unit test fixture.
    /// </summary>
    [TestFixture]
    public class AccountNUnitTestFixture
    {
        /// <summary>
        /// The can_ login_ with_ valid_ credentials.
        /// </summary>
        [Test]
        public void Can_Login_With_Valid_Credentials()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", "password", It.IsAny<bool>())).Returns(true);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "password", RememberMe = true };

            // create a controller 
            string returnUrl = "/MyURL";
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual(returnUrl, ((RedirectResult)result).Url);
        }

        /// <summary>
        /// The can_ logout.
        /// </summary>
        [Test]
        public void Can_Logout()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.LogOff();

            // Assert
            mockA.Verify(m => m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
            Assert.IsTrue(controller.ModelState.IsValid);
        }

        /// <summary>
        /// The can_ register_ with_ valid_ user data.
        /// </summary>
        [Test]
        public void Can_Register_With_Valid_UserData()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            var mockB = new Mock<ISendMailProvider>();
            var mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate("user", "password", false)).Returns(true);
            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The can_ unlock.
        /// </summary>
        [Test]
        public void Can_Unlock()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }

        /// <summary>
        /// The cannot_ authenticate_ after_ registered_ with_ authenticate_ exception.
        /// </summary>
        [Test]
        public void Cannot_Authenticate_After_Registered_With_Authenticate_Exception()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            var mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send("userEmail", MembershipCreateStatus.Success.ErrorCodeToString()));
            var mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<Exception>();

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The cannot_ authenticate_ after_ registered_ with_ authenticate_ service_ error.
        /// </summary>
        [Test]
        public void Cannot_Authenticate_After_Registered_With_Authenticate_Service_Error()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            var mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send("userEmail", MembershipCreateStatus.Success.ErrorCodeToString()));
            var mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The cannot_ confirm_ registered_ send_ email_ with_ service_ email_ error.
        /// </summary>
        [Test]
        public void Cannot_Confirm_Registered_Send_Email_With_Service_Email_Error()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            var mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            var mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate("user", "password", false)).Returns(true);

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The cannot_ get_ user_ info_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Get_User_Info_With_Exception()
        {
            // Arrange
            string currentUserName = "current-user";
            var ui = new UserInfo { CurrentUser = currentUserName };
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser).Throws<Exception>();

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.UserInfo();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsFalse(((PartialViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, null);
        }

        /// <summary>
        /// The cannot_ get_ user_ info_ with_ sql exception.
        /// </summary>
        [Test]
        public void Cannot_Get_User_Info_With_SqlException()
        {
            // Arrange
            string currentUserName = "current-user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.UserInfo();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsFalse(((PartialViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, null);
        }

        /// <summary>
        /// The cannot_ locking_ user_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Locking_User_With_Exception()
        {
            // Arrange
            string email = "test@test.test";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            mockA.Setup(m => m.LockUser(It.IsAny<string>())).Throws<Exception>();
            var mockB = new Mock<ISendMailProvider>();
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "passwordBad", RememberMe = true };

            // create a controller 
            string returnUrl = "/MyURL";
            var controller = new AccountController(mockA.Object, null, mockB.Object);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsUserExist(loginViewModel.UserName));
            mockA.Verify(m => m.GetPasswordFailuresSinceLastSuccess(loginViewModel.UserName));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            mockB.Verify(m => m.Send(email, It.IsAny<string>()));
            mockA.Verify(m => m.LockUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ login_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Login_With_Exception()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<Exception>();

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "password", RememberMe = true };

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ login_ with_ invalid_ credentials.
        /// </summary>
        [Test]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("userBad", "passwordBad", true)).Returns(false);

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            var loginViewModel = new LoginViewModel { UserName = "userBad", Password = "passwordBad", RememberMe = true };

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ login_ with_ locked_ user.
        /// </summary>
        [Test]
        public void Cannot_Login_With_Locked_User()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", "password", It.IsAny<bool>())).Returns(true);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(false);
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "password", RememberMe = true };

            // create a controller 
            string returnUrl = "/MyURL";
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ login_ with_ model error.
        /// </summary>
        [Test]
        public void Cannot_Login_With_ModelError()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            var loginViewModel = new LoginViewModel { UserName = "userBad", Password = "passwordBad", RememberMe = true };

            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ login_ with_ service_ error.
        /// </summary>
        [Test]
        public void Cannot_Login_With_Service_Error()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "password", RememberMe = true };

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ logout_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Logout_With_Exception()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Logout()).Throws<Exception>();

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.LogOff();

            // Assert
            // Assert
            mockA.Verify(m => m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The cannot_ logout_ with_ service_ error.
        /// </summary>
        [Test]
        public void Cannot_Logout_With_Service_Error()
        {
            // Arrange
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Logout()).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.LogOff();

            // Assert
            // Assert
            mockA.Verify(m => m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["action"], "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues["controller"], "Manager");
        }

        /// <summary>
        /// The cannot_ register_ with_ invalid_ user data.
        /// </summary>
        [Test]
        public void Cannot_Register_With_Invalid_UserData()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmailBad", "userAddress")).Returns(MembershipCreateStatus.InvalidEmail);
            var mockB = new Mock<ISendMailProvider>();
            var mockC = new Mock<IAuthProvider>();

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmailBad", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }

        /// <summary>
        /// The cannot_ register_ with_ model error.
        /// </summary>
        [Test]
        public void Cannot_Register_With_ModelError()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            var mockB = new Mock<ISendMailProvider>();
            var mockC = new Mock<IAuthProvider>();
            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Arrange - add an error to the model state
            controller.ModelState.AddModelError(string.Empty, "error");

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }

        /// <summary>
        /// The cannot_ register_ with_ registration_ exception.
        /// </summary>
        [Test]
        public void Cannot_Register_With_Registration_Exception()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            var mockB = new Mock<ISendMailProvider>();
            var mockC = new Mock<IAuthProvider>();

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }

        /// <summary>
        /// The cannot_ register_ with_ service_ registration_ error.
        /// </summary>
        [Test]
        public void Cannot_Register_With_Service_Registration_Error()
        {
            // Arrange
            var mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            var mockB = new Mock<ISendMailProvider>();
            var mockC = new Mock<IAuthProvider>();

            var registerViewModel = new RegisterViewModel
                                        {
                                            UserName = "user", 
                                            Password = "password", 
                                            ConfirmPassword = "password", 
                                            UserEmail = "userEmail", 
                                            UserAddress = "userAddress"
                                        };

            // create a controller 
            var controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);

            // Assert
            mockA.Verify(
                m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }

        /// <summary>
        /// The cannot_ send_ mail_ when_ locking_ user_ with_ mail_ service_ exception.
        /// </summary>
        [Test]
        public void Cannot_Send_Mail_When_Locking_User_With_Mail_Service_Exception()
        {
            // Arrange
            string email = "test@test.test";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            var mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "passwordBad", RememberMe = true };

            // create a controller 
            string returnUrl = "/MyURL";
            var controller = new AccountController(mockA.Object, null, mockB.Object);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsUserExist(loginViewModel.UserName));
            mockA.Verify(m => m.GetPasswordFailuresSinceLastSuccess(loginViewModel.UserName));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            mockB.Verify(m => m.Send(email, It.IsAny<string>()));
            mockA.Verify(m => m.LockUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The cannot_ unlock_ with_ get user_ exception.
        /// </summary>
        [Test]
        public void Cannot_Unlock_With_GetUser_Exception()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Throws<Exception>();

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, "no user");
        }

        /// <summary>
        /// The cannot_ unlock_ with_ get user_ sql exception.
        /// </summary>
        [Test]
        public void Cannot_Unlock_With_GetUser_SqlException()
        {
            // Arrange
            int userId = 1;
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(It.IsAny<int>()))
                .Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, "no user");
        }

        /// <summary>
        /// The cannot_ unlock_ with_ operation exception.
        /// </summary>
        [Test]
        public void Cannot_Unlock_With_OperationException()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(It.IsAny<string>())).Throws<InvalidOperationException>();

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }

        /// <summary>
        /// The cannot_ unlock_ with_ unlock_ exception.
        /// </summary>
        [Test]
        public void Cannot_Unlock_With_Unlock_Exception()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(userName)).Throws<Exception>();

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }

        /// <summary>
        /// The cannot_ unlock_ with_ unlock_ sql exception.
        /// </summary>
        [Test]
        public void Cannot_Unlock_With_Unlock_SqlException()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(It.IsAny<string>())).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            // create a controller 
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.Unlock(userId);

            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }

        /// <summary>
        /// The get_ user_ info.
        /// </summary>
        [Test]
        public void Get_User_Info()
        {
            // Arrange
            string currentUserName = "current-user";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser).Returns(currentUserName);

            // create a controller
            var controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.UserInfo();

            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsTrue(((PartialViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, currentUserName);
        }

        /// <summary>
        /// The locking_ user_ with_ failure_ large_5.
        /// </summary>
        [Test]
        public void Locking_User_With_Failure_Large_5()
        {
            // Arrange
            string email = "test@test.test";
            var mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            var mockB = new Mock<ISendMailProvider>();
            var loginViewModel = new LoginViewModel { UserName = "user", Password = "passwordBad", RememberMe = true };

            // create a controller 
            string returnUrl = "/MyURL";
            var controller = new AccountController(mockA.Object, null, mockB.Object);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);

            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsUserExist(loginViewModel.UserName));
            mockA.Verify(m => m.GetPasswordFailuresSinceLastSuccess(loginViewModel.UserName));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            mockB.Verify(m => m.Send(email, It.IsAny<string>()));
            mockA.Verify(m => m.LockUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }

        /// <summary>
        /// The login_ get view test.
        /// </summary>
        [Test]
        public void Login_GetViewTest()
        {
            // Arrange
            // create a controller
            var controller = new AccountController(null, null, null);
            string returnUrl = "returnUrl";

            // Action
            ActionResult result = controller.Login(returnUrl);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, null);
        }

        /// <summary>
        /// The register_ get view test.
        /// </summary>
        [Test]
        public void Register_GetViewTest()
        {
            // Arrange
            var controller = new AccountController(null, null, null);

            // Action
            ActionResult result = controller.Register();

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, null);
        }
    }
}