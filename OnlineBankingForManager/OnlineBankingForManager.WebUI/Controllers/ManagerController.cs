using System;
using System.Collections;
using System.Collections.Generic;
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
            ModelState.Merge(TempData["ModelState"] as ModelStateDictionary);
            try
            {
                return View(repository.Clients.ToList());
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(String.Empty, "Repository service error, try again later.");
                Logger.Log.Error(String.Format("Repository service error when get Clients:{0} ", ex.ToString()),ex);
            }
            return View(new List<Client>{});
        }

        public ViewResult Edit(int clientId)
        {
            Client client;
            try
            {
               client = repository.Clients.FirstOrDefault(p => p.ClientId == clientId);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Repository service error, try again later.");
                Logger.Log.Error(String.Format("Repository service error when get Client ({0}) :{1} ", clientId, ex.ToString()), ex);
                client = new Client();
            }
            ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
            return View(client);
        }

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveClient(client);
                    TempData["message"] = string.Format("{0} has been saved", client.FirstName);
                    return RedirectToAction("List");
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty, "Repository service error, client not saved, try again later.");
                    Logger.Log.Error(
                        String.Format("Repository service error when saving Client ({0}) :{1} ", client.FirstName, ex.ToString()),
                        ex);
                    ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
                    return View(client);
                }
            }
            else
            {
                ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
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
            try
            {
                Client deletedClient = repository.DeleteClient(clientId);

                if (deletedClient != null)
                {
                    TempData["message"] = string.Format("{0} was deleted", deletedClient.FirstName);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, "Repository service error, client not deleted, try again later.");
                Logger.Log.Error(
                    String.Format("Repository service error when deleting Client ({0}) :{1} ", clientId, ex.ToString()), ex);
            }
            TempData["ModelState"] = ModelState;
            return RedirectToAction("List");
        }
    }
}
