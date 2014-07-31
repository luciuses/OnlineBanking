// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PagingHelpers.cs" company="">
//   
// </copyright>
// <summary>
//   The paging helpers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI.HtmlHelpers
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using OnlineBankingForManager.WebUI.Models;

    /// <summary>
    /// The paging helpers.
    /// </summary>
    public static class PagingHelpers
    {
        /// <summary>
        /// The page links.
        /// </summary>
        /// <param name="html">
        /// The html.
        /// </param>
        /// <param name="pagingInfo">
        /// The paging info.
        /// </param>
        /// <param name="pageUrl">
        /// The page url.
        /// </param>
        /// <returns>
        /// The <see cref="MvcHtmlString"/>.
        /// </returns>
        public static MvcHtmlString PageLinks(this HtmlHelper html, PagingInfo pagingInfo, Func<int, string> pageUrl)
        {
            var result = new StringBuilder();

            for (int i = 1; i <= pagingInfo.TotalPages; i++)
            {
                var tag = new TagBuilder("a"); // Construct an <a> tag
                tag.MergeAttribute("href", pageUrl(i));
                tag.InnerHtml = i.ToString();
                if (i == pagingInfo.CurrentPage)
                {
                    tag.AddCssClass("selected");
                }

                result.Append(tag);
            }

            return MvcHtmlString.Create(result.ToString());
        }
    }
}