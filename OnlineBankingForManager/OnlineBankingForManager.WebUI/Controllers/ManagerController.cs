using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineBankingForManager.Domain.Abstract;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.WebUI.Controllers
{
    [Authorize]
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/

        private IClientRepository repository;

        public ManagerController(IClientRepository repo)
        {
            repository = repo;
        }

        public ViewResult List()
        {
            return View(repository.Clients);
        }

        public ViewResult Edit(int clientId)
        {
            Client product = repository.Clients.FirstOrDefault(p => p.ClientId == clientId);
            ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
            return View(product);
        }

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                repository.SaveClient(client);
                TempData["message"] = string.Format("{0} has been saved", client.FirstName);
                return RedirectToAction("List");
            }
            else
            {
                // there is something wrong with the data values
                return View(client);
            }
        }

        public ViewResult Create()
        {
            ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
            return View("Edit",new Client{DataBirth = DateTime.Today});
        }

        
        public ActionResult Delete(int clientId)
        {
            Client deletedClient = repository.DeleteClient(clientId);
            if (deletedClient != null)
            {
                TempData["message"] = string.Format("{0} was deleted", deletedClient.FirstName);
            }
            return RedirectToAction("List");
        }
    }
}
