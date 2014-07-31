// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ClientListViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The client list view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Models
{
    using System.Collections.Generic;
    using OnlineBankingForManager.Domain.Entities;

    /// <summary>
    /// The client list view model.
    /// </summary>
    public class ClientListViewModel
    {
        /// <summary>
        /// Gets or sets the clients.
        /// </summary>
        public IEnumerable<Client> Clients { get; set; }

        /// <summary>
        /// Gets or sets the paging info.
        /// </summary>
        public PagingInfo PagingInfo { get; set; }

        /// <summary>
        /// Gets or sets the current status client.
        /// </summary>
        public StatusClient? CurrentStatusClient { get; set; }

        /// <summary>
        /// Gets or sets the current order clients.
        /// </summary>
        public string CurrentOrderClients { get; set; }

        /// <summary>
        /// Gets or sets the clients total.
        /// </summary>
        public int ClientsTotal { get; set; }

        /// <summary>
        /// Gets or sets the clients vip status.
        /// </summary>
        public int ClientsVipStatus { get; set; }

        /// <summary>
        /// Gets or sets the clients classic status.
        /// </summary>
        public int ClientsClassicStatus { get; set; }

        /// <summary>
        /// Gets or sets the clients use deposit.
        /// </summary>
        public int ClientsUseDeposit { get; set; }
    }
}