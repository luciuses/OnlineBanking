
namespace OnlineBankingForManager.NUnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core;
    using System.Linq;
    using System.Web.Mvc;
    using Moq;
    using NUnit.Framework;
    using OnlineBankingForManager.Domain.Abstract;
    using OnlineBankingForManager.Domain.Entities;
    using OnlineBankingForManager.WebUI.Areas.Admin.Controllers;
    using OnlineBankingForManager.WebUI.Areas.Admin.Infrastructure.Abstract;
    using OnlineBankingForManager.WebUI.Areas.Admin.Models;
    using OnlineBankingForManager.WebUI.Controllers;
    using OnlineBankingForManager.WebUI.Infrastructure;
    using OnlineBankingForManager.WebUI.Models;

    [TestFixture]
    internal class AdminTests
    {
        [Test]
        public void CanGetUsers()
        {
            var mock = new Mock<IAdminProvider>();
            mock.Setup(u => u.GetUsers())
                .Returns(
                new[] 
                { 
                    new UserProfile { UserId = 1, UserName = "User1", UserAddress = "Address1", UserEmail = "Email1" }, 
                    new UserProfile { UserId = 2, UserName = "User2", UserAddress = "Address2", UserEmail = "Email2" },
                    new UserProfile { UserId = 3, UserName = "User3", UserAddress = "Address3", UserEmail = "Email3" }
                });
            var controller = new AdminController(mock.Object);

            var result = controller.Users();

            Assert.IsInstanceOf(typeof(ViewResult), result);
            var users = ((UsersViewModel)result.Model).Users.ToList();
            Assert.AreEqual(users.Count, 3);
            Assert.AreEqual(users[0].UserName, "User1");
            Assert.AreEqual(users[1].UserName, "User2");
            Assert.AreEqual(users[2].UserName, "User3");
        }
    }
}