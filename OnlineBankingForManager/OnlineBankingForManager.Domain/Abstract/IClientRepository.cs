// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IClientRepository.cs" company="">
//   
// </copyright>
// <summary>
//   The ClientRepository interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.Domain.Abstract
{
    using System.Linq;
    using OnlineBankingForManager.Domain.Entities;

    // Interface for Client's data base.
    /// <summary>
    /// The ClientRepository interface.
    /// </summary>
    public interface IClientRepository
    {
        /// <summary>
        /// Gets the clients.
        /// </summary>
        IQueryable<Client> Clients { get; }

        /// <summary>
        /// The save client.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        void SaveClient(Client client);

        /// <summary>
        /// The delete client.
        /// </summary>
        /// <param name="clientId">
        /// The client id.
        /// </param>
        /// <returns>
        /// The <see cref="Client"/>.
        /// </returns>
        Client DeleteClient(int clientId);
    }
}