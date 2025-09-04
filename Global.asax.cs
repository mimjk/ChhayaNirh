using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;

namespace ChhayaNirh
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Configure AntiForgery for JWT
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "UserId";
            AntiForgeryConfig.CookieName = "__RequestVerificationToken";
            AntiForgeryConfig.RequireSsl = false; // Set to true in production with HTTPS
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();

            // Handle anti-forgery token errors gracefully
            if (exception is HttpAntiForgeryException)
            {
                Response.Clear();
                Server.ClearError();

                // Redirect to login for auth issues
                Response.Redirect("~/Login/Login?error=session_expired");
                return;
            }
        }
    }
}