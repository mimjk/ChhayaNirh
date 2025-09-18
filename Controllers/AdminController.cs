using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChhayaNirh.Models; // make sure your ApplicationDbContext is in this namespace
using System.Data.Entity;  // for Include()

namespace ChhayaNirh.Controllers
{
    public class AdminController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // Redirect /Admin to /Admin/Dashboard
        public ActionResult Admin()
        {
            return RedirectToAction("Dashboard");
        }

        // Admin panel pages
        public ActionResult Dashboard() => View();
        public ActionResult PostManagement() => View();
        public ActionResult PaymentHandling() => View();
        public ActionResult ReportManagement() => View();
        public ActionResult VerificationQueue() => View();
        public ActionResult Analytics() => View();

        // GET: Admin/Settings
        public ActionResult Settings()
        {
            return View();
        }

        // POST: Admin/Settings
        [HttpPost]
        public ActionResult Settings(string maintenanceMode, int reportThreshold, int verificationTimeout)
        {
            // TODO: Save these values to the database or config file
            ViewBag.Message = "Settings saved successfully!";
            return View();
        }

        // ================= User Management =================

        // 🔍 User Listing with search and status filters
        public ActionResult UserManagement(string search, string status)
        {
            var users = db.Users.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                users = users.Where(u => u.FullName.Contains(search) || u.Email.Contains(search));

            if (!string.IsNullOrEmpty(status))
            {
                switch (status)
                {
                    case "Verified": users = users.Where(u => u.IsVerified); break;
                    case "NotVerified": users = users.Where(u => !u.IsVerified); break;
                    case "Banned": users = users.Where(u => u.IsBanned); break;
                    case "Active": users = users.Where(u => !u.IsBanned); break;
                }
            }

            return View(users.ToList());
        }

        // User details
        public ActionResult UserDetails(int id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            // Load posts/reports separately (no Include)
            ViewBag.Posts = db.Posts.Where(p => p.UserId == id).ToList();
            ViewBag.Reports = db.Reports.Where(r => r.ReportedByUserId == id).ToList();

            return View(user);
        }

        // Ban/Unban/Delete as before...
        [HttpPost]
        public ActionResult BanUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsBanned = true;
                db.SaveChanges();
            }
            return RedirectToAction("UserDetails", new { id });
        }

        [HttpPost]
        public ActionResult UnbanUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                user.IsBanned = false;
                db.SaveChanges();
            }
            return RedirectToAction("UserDetails", new { id });
        }

        [HttpPost]
        public ActionResult DeleteUser(int id)
        {
            var user = db.Users.Find(id);
            if (user != null)
            {
                db.Users.Remove(user);
                db.SaveChanges();
            }
            return RedirectToAction("UserManagement");
        }

        public ActionResult Details()
        {
            return View(); // This will look for Views/Admin/Details.cshtml
        }
    }
}
