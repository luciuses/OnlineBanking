// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EFClientRepository.cs" company="">
//   
// </copyright>
// <summary>
//   The ef client repository.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.Domain.Concrete
{
    using System.Linq;
    using OnlineBankingForManager.Domain.Abstract;
    using OnlineBankingForManager.Domain.Entities;

    /// <summary>
    /// The ef client repository.
    /// </summary>
    public class EFClientRepository : IClientRepository
    {
        /// <summary>
        /// The context.
        /// </summary>
        private readonly EFDbContext context = new EFDbContext();

        /// <summary>
        /// Gets the clients.
        /// </summary>
        public IQueryable<Client> Clients
        {
            get
            {
                return context.Clients;
            }
        }

        /// <summary>
        /// The save client.
        /// </summary>
        /// <param name="client">
        /// The client.
        /// </param>
        public void SaveClient(Client client)
        {
            if (client.ClientId == 0)
            {
                context.Clients.Add(client);
            }
            else
            {
                Client dbEntry = context.Clients.Find(client.ClientId);
                if (dbEntry != null)
                {
                    dbEntry.ContractNumber = client.ContractNumber;
                    dbEntry.FirstName = client.FirstName;
                    dbEntry.LastName = client.LastName;
                    dbEntry.DateBirth = client.DateBirth;
                    dbEntry.Deposit = client.Deposit;
                    dbEntry.PhoneNumber = client.PhoneNumber;
                    dbEntry.Status = client.Status;
                }
            }

            context.SaveChanges();
        }

        /// <summary>
        /// The delete client.
        /// </summary>
        /// <param name="cliendId">
        /// The cliend id.
        /// </param>
        /// <returns>
        /// The <see cref="Client"/>.
        /// </returns>
        public Client DeleteClient(int cliendId)
        {
            Client dbEntry = context.Clients.Find(cliendId);
            if (dbEntry != null)
            {
                context.Clients.Remove(dbEntry);
                context.SaveChanges();
            }

            return dbEntry;
        }
    }
}