using System.Linq;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.Domain.Abstract
{
    public interface IClientRepository 
    {
        IQueryable<Client> Clients { get; }
        void SaveClient(Client client);
        Client DeleteClient(int clientId);
    }
}