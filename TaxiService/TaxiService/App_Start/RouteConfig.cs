using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace TaxiService
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Musterija",
                url: "Musterija",
                defaults: new { controller = "Home", action = "Musterija" }
            );
            routes.MapRoute(
                name: "Dispecer",
                url: "Dispecer",
                defaults: new { controller = "Home", action = "Dispecer" }
            );

            routes.MapRoute(
                name: "Vozac",
                url: "Vozac",
                defaults: new { controller = "Home", action = "Vozac" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
