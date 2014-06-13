using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using OnlineBankingForManager.Domain.Abstract;
using OnlineBankingForManager.Domain.Entities;
using OnlineBankingForManager.WebUI.HtmlHelpers;
using OnlineBankingForManager.WebUI.Models;

namespace OnlineBankingForManager.WebUI.Controllers
{
    [Authorize(Roles = "ActiveUser")]
    public class ManagerController : Controller
    {
        //
        // GET: /Manager/
        public int PageSize = 10;
        private IClientRepository repository;

        public ManagerController(IClientRepository repo)
        {
            repository = repo;
        }
        [ImportModelStateFromTempData]
        public ViewResult List(int page,StatusClient? status)
        {
            //ModelState.Merge(TempData["ModelState"] as ModelStateDictionary);
            try
            {
                ClientListViewModel viewModel = new ClientListViewModel
                {
                    Clients = repository.Clients
                      .Where(p => status == null || p.Status == status)
                      .OrderBy(p => p.ClientId)
                      .Skip((page - 1) * PageSize)
                      .Take(PageSize).ToList(),
                    PagingInfo = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = PageSize,
                        TotalItems = status == null ?
                          repository.Clients.Count() :
                          repository.Clients.Where(e => e.Status == status).Count()
                    },
                    CurrentStatusClient = status
                };
                return View(viewModel);
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Repository service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(String.Format("Repository service error when get Clients:{0} ", ex.ToString()), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}, try again later.",ex.GetType().ToString(), ex.Message));
                Logger.Log.Error(String.Format("{0} when get Clients:{1} ", ex.GetType().ToString(), ex.ToString()), ex);
            }
            return View(new ClientListViewModel{Clients=new List<Client>(),CurrentStatusClient = null, PagingInfo = new PagingInfo{CurrentPage = 1,ItemsPerPage = PageSize,TotalItems = 0}});
        }

        public ViewResult Edit(int clientId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Client client;
            try
            {
                client = repository.Clients.FirstOrDefault(p => p.ClientId == clientId);
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Repository service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(String.Format("Repository service error when get Client ({0}) :{1} ", clientId, ex.ToString()), ex);
                client = new Client();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty,string.Format("{0} error:{1}, try again later.", ex.GetType().ToString(), ex.Message));
                Logger.Log.Error( String.Format("{0} when get Client ({1}) :{2} ", ex.GetType().ToString(), clientId, ex.ToString()),ex);
                client = new Client();
            }
            
            ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
            return View(client);
        }

        [HttpPost]
        public ActionResult Edit(Client client, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (ModelState.IsValid)
            {
                try
                {
                    repository.SaveClient(client);
                    TempData["message"] = string.Format("{0} {1} has been saved", client.FirstName, client.LastName);
                    return Redirect(returnUrl ?? Url.Action("List", "Manager"));
                }
                catch (EntityException ex)
                {
                    ModelState.AddModelError(String.Empty,
                        string.Format("Repository service error:{0}, client not saved, try again later.", ex.Message));
                    Logger.Log.Error(
                        String.Format("Repository service error when saving Client ({0}) :{1} ", client.ClientId,
                            ex.ToString()), ex);
                    ViewData["StatusList"] = Enum.GetValues(typeof (StatusClient)).Cast<StatusClient>();
                    return View(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(String.Empty,
                        string.Format("{0} error:{1}, client not saved, try again later.", ex.GetType(), ex.Message));
                    Logger.Log.Error(
                        String.Format("{0} when saving Client ({1}) :{2} ", ex.GetType(), client.ClientId, ex.ToString()),
                        ex);
                    ViewData["StatusList"] = Enum.GetValues(typeof (StatusClient)).Cast<StatusClient>();
                    return View(client);
                }
            }

            ViewData["StatusList"] = Enum.GetValues(typeof (StatusClient)).Cast<StatusClient>();
            return View(client);
        }

        public ViewResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            ViewData["StatusList"] = Enum.GetValues(typeof(StatusClient)).Cast<StatusClient>();
            return View("Edit",new Client{DateBirth = DateTime.Today});
        }

        [ExportModelStateToTempData]
        public ActionResult Delete(int clientId, string returnUrl)
        {
            try
            {
                Client deletedClient = repository.DeleteClient(clientId);

                if (deletedClient != null)
                {
                    TempData["message"] = string.Format("{0} {1} - was deleted", deletedClient.FirstName,deletedClient.LastName);
                }
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("Repository service error:{0}, client not deleted, try again later.",ex.Message));
                Logger.Log.Error(String.Format("Repository service error when deleting Client ({0}) :{1} ", clientId, ex.ToString()), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(String.Empty, string.Format("{0} error:{1}, client not deleted, try again later.",ex.GetType(), ex.Message));
                Logger.Log.Error(String.Format("{0} when deleting Client ({1}) :{2} ",ex.GetType(), clientId, ex.ToString()), ex);
            }

            return Redirect(returnUrl??Url.Action("List", "Manager"));
        }
    }
}
