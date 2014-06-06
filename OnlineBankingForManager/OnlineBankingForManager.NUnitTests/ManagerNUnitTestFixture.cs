using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using OnlineBankingForManager.Domain.Abstract;
using OnlineBankingForManager.Domain.Entities;
using OnlineBankingForManager.WebUI.Controllers;

namespace OnlineBankingForManager.NUnitTests
{
    [TestFixture]
    class ManagerNUnitTestFixture
    {
        [Test]
        public void Can_Listing()
        {
            // Arrange
            Mock<IClientRepository> mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Returns(new Client[]
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
            ManagerController controller = new ManagerController(mockA.Object);
            // Action
            ActionResult result = controller.List();
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            List<Client> clients = (List<Client>)((ViewResult)result).Model;
            Assert.IsTrue(clients.Count()==5);
            Assert.AreEqual(clients.ElementAt(0).FirstName,"fname1");
            Assert.AreEqual(clients.ElementAt(1).FirstName, "fname2");
            Assert.AreEqual(clients.ElementAt(2).FirstName, "fname3");
            Assert.AreEqual(clients.ElementAt(3).FirstName, "fname4");
            Assert.AreEqual(clients.ElementAt(4).FirstName, "fname5");
        }
        [Test]
        public void Cannot_Listing_With_Service_Error()
        {
            // Arrange
            Mock<IClientRepository> mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<Exception>();
            // create a controller
            ManagerController controller = new ManagerController(mockA.Object);
            // Action
            ActionResult result = controller.List();
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            List<Client> clients = (List<Client>)((ViewResult)result).Model;
            Assert.IsFalse(clients.Any());
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
        }
        [Test]
        public void Can_Edit()
        {
            // Arrange
            Mock<IClientRepository> mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Returns(new Client[]
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
            ManagerController controller = new ManagerController(mockA.Object);
            // Action
            Client c1=(controller.Edit(1)).ViewData.Model as Client;
            Client c2 = controller.Edit(2).ViewData.Model as Client;
            Client c3 = controller.Edit(3).ViewData.Model as Client;
            // Assert
            Assert.AreEqual(c1.ClientId, 1);
            Assert.AreEqual(c2.ClientId, 2);
            Assert.AreEqual(c3.ClientId, 3);
        }
        [Test]
        public void Cannot_Edit_With_Service_Error()
        {
            // Arrange
            Mock<IClientRepository> mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Throws<Exception>();
            // create a controller
            ManagerController controller = new ManagerController(mockA.Object);
            // Action
            ViewResult result = controller.Edit(1);
           
            // Assert
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual((((ViewResult)result).Model as Client).ClientId, 0);
            Assert.AreEqual(((ViewResult)result).ViewData["StatusList"],Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>());
        }
       
        [Test]
        public void Cannot_Edit_Nonexistent_Client()
        {
            // Arrange
            Mock<IClientRepository> mockA = new Mock<IClientRepository>();
            mockA.Setup(m => m.Clients).Returns(new Client[]
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
            ManagerController controller = new ManagerController(mockA.Object);
            // Action
            Client c = controller.Edit(4).ViewData.Model as Client;
            // Assert
            Assert.IsNull(c);
        }
        [Test]
        public void Can_Save_Valid_Changes()
        {
            // Arrange - create mock repository
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            // Arrange - create the controller
            ManagerController controller = new ManagerController(mock.Object);
            // Arrange - create a client
            Client client = new Client { FirstName = "Test"};
            // Act - try to save the product
            ActionResult result = controller.Edit(client);
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveClient(client));
            // Assert - check the method result type
            Assert.IsInstanceOf(typeof(RedirectToRouteResult),result);
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Keys.ElementAt(0),"action");
            Assert.AreEqual(((RedirectToRouteResult)result).RouteValues.Values.ElementAt(0), "List");
        }
        [Test]
        public void Cannot_Save_Client_With_Service_Error()
        {
            // Arrange
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            mock.Setup(m => m.SaveClient(It.IsAny<Client>())).Throws<Exception>();
            // create a controller
            ManagerController controller = new ManagerController(mock.Object);
            Client client = new Client { FirstName = "Test" };
            // Action

            ActionResult result = controller.Edit(client);

            // Assert
            // Assert - check that the repository was called
            mock.Verify(m => m.SaveClient(client));
            Assert.IsInstanceOf(typeof(ViewResult), result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual((((ViewResult)result).Model as Client), client);
            Assert.AreEqual(((ViewResult)result).ViewData["StatusList"], Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>());
        }
        [Test]
        public void Cannot_Save_Invalid_Changes()
        {
            // Arrange - create mock repository
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            // Arrange - create the controller
            ManagerController controller = new ManagerController(mock.Object);
            // Arrange - create a product
            Client client = new Client { FirstName = "Test" };
            // Arrange - add an error to the model state
            controller.ModelState.AddModelError("error", "error");
            // Act - try to save the product
            ActionResult result = controller.Edit(client);
            // Assert - check that the repository was not called
            mock.Verify(m => m.SaveClient(It.IsAny<Client>()), Times.Never());
            // Assert - check the method result type
            Assert.IsInstanceOf(typeof(ViewResult),result);
            Assert.IsFalse(((ViewResult)result).ViewData.ModelState.IsValid);
            Assert.IsNotNull(((ViewResult)result).Model as Client);
            Assert.AreEqual((((ViewResult)result).Model as Client), client);
            Assert.AreEqual(((ViewResult)result).ViewData["StatusList"], Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>());
        }

        [Test]
        public void Can_Delete_Valid_Clients()
        {
            // Arrange - create a client
            Client client = new Client { ClientId = 2, FirstName = "Test" };
            // Arrange - create the mock repository
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            mock.Setup(m => m.DeleteClient(client.ClientId)).Returns(client);
            // Arrange - create the controller
            ManagerController controller = new ManagerController(mock.Object);
            // Act - delete the client
            ActionResult result=controller.Delete(client.ClientId);
            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(client.ClientId));
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsNotNull(controller.TempData["ModelState"] as ModelStateDictionary);
            Assert.IsTrue((controller.TempData["ModelState"] as ModelStateDictionary).IsValid);
            Assert.AreEqual(controller.TempData["message"] as string,string.Format("{0} was deleted", client.FirstName));
        }
        [Test]
        public void Cannot_Delete_Invalid_Clients()
        {
            // Arrange - create the mock repository
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            // Arrange - create the controller
            ManagerController controller = new ManagerController(mock.Object);
            // Act - delete the client
            ActionResult result = controller.Delete(1);
            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(1));
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsTrue(controller.ModelState.IsValid);
            Assert.IsNotNull(controller.TempData["ModelState"] as ModelStateDictionary);
            Assert.IsTrue((controller.TempData["ModelState"] as ModelStateDictionary).IsValid);
            Assert.IsNull(controller.TempData["message"]);
        }
        [Test]
        public void Cannot_Delete_With_Service_Error()
        {
            // Arrange - create the mock repository
            Mock<IClientRepository> mock = new Mock<IClientRepository>();
            mock.Setup(m => m.DeleteClient(It.IsAny<int>())).Throws<Exception>();
            // Arrange - create the controller
            ManagerController controller = new ManagerController(mock.Object);
            // Act - delete the client
            ActionResult result=controller.Delete(3);
            // Assert - ensure that the repository delete method was
            // called with the correct Client
            mock.Verify(m => m.DeleteClient(3));
            Assert.IsInstanceOf(typeof(RedirectToRouteResult), result);
            Assert.IsFalse(controller.ModelState.IsValid);
            Assert.IsNotNull(controller.TempData["ModelState"] as ModelStateDictionary);
            Assert.IsFalse((controller.TempData["ModelState"] as ModelStateDictionary).IsValid);
        }
    }
}
