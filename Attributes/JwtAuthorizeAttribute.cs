using ChhayaNirh.Helpers;
using System;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ChhayaNirh.Attributes
{
    public class JwtAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // First check if user is already authenticated via session
            if (httpContext.Session["UserId"] != null)
            {
                // Create consistent ClaimsIdentity for session users
                var sessionUserId = httpContext.Session["UserId"].ToString();
                var sessionUserName = httpContext.Session["UserName"]?.ToString() ?? "";
                var sessionUserType = httpContext.Session["UserType"]?.ToString() ?? "User";

                var sessionIdentity = new ClaimsIdentity("Session");
                sessionIdentity.AddClaim(new Claim("UserId", sessionUserId));
                sessionIdentity.AddClaim(new Claim("UserName", sessionUserName));
                sessionIdentity.AddClaim(new Claim("UserType", sessionUserType));

                httpContext.User = new ClaimsPrincipal(sessionIdentity);
                return true;
            }

            // Fallback to JWT token validation
            var token = httpContext.Request.Cookies["AuthToken"]?.Value;
            if (string.IsNullOrEmpty(token))
                return false;

            var principal = JwtHelper.ValidateToken(token);
            if (principal == null)
                return false;

            // Extract claims from JWT
            var userIdClaim = principal.FindFirst("UserId");
            var userNameClaim = principal.FindFirst("UserName");
            var userTypeClaim = principal.FindFirst("UserType");

            if (userIdClaim == null)
                return false;

            // Create consistent ClaimsIdentity
            var identity = new ClaimsIdentity("JWT");
            identity.AddClaim(new Claim("UserId", userIdClaim.Value));
            identity.AddClaim(new Claim("UserName", userNameClaim?.Value ?? ""));
            identity.AddClaim(new Claim("UserType", userTypeClaim?.Value ?? "User"));

            // Sync with session for consistency
            if (int.TryParse(userIdClaim.Value, out int userId))
            {
                httpContext.Session["UserId"] = userId;
                httpContext.Session["UserName"] = userNameClaim?.Value ?? "";
                httpContext.Session["UserType"] = userTypeClaim?.Value ?? "User";
                httpContext.Session.Timeout = 60; // 60 minutes
            }

            httpContext.User = new ClaimsPrincipal(identity);
            return true;
        }

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Clear any invalid session data
            filterContext.HttpContext.Session.Clear();

            // Redirect to login
            filterContext.Result = new RedirectResult("~/Login/Login");
        }
    }
}