using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace OnlineBankingForManager.WebUI.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        private int _itemsPerPage;

        public int ItemsPerPage
        {
            get { return _itemsPerPage <= 0 ? 1 : _itemsPerPage; }
            set { _itemsPerPage = value; }
        }

        private int _currentPage;

        public int CurrentPage
        {
            get { return _currentPage; }
            set { _currentPage = value > TotalPages ? TotalPages : value; }
        }

        public int TotalPages
        {
            get
            {
                var tp = (int) Math.Ceiling((decimal) TotalItems/ItemsPerPage);
                return tp<=0?1:tp ; 
            }
        }
    }
}