using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ChhayaNirh
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Custom clean routes
            routes.MapRoute(
                name: "Login",
                url: "Login",
                defaults: new { controller = "Login", action = "Login" }
            );

            routes.MapRoute(
                name: "Signup",
                url: "Signup",
                defaults: new { controller = "Signup", action = "Signup" }
            );

            routes.MapRoute(
                name: "Chat",
                url: "Chat",
                defaults: new { controller = "Chat", action = "Chat" }
            );

            routes.MapRoute(
                name: "Post",
                url: "Post",
                defaults: new { controller = "Post", action = "Post" }
            );

            routes.MapRoute(
                name: "Profile",
                url: "Profile",
                defaults: new { controller = "Profile", action = "Profile" }
            );

            routes.MapRoute(
                name: "Admin",
                url: "Admin",
                defaults: new { controller = "Admin", action = "Admin" }
            );

            routes.MapRoute(
                name: "Terms",
                url: "Terms",
                defaults: new { controller = "Terms", action = "Terms" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
