using System.Web.Mvc;
using System.Web.Routing;

namespace DrivingSclApp
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "Login", id = UrlParameter.Optional }, // Parameter defaults
                new[] { "DrivingSclApp.Controllers" } // Prioritized namespace
            );
        }
    }
}
