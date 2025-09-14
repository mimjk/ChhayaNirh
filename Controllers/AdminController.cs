using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChhayaNirh.Controllers
{
    public class AdminController : Controller
    {
        // Redirect /Admin to /Admin/Dashboard
        public ActionResult Admin()
        {
            return RedirectToAction("Dashboard");
        }

        // Admin panel pages
        public ActionResult Dashboard() => View();
        public ActionResult UserManagement() => View();
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


    }
}