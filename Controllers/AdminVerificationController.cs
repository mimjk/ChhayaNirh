using ChhayaNirh.Models;
using System;
using System.Data.Entity;
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

        // 🔹 Approve Verification - FIXED
        [HttpPost]
        public ActionResult ApproveVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("NIDVerificationQueue");
            }

            // Update verification status
            user.IsVerified = true;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // --- Send Auto Message from Admin ---
            int adminId = 3; // <-- Your real admin Id from Users table

            var message = new Chat
            {
                SenderId = adminId,       // must exist in Users table
                ReceiverId = user.Id,
                MessageText = "✅ Your profile verification is successful.",
                SentAt = DateTime.Now,
                IsDelivered = false,
                IsRead = false
            };

            db.Chats.Add(message);
            db.SaveChanges();

            TempData["Success"] = "User has been verified successfully, and notification sent.";
            return RedirectToAction("NIDVerificationQueue");
        }

        // 🔹 Reject Verification - FIXED
        [HttpPost]
        public ActionResult RejectVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("NIDVerificationQueue");
            }

            // Keep verification false
            user.IsVerified = false;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            // --- Send Auto Message from Admin ---
            int adminId = 3; // <-- Your real admin Id

            var message = new Chat
            {
                SenderId = adminId,       // must exist in Users table
                ReceiverId = user.Id,
                MessageText = "❌ Your profile verification was denied. Please check and upload correct information.",
                SentAt = DateTime.Now,
                IsDelivered = false,
                IsRead = false
            };

            db.Chats.Add(message);
            db.SaveChanges();

            TempData["Error"] = "User verification has been rejected, and notification sent.";
            return RedirectToAction("NIDVerificationQueue");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
