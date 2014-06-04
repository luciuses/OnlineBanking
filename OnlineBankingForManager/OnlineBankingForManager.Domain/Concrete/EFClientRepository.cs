using System.Linq;
using OnlineBankingForManager.Domain.Abstract;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.Domain.Concrete
{
    public class EFClientRepository : IClientRepository
    {
        private EFDbContext context = new EFDbContext();
        public IQueryable<Client> Clients
        {
            get { return context.Clients; }
        }

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