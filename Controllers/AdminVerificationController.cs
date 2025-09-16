using ChhayaNirh.Models;
using System;
using System.Linq;
using System.Web.Mvc;

namespace ChhayaNirh.Controllers
{
    public class AdminVerificationController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // Show list of users with uploaded NID but not verified
        public ActionResult NIDVerificationQueue()
        {
            var unverifiedUsers = db.Users
                .Where(u => !u.IsVerified && u.NIDDocumentPath != null)
                .ToList();

            return View(unverifiedUsers);
        }

        // Show details of one user
        public ActionResult VerifyUser(int id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();
            return View(user);
        }

        // Approve verification
        [HttpPost]
        public ActionResult ApproveVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            user.IsVerified = true;
            db.SaveChanges();

            TempData["Success"] = "User verified successfully.";
            return RedirectToAction("NIDVerificationQueue");
        }

        // Reject verification
        [HttpPost]
        public ActionResult RejectVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null) return HttpNotFound();

            user.IsVerified = false;
            db.SaveChanges();

            TempData["Error"] = "User verification rejected.";
            return RedirectToAction("NIDVerificationQueue");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
