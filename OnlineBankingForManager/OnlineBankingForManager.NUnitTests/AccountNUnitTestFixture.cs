using System;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using OnlineBankingForManager.WebUI.Controllers;
using OnlineBankingForManager.WebUI.Infrastructure;
using OnlineBankingForManager.WebUI.Infrastructure.Abstract;
using System.Web.Security;
using OnlineBankingForManager.WebUI.Models;

namespace OnlineBankingForManager.NUnitTests
{
    [TestFixture]
    public class AccountNUnitTestFixture
    {
        [Test]
        public void Get_User_Info()
        {
            // Arrange
            string currentUserName = "current-user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser).Returns(currentUserName);
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.UserInfo();
            // Assert
            Assert.IsInstanceOf(typeof(PartialViewResult), result);
            Assert.IsTrue(((PartialViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, currentUserName);
        }

        [Test]
        public void Cannot_Get_User_Info_With_SqlException()
        {
            // Arrange
            string currentUserName = "current-user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser)
                .Throws(FormatterServices.GetUninitializedObject(typeof (SqlException)) as SqlException);
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.UserInfo();
            // Assert
            Assert.IsInstanceOf(typeof (PartialViewResult), result);
            Assert.IsFalse(((PartialViewResult) result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, null);
        }

        [Test]
        public void Cannot_Get_User_Info_With_Exception()
        {
            // Arrange
            string currentUserName = "current-user";
            UserInfo ui = new UserInfo {CurrentUser = currentUserName};
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.CurrentUser).Throws<Exception>();
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.UserInfo();
            // Assert
            Assert.IsInstanceOf(typeof (PartialViewResult), result);
            Assert.IsFalse(((PartialViewResult) result).ViewData.ModelState.IsValid);
            Assert.AreEqual((((PartialViewResult)result).Model as UserInfo).CurrentUser, null);
        }

        // Test get login view
        [Test]
        public void Login_GetViewTest()
        {
            // Arrange
            // create a controller
            AccountController controller = new AccountController(null,null,null);
            string returnUrl = "returnUrl";
            // Action
            ActionResult result = controller.Login(returnUrl);
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult),result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, null);
        }

        [Test]
        public void Can_Login_With_Valid_Credentials()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", "password", It.IsAny<bool>())).Returns(true);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "password",
                RememberMe = true
            };
            // create a controller 
            string returnUrl = "/MyURL";
            AccountController controller = new AccountController(mockA.Object, null,null);
            
            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(RedirectResult),result);
            Assert.AreEqual(returnUrl, ((RedirectResult)result).Url);
        }
        [Test]
        public void Cannot_Login_With_Locked_User()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", "password", It.IsAny<bool>())).Returns(true);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(false);
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "password",
                RememberMe = true
            };
            // create a controller 
            string returnUrl = "/MyURL";
            AccountController controller = new AccountController(mockA.Object, null, null);

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
        [Test]
        public void Locking_User_With_Failure_Large_5()
        {
            // Arrange
            string email = "test@test.test";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "passwordBad",
                RememberMe = true
            };
            // create a controller 
            string returnUrl = "/MyURL";
            AccountController controller = new AccountController(mockA.Object,null, mockB.Object);

            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            mockA.Verify(m => m.IsUserExist(loginViewModel.UserName));
            mockA.Verify(m => m.GetPasswordFailuresSinceLastSuccess(loginViewModel.UserName));
            mockA.Verify(m => m.IsActiveUser(loginViewModel.UserName));
            mockB.Verify(m=>m.Send(email,It.IsAny<string>()));
            mockA.Verify(m => m.LockUser(loginViewModel.UserName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }
        [Test]
        public void Cannot_Send_Mail_When_Locking_User_With_Mail_Service_Exception()
        {
            // Arrange
            string email = "test@test.test";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "passwordBad",
                RememberMe = true
            };
            // create a controller 
            string returnUrl = "/MyURL";
            AccountController controller = new AccountController(mockA.Object, null, mockB.Object);

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
        [Test]
        public void Cannot_Locking_User_With_Exception()
        {
            // Arrange
            string email = "test@test.test";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("user", It.IsAny<string>(), It.IsAny<bool>())).Returns(false);
            mockA.Setup(m => m.IsUserExist("user")).Returns(true);
            mockA.Setup(m => m.GetPasswordFailuresSinceLastSuccess("user")).Returns(6);
            mockA.Setup(m => m.IsActiveUser("user")).Returns(true);
            mockA.Setup(m => m.GetEmailUser("user")).Returns(email);
            mockA.Setup(m => m.GetUserId("user")).Returns(1);
            mockA.Setup(m => m.LockUser(It.IsAny<string>())).Throws<Exception>();
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "passwordBad",
                RememberMe = true
            };
            // create a controller 
            string returnUrl = "/MyURL";
            AccountController controller = new AccountController(mockA.Object, null, mockB.Object);

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
        [Test]
        public void Cannot_Login_With_Invalid_Credentials()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate("userBad", "passwordBad", true)).Returns(false);
           // create a controller
            AccountController controller = new AccountController(mockA.Object, null,null);
            string returnUrl = "returnUrl";
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "userBad",
                Password = "passwordBad",
                RememberMe = true
            };
            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }
        [Test]
        public void Cannot_Login_With_ModelError()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null,null);
            string returnUrl = "returnUrl";
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "userBad",
                Password = "passwordBad",
                RememberMe = true
            };
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }
        [Test]
        public void Cannot_Login_With_Service_Error()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>()
                )).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "password",
                RememberMe = true
            };
            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }
        [Test]
        public void Cannot_Login_With_Exception()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<Exception>();
            // create a controller
            AccountController controller = new AccountController(mockA.Object, null, null);
            string returnUrl = "returnUrl";
            LoginViewModel loginViewModel = new LoginViewModel
            {
                UserName = "user",
                Password = "password",
                RememberMe = true
            };
            // Action
            ActionResult result = controller.Login(loginViewModel, returnUrl);
            // Assert
            mockA.Verify(m => m.Authenticate(loginViewModel.UserName, loginViewModel.Password, loginViewModel.RememberMe));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.ReturnUrl, returnUrl);
            Assert.AreEqual(((ViewResult)result).Model, loginViewModel);
        }
        [Test]
        public void Can_Logout()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null,null);

            // Action
            ActionResult result = controller.LogOff();
            // Assert
            mockA.Verify(m=>m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
            Assert.IsTrue(controller.ModelState.IsValid);
            
        }
        [Test]
        public void Cannot_Logout_With_Service_Error()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Logout()).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.LogOff();
            // Assert
            // Assert
            mockA.Verify(m => m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
        [Test]
        public void Cannot_Logout_With_Exception()
        {
            // Arrange
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.Logout()).Throws<Exception>();
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);

            // Action
            ActionResult result = controller.LogOff();
            // Assert
            // Assert
            mockA.Verify(m => m.Logout());
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
        [Test]
        public void Register_GetViewTest()
        {
            // Arrange
            AccountController controller = new AccountController(null, null,null);
            // Action
            ActionResult result = controller.Register();
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, null);
        }

        [Test]
        public void Can_Register_With_Valid_UserData()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail","userAddress")).Returns(MembershipCreateStatus.Success);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate("user", "password", false)).Returns(true);
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object,mockA.Object,mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName,registerViewModel.Password,registerViewModel.UserEmail,registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail,It.IsAny<string>()));
            mockC.Verify(m=>m.Authenticate(registerViewModel.UserName,registerViewModel.Password,false));
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }

        [Test]
        public void Cannot_Register_With_Invalid_UserData()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmailBad", "userAddress")).Returns(MembershipCreateStatus.InvalidEmail);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmailBad",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()),Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }
        [Test]
        public void Cannot_Register_With_ModelError()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError(String.Empty, "error");
            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()),Times.Never());
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);
        }
        [Test]
        public void Cannot_Register_With_Service_Registration_Error()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<string>()
                )).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);

        }
        [Test]
        public void Cannot_Register_With_Registration_Exception()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            mockC.Verify(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()), Times.Never());
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).Model, registerViewModel);

        }
        [Test]
        public void Cannot_Confirm_Registered_Send_Email_With_Service_Email_Error()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate("user", "password", false)).Returns(true);

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
        
        [Test]
        public void Cannot_Authenticate_After_Registered_With_Authenticate_Service_Error()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send("userEmail", MembershipCreateStatus.Success.ErrorCodeToString()));
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate(
                It.IsAny<string>(), 
                It.IsAny<string>(), 
                It.IsAny<bool>()
                )).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
        [Test]
        public void Cannot_Authenticate_After_Registered_With_Authenticate_Exception()
        {
            // Arrange
            Mock<IRegisterProvider> mockA = new Mock<IRegisterProvider>();
            mockA.Setup(m => m.Register("user", "password", "userEmail", "userAddress")).Returns(MembershipCreateStatus.Success);
            Mock<ISendMailProvider> mockB = new Mock<ISendMailProvider>();
            mockB.Setup(m => m.Send("userEmail", MembershipCreateStatus.Success.ErrorCodeToString()));
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>()
                )).Throws<Exception>();

            RegisterViewModel registerViewModel = new RegisterViewModel
            {
                UserName = "user",
                Password = "password",
                ConfirmPassword = "password",
                UserEmail = "userEmail",
                UserAddress = "userAddress"
            };
            // create a controller 
            AccountController controller = new AccountController(mockC.Object, mockA.Object, mockB.Object);

            // Action
            ActionResult result = controller.Register(registerViewModel);
            // Assert
            mockA.Verify(m => m.Register(registerViewModel.UserName, registerViewModel.Password, registerViewModel.UserEmail, registerViewModel.UserAddress));
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, It.IsAny<string>()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
        [Test]
        public void Can_Unlock()
        {
            // Arrange
            int userId = 1;
            string userName="user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsTrue(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username,userName);
        }
        [Test]
        public void Cannot_Unlock_With_OperationException()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(It.IsAny<string>())).Throws<InvalidOperationException>();
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }
        [Test]
        public void Cannot_Unlock_With_GetUser_SqlException()
        {
            // Arrange
            int userId = 1;
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(It.IsAny<int>())).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(It.IsAny<string>()), Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, "no user");
        }
        [Test]
        public void Cannot_Unlock_With_Unlock_SqlException()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(It.IsAny<string>())).Throws(FormatterServices.GetUninitializedObject(typeof(SqlException)) as SqlException);
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }
        [Test]
        public void Cannot_Unlock_With_Unlock_Exception()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Returns(userName);
            mockA.Setup(m => m.UnlockUser(userName)).Throws<Exception>();
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(userName));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, userName);
        }
        [Test]
        public void Cannot_Unlock_With_GetUser_Exception()
        {
            // Arrange
            int userId = 1;
            string userName = "user";
            Mock<IAuthProvider> mockA = new Mock<IAuthProvider>();
            mockA.Setup(m => m.GetUserNameById(userId)).Throws<Exception>();
            // create a controller 
            AccountController controller = new AccountController(mockA.Object, null, null);
            // Action
            ActionResult result = controller.Unlock(userId);
            // Assert
            mockA.Verify(m => m.GetUserNameById(userId));
            mockA.Verify(m => m.UnlockUser(It.IsAny<string>()),Times.Never);
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.AreEqual(((ViewResult)result).ViewBag.Username, "no user");
        }
    }
}

