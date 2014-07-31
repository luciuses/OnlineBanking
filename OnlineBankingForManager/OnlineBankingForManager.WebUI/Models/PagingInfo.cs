// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingInfo.cs" company="">
//   
// </copyright>
// <summary>
//   The paging info.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.Models
{
    using System;

    /// <summary>
    /// The paging info.
    /// </summary>
    public class PagingInfo
    {
        /// <summary>
        /// The _current page.
        /// </summary>
        private int _currentPage;

        /// <summary>
        /// The _items per page.
        /// </summary>
        private int _itemsPerPage;

        /// <summary>
        /// Gets or sets the total items.
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Gets or sets the items per page.
        /// </summary>
        public int ItemsPerPage
        {
            get
            {
                return _itemsPerPage <= 0 ? 1 : _itemsPerPage;
            }

            set
            {
                _itemsPerPage = value;
            }
        }

        /// <summary>
        /// Gets or sets the current page.
        /// </summary>
        public int CurrentPage
        {
            get
            {
                return _currentPage;
            }

            set
            {
                _currentPage = value > TotalPages ? TotalPages : value;
            }
        }

        /// <summary>
        /// Gets the total pages.
        /// </summary>
        public int TotalPages
        {
            get
            {
                var tp = (int)Math.Ceiling((decimal)TotalItems / ItemsPerPage);
                return tp <= 0 ? 1 : tp;
            }
        }
    }
}