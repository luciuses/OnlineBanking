// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RouteConfig.cs" company="">
//   
// </copyright>
// <summary>
//   The route config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace OnlineBankingForManager.WebUI
{
    using System.Web.Mvc;
    using System.Web.Routing;
    using OnlineBankingForManager.Domain.Entities;

    /// <summary>
    /// The route config.
    /// </summary>
    public class RouteConfig
    {
        /// <summary>
        /// The register routes.
        /// </summary>
        /// <param name="routes">
        /// The routes.
        /// </param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                null, 
                string.Empty, 
                new { controller = "Manager", action = "List", status = (StatusClient?)null, order = (string)null, page = 1, pageSize = 10 });

            routes.MapRoute(null, "{controller}/{action}");
        }
    }
}