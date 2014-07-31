// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerNUnitTestFixture.cs" company="">
//   
// </copyright>
// <summary>
//   The manager n unit test fixture.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

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
    using OnlineBankingForManager.WebUI.Controllers;
    using OnlineBankingForManager.WebUI.Models;

    /// <summary>
    /// The manager n unit test fixture.
    /// </summary>
    [TestFixture]
    internal class ManagerNUnitTestFixture
    {
        /// <summary>
        /// The can_ create.
        /// </summary>
        [Test]
        public void Can_Create()
        {
            // Arrange
            // create a controller
            var controller = new ManagerController(null);
            string returnUrl = "/MyUrl";

            // Action
            ViewResult result = controller.Create(returnUrl);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsNotNull(result.Model as Client);
            Assert.AreEqual((result.Model as Client).ClientId, 0);
            Assert.AreEqual((result.Model as Client).DateBirth, DateTime.Today);
        }

        /// <summary>
        /// The can_ delete_ valid_ clients.
        /// </summary>
        [Test]
        public void Can_Delete_Valid_Clients()
        {
            // Arrange - create a client
            var client = new Client { ClientId = 2, FirstName = "firstName", LastName = "lastName" };

            // Arrange - create the mock repository
            var mock = new Mock<IClientRepository>();
            mock.Setup(m => m.DeleteClient(client.ClientId)).Returns(client);

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);
            string returnUrl = "/MyUrl";

            // Act - delete the client
            ActionResult result = controller.Delete(client.ClientId, returnUrl);

            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(client.ClientId));
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.AreEqual(controller.TempData["message"] as string, string.Format("{0} {1} - was deleted", client.FirstName, client.LastName));
        }

        /// <summary>
        /// The can_ edit.
        /// </summary>
        [Test]
        public void Can_Edit()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients)
                .Returns(
                    new[]
                        {
                            new Client
                                {
                                    ClientId = 1, 
                                    ContractNumber = 11, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname1", 
                                    LastName = "lname1", 
                                    PhoneNumber = "111", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 2, 
                                    ContractNumber = 22, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname2", 
                                    LastName = "lname2", 
                                    PhoneNumber = "222", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 3, 
                                    ContractNumber = 33, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname3", 
                                    LastName = "lname3", 
                                    PhoneNumber = "333", 
                                    Status = StatusClient.Classic
                                }
                        }.AsQueryable());

            // create a controller
            string returnUrl = "/MyUrl";
            var controller = new ManagerController(mockA.Object);

            // Action
            var c1 = controller.Edit(1, returnUrl).ViewData.Model as Client;
            var c2 = controller.Edit(2, returnUrl).ViewData.Model as Client;
            var c3 = controller.Edit(3, returnUrl).ViewData.Model as Client;

            // Assert
            Assert.AreEqual(c1.ClientId, 1);
            Assert.AreEqual(c2.ClientId, 2);
            Assert.AreEqual(c3.ClientId, 3);
        }

        /// <summary>
        /// The can_ listing.
        /// </summary>
        [Test]
        public void Can_Listing()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients)
                .Returns(
                    new[]
                        {
                            new Client
                                {
                                    ClientId = 1, 
                                    ContractNumber = 11, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname1", 
                                    LastName = "lname1", 
                                    PhoneNumber = "111", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 2, 
                                    ContractNumber = 22, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname2", 
                                    LastName = "lname2", 
                                    PhoneNumber = "222", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 3, 
                                    ContractNumber = 33, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname3", 
                                    LastName = "lname3", 
                                    PhoneNumber = "333", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 4, 
                                    ContractNumber = 44, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname4", 
                                    LastName = "lname4", 
                                    PhoneNumber = "444", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 5, 
                                    ContractNumber = 55, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname5", 
                                    LastName = "lname5", 
                                    PhoneNumber = "555", 
                                    Status = StatusClient.Classic
                                }
                        }.AsQueryable());

            // create a controller
            var controller = new ManagerController(mockA.Object);

            // Action
            ActionResult result = controller.List(1, 10, StatusClient.Classic, "ClientId");

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            var clients = (List<Client>)((ClientListViewModel)((ViewResult)result).Model).Clients;
            Assert.IsTrue(clients.Count() == 5);
            Assert.AreEqual(clients.ElementAt(0).FirstName, "fname1");
            Assert.AreEqual(clients.ElementAt(1).FirstName, "fname2");
            Assert.AreEqual(clients.ElementAt(2).FirstName, "fname3");
            Assert.AreEqual(clients.ElementAt(3).FirstName, "fname4");
            Assert.AreEqual(clients.ElementAt(4).FirstName, "fname5");
        }

        /// <summary>
        /// The can_ save_ valid_ changes.
        /// </summary>
        [Test]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            var mock = new Mock<IClientRepository>();

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);

            // Arrange - create a client
            var client = new Client { FirstName = "Test" };

            // Act - try to save the product
            string returnUrl = "/MyUrl";
            ActionResult result = controller.Edit(client, returnUrl);

            // Assert - check that the repository was called
            mock.Verify(m => m.SaveClient(client));

            // Assert - check the method result type
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual(((RedirectResult)result).Url, returnUrl);
        }

        /// <summary>
        /// The cannot_ delete_ invalid_ clients.
        /// </summary>
        [Test]
        public void Cannot_Delete_Invalid_Clients()
        {
            // Arrange - create the mock repository
            var mock = new Mock<IClientRepository>();

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);
            string returnUrl = "/MyUrl";

            // Act - delete the client
            ActionResult result = controller.Delete(1, returnUrl);

            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(1));
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual(((RedirectResult)result).Url, returnUrl);
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsNull(controller.TempData["message"]);
        }

        /// <summary>
        /// The cannot_ delete_ with_ entity exception.
        /// </summary>
        [Test]
        public void Cannot_Delete_With_EntityException()
        {
            // Arrange - create the mock repository
            var mock = new Mock<IClientRepository>();
            mock.Setup(m => m.DeleteClient(It.IsAny<int>())).Throws<EntityException>();

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);
            string returnUrl = "/MyUrl";

            // Act - delete the client
            ActionResult result = controller.Delete(3, returnUrl);

            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(3));
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual(((RedirectResult)result).Url, returnUrl);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        /// <summary>
        /// The cannot_ delete_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Delete_With_Exception()
        {
            // Arrange - create the mock repository
            var mock = new Mock<IClientRepository>();
            mock.Setup(m => m.DeleteClient(It.IsAny<int>())).Throws<Exception>();

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);
            string returnUrl = "/MyUrl";

            // Act - delete the client
            ActionResult result = controller.Delete(3, returnUrl);

            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(3));
            Assert.IsInstanceOf(typeof(RedirectResult), result);
            Assert.AreEqual(((RedirectResult)result).Url, returnUrl);
            Assert.IsFalse(controller.ModelState.IsValid);
        }

        /// <summary>
        /// The cannot_ edit_ nonexistent_ client.
        /// </summary>
        [Test]
        public void Cannot_Edit_Nonexistent_Client()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients)
                .Returns(
                    new[]
                        {
                            new Client
                                {
                                    ClientId = 1, 
                                    ContractNumber = 11, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname1", 
                                    LastName = "lname1", 
                                    PhoneNumber = "111", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 2, 
                                    ContractNumber = 22, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname2", 
                                    LastName = "lname2", 
                                    PhoneNumber = "222", 
                                    Status = StatusClient.Classic
                                }, 
                            new Client
                                {
                                    ClientId = 3, 
                                    ContractNumber = 33, 
                                    DateBirth = DateTime.Today, 
                                    Deposit = true, 
                                    FirstName = "fname3", 
                                    LastName = "lname3", 
                                    PhoneNumber = "333", 
                                    Status = StatusClient.Classic
                                }
                        }.AsQueryable());

            // create a controller
            string returnUrl = "/MyUrl";
            var controller = new ManagerController(mockA.Object);

            // Action
            var c = controller.Edit(4, returnUrl).ViewData.Model as Client;

            // Assert
            Assert.IsNull(c);
        }

        /// <summary>
        /// The cannot_ edit_ with_ entity exception.
        /// </summary>
        [Test]
        public void Cannot_Edit_With_EntityException()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<EntityException>();
            string returnUrl = "/MyUrl";

            // create a controller
            var controller = new ManagerController(mockA.Object);

            // Action
            ViewResult result = controller.Edit(1, returnUrl);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.Model as Client);
            Assert.AreEqual((result.Model as Client).ClientId, 0);
        }

        /// <summary>
        /// The cannot_ edit_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Edit_With_Exception()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<Exception>();
            string returnUrl = "/MyUrl";

            // create a controller
            var controller = new ManagerController(mockA.Object);

            // Action
            ViewResult result = controller.Edit(1, returnUrl);

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(result.ViewData.ModelState.IsValid);
            Assert.IsNotNull(result.Model as Client);
            Assert.AreEqual((result.Model as Client).ClientId, 0);
        }

        /// <summary>
        /// The cannot_ listing_ with_ entity exception.
        /// </summary>
        [Test]
        public void Cannot_Listing_With_EntityException()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<EntityException>();

            // create a controller
            var controller = new ManagerController(mockA.Object);

            // Action
            ActionResult result = controller.List(1, 10, StatusClient.Classic, "ClientId");

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            var clients = (List<Client>)((ClientListViewModel)((ViewResult)result).Model).Clients;
            Assert.IsFalse(clients.Any());
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        /// <summary>
        /// The cannot_ listing_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Listing_With_Exception()
        {
            // Arrange
            var mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<Exception>();

            // create a controller
            var controller = new ManagerController(mockA.Object);

            // Action
            ActionResult result = controller.List(1, 10, StatusClient.Classic, "ClientId");

            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            var clients = (List<Client>)((ClientListViewModel)((ViewResult)result).Model).Clients;
            Assert.IsFalse(clients.Any());
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }

        /// <summary>
        /// The cannot_ save_ client_ with_ entity exception.
        /// </summary>
        [Test]
        public void Cannot_Save_Client_With_EntityException()
        {
            // Arrange
            var mock = new Mock<IClientRepository>();
            mock.Setup(m => m.SaveClient(It.IsAny<Client>())).Throws<EntityException>();

            // create a controller
            var controller = new ManagerController(mock.Object);
            var client = new Client { FirstName = "Test" };
            string returnUrl = "/MyUrl";

            // Action
            ActionResult result = controller.Edit(client, returnUrl);

            // Assert
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveClient(client));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual(((ViewResult)result).Model as Client, client);
        }

        /// <summary>
        /// The cannot_ save_ client_ with_ exception.
        /// </summary>
        [Test]
        public void Cannot_Save_Client_With_Exception()
        {
            // Arrange
            var mock = new Mock<IClientRepository>();
            mock.Setup(m => m.SaveClient(It.IsAny<Client>())).Throws<Exception>();

            // create a controller
            var controller = new ManagerController(mock.Object);
            var client = new Client { FirstName = "Test" };
            string returnUrl = "/MyUrl";

            // Action
            ActionResult result = controller.Edit(client, returnUrl);

            // Assert
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveClient(client));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual(((ViewResult)result).Model as Client, client);
        }

        /// <summary>
        /// The cannot_ save_ invalid_ changes.
        /// </summary>
        [Test]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            var mock = new Mock<IClientRepository>();

            // Arrange - create the controller
            var controller = new ManagerController(mock.Object);

            // Arrange - create a product
            var client = new Client { FirstName = "Test" };

            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            string returnUrl = "/MyUrl";

            // Act - try to save the product
            ActionResult result = controller.Edit(client, returnUrl);

            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveClient(It.IsAny<Client>()), Times.Never());

            // Assert - check the method result type
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual(((ViewResult)result).Model as Client, client);
        }
    }
}