using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.WebUI.Models
{
    
    public class ClientListViewModel
    {
        public IEnumerable<Client> Clients { get; set; }
        public PagingInfo PagingInfo { get; set; }
        public StatusClient? CurrentStatusClient { get; set; }
        public string CurrentOrderClients { get; set; }
    }
}