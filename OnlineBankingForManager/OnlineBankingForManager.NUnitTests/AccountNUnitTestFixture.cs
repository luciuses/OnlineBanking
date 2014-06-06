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
            mockA.Setup(m => m.Authenticate("user", "password", true)).Returns(true);
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
            Assert.IsInstanceOf(typeof(RedirectResult),result);
            Assert.AreEqual(returnUrl, ((RedirectResult)result).Url);
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
            Assert.IsNotNull(controller.TempData["ModelState"] as ModelStateDictionary);
            Assert.IsTrue((controller.TempData["ModelState"] as ModelStateDictionary).IsValid);
        }
        [Test]
        public void Cannot_Logout_With_Service_Error()
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
            Assert.IsNotNull((controller.TempData["ModelState"]) as ModelStateDictionary);
            Assert.IsFalse(((controller.TempData["ModelState"]) as ModelStateDictionary).IsValid);
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
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
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
            mockB.Verify(m => m.Send(registerViewModel.UserEmail,"Registration message - "+ MembershipCreateStatus.Success.ErrorCodeToString()));
            mockC.Verify(m=>m.Authenticate(registerViewModel.UserName,registerViewModel.Password,false));
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsNotNull((controller.TempData["ModelState"]) as ModelStateDictionary);
            Assert.IsTrue(((controller.TempData["ModelState"]) as ModelStateDictionary).IsValid);
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
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
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
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
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
            mockA.Setup(m => m.Register(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
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
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
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
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, "Registration message - " + MembershipCreateStatus.Success.ErrorCodeToString()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull((controller.TempData["ModelState"]) as ModelStateDictionary);
            Assert.IsFalse(((controller.TempData["ModelState"]) as ModelStateDictionary).IsValid);
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
            Mock<ISendConfirmMailProvider> mockB = new Mock<ISendConfirmMailProvider>();
            mockB.Setup(m => m.Send("userEmail", MembershipCreateStatus.Success.ErrorCodeToString()));
            Mock<IAuthProvider> mockC = new Mock<IAuthProvider>();
            mockC.Setup(m => m.Authenticate(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>())).Throws<Exception>();

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
            mockB.Verify(m => m.Send(registerViewModel.UserEmail, "Registration message - " + MembershipCreateStatus.Success.ErrorCodeToString()));
            mockC.Verify(m => m.Authenticate(registerViewModel.UserName, registerViewModel.Password, false));
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull((controller.TempData["ModelState"]) as ModelStateDictionary);
            Assert.IsFalse(((controller.TempData["ModelState"]) as ModelStateDictionary).IsValid);
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0), "action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(1), "controller");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(1), "Manager");
        }
    }
}

