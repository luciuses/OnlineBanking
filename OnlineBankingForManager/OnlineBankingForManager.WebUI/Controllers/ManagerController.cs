// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ManagerController.cs" company="">
//   
// </copyright>
// <summary>
//   The manager controller.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Core;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Web.Mvc;
    using OnlineBankingForManager.Domain.Abstract;
    using OnlineBankingForManager.Domain.Entities;
    using OnlineBankingForManager.WebUI.HtmlHelpers;
    using OnlineBankingForManager.WebUI.Models;

    /// <summary>
    /// The manager controller.
    /// </summary>
    [Authorize(Roles = "ActiveUser")]
    public partial class ManagerController : Controller
    {
        /// <summary>
        /// The repository.
        /// </summary>
        private readonly IClientRepository repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ManagerController"/> class.
        /// </summary>
        /// <param name="repo">
        /// The repo.
        /// </param>
        public ManagerController(IClientRepository repo)
        {
            repository = repo;
        }

        /// <summary>
        /// The list.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="pageSize">
        /// The page size.
        /// </param>
        /// <param name="status">
        /// The status.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <returns>
        /// The <see cref="ViewResult"/>.
        /// </returns>
        [ImportModelStateFromTempData]
        public virtual ViewResult List(int? page, int? pageSize, StatusClient? status, string order)
        {
            PagingInfo pi = null;
            try
            {
                pi = new PagingInfo
                         {
                             ItemsPerPage = pageSize ?? 10, 
                             TotalItems =
                                 status == null ? repository.Clients.Count() : repository.Clients.Where(e => e.Status == status).Count(), 
                             CurrentPage = page ?? 1
                         };

                var viewModel = new ClientListViewModel
                                    {
                                        PagingInfo = pi, 
                                        Clients =
                                            repository.Clients.Where(c => status == null || c.Status == status)
                                            .OrderBy(string.IsNullOrEmpty(order) ? "ClientId" : order)
                                            .Skip((pi.CurrentPage - 1) * pi.ItemsPerPage)
                                            .Take(pi.ItemsPerPage)
                                            .ToList(), 
                                        CurrentStatusClient = status, 
                                        CurrentOrderClients = order, 
                                        ClientsTotal = repository.Clients.Count(), 
                                        ClientsClassicStatus =
                                            repository.Clients.Where(c => c.Status == StatusClient.Classic).Count(), 
                                        ClientsVipStatus = repository.Clients.Where(c => c.Status == StatusClient.VIP).Count(), 
                                        ClientsUseDeposit = repository.Clients.Where(c => c.Deposit).Count()
                                    };

                return View(viewModel);
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Repository service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(string.Format("Repository service error when get Clients:{0} ", ex), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}, try again later.", ex.GetType(), ex.Message));
                Logger.Log.Error(string.Format("{0} when get Clients:{1} ", ex.GetType(), ex), ex);
            }

            pi = new PagingInfo { CurrentPage = page ?? 1, ItemsPerPage = pageSize ?? 10, TotalItems = 0 };
            return View(new ClientListViewModel { PagingInfo = pi, Clients = new List<Client>(), CurrentStatusClient = null });
        }

        /// <summary>
        /// The edit.
        /// </summary>
        /// <param name="clientId">
        /// The client id.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ViewResult"/>.
        /// </returns>
        public virtual ViewResult Edit(int clientId, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            Client client;
            try
            {
                client = repository.Clients.FirstOrDefault(p => p.ClientId == clientId);
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("Repository service error:{0}, try again later.", ex.Message));
                Logger.Log.Error(string.Format("Repository service error when get Client ({0}) :{1} ", clientId, ex), ex);
                client = new Client();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}, try again later.", ex.GetType(), ex.Message));
                Logger.Log.Error(string.Format("{0} when get Client ({1}) :{2} ", ex.GetType(), clientId, ex), ex);
                client = new Client();
            }

            return View(client);
        }

        /// <summary>
        /// The edit.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [HttpPost]
        public virtual ActionResult Edit(Client client, string returnUrl)
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
                    ModelState.AddModelError(
                        string.Empty, 
                        string.Format("Repository service error:{0}, client not saved, try again later.", ex.Message));
                    Logger.Log.Error(string.Format("Repository service error when saving Client ({0}) :{1} ", client.ClientId, ex), ex);
                    return View(client);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(
                        string.Empty, 
                        string.Format("{0} error:{1}, client not saved, try again later.", ex.GetType(), ex.Message));
                    Logger.Log.Error(string.Format("{0} when saving Client ({1}) :{2} ", ex.GetType(), client.ClientId, ex), ex);
                    return View(client);
                }
            }

            return View(client);
        }

        /// <summary>
        /// The create.
        /// </summary>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ViewResult"/>.
        /// </returns>
        public virtual ViewResult Create(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View("Edit", new Client { DateBirth = DateTime.Today });
        }

        /// <summary>
        /// The delete.
        /// </summary>
        /// <param name="clientId">
        /// The client id.
        /// </param>
        /// <param name="returnUrl">
        /// The return url.
        /// </param>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [ExportModelStateToTempData]
        public virtual ActionResult Delete(int clientId, string returnUrl)
        {
            try
            {
                Client deletedClient = repository.DeleteClient(clientId);

                if (deletedClient != null)
                {
                    TempData["message"] = string.Format("{0} {1} - was deleted", deletedClient.FirstName, deletedClient.LastName);
                }
            }
            catch (EntityException ex)
            {
                ModelState.AddModelError(
                    string.Empty, 
                    string.Format("Repository service error:{0}, client not deleted, try again later.", ex.Message));
                Logger.Log.Error(string.Format("Repository service error when deleting Client ({0}) :{1} ", clientId, ex), ex);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, string.Format("{0} error:{1}, client not deleted, try again later.", ex.GetType(), ex.Message));
                Logger.Log.Error(string.Format("{0} when deleting Client ({1}) :{2} ", ex.GetType(), clientId, ex), ex);
            }

            return Redirect(returnUrl ?? Url.Action(MVC.Manager.List()));
        }
    }
}