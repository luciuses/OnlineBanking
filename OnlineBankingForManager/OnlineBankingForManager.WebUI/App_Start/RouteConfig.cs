using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using OnlineBankingForManager.Domain.Entities;

namespace OnlineBankingForManager.WebUI
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(null,
              "",
              new
              {
                  controller = "Manager",
                  action = "List",
                  status = (StatusClient?)null,
                  page = 1
              }
            );

            routes.MapRoute(null,
              "Page{page}",
              new { controller = "Manager", action = "List", status = (StatusClient?)null},
              new { page = @"\d+" }
            );

            routes.MapRoute(null,
              "{category}",
              new { controller = "Manager", action = "List", page = 1 }
            );

            routes.MapRoute(null,
              "{status}/Page{page}",
              new { controller = "Manager", action = "List" },
              new { page = @"\d+" }
            );

            routes.MapRoute(null, "{controller}/{action}");
        }
    }
}