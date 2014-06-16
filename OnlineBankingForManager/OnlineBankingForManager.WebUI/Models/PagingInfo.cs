using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineBankingForManager.WebUI.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        private int _currentPage;
        public int CurrentPage { get { return _currentPage; } set { _currentPage = value>TotalPages?TotalPages:value; } }

        public int TotalPages
        {
            get { return (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage); }
        }
    }
}