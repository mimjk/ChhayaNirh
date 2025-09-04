using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChhayaNirh.Models;
using ChhayaNirh.Helpers;

namespace ChhayaNirh.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Login()
        {
            return View("Login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form)
        {
            string phone = form["Phone"]?.Trim();
            string password = form["Password"];

            if (string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password))
            {
                TempData["Error"] = "Please enter both phone number and password.";
                return View("Login");
            }

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Phone == phone);

                if (user != null && PasswordHelper.VerifyPassword(password, user.Password))
                {
                    // Generate JWT token
                    string token = JwtHelper.GenerateToken(user.Id, user.FullName, user.UserType);

                    // Set cookie with proper configuration
                    var cookie = new HttpCookie("AuthToken", token)
                    {
                        HttpOnly = true,
                        Secure = Request.IsSecureConnection, // Use HTTPS in production
                        Expires = DateTime.Now.AddDays(7),
                        SameSite = SameSiteMode.Lax // Important for modern browsers
                    };
                    Response.Cookies.Add(cookie);

                    // Keep session for compatibility but sync with JWT
                    Session.Timeout = 60; // 60 minutes
                    Session["UserId"] = user.Id;
                    Session["UserName"] = user.FullName;
                    Session["UserType"] = user.UserType;

                    TempData["Success"] = $"Welcome back, {user.FullName}!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Invalid phone number or password.";
                    return View("Login");
                }
            }
        }

        public ActionResult Logout()
        {
            // Clear session
            Session.Clear();
            Session.Abandon();

            // Remove JWT cookie properly
            var cookie = new HttpCookie("AuthToken", "")
            {
                Expires = DateTime.Now.AddDays(-1),
                HttpOnly = true
            };
            Response.Cookies.Add(cookie);

            TempData["Success"] = "You have been logged out successfully.";
            return RedirectToAction("Index", "Home");
        }
    }
}