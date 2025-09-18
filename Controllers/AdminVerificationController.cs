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

        // -------------------- NID Verification --------------------

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

        // Approve Verification
        [HttpPost]
        public ActionResult ApproveVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("NIDVerificationQueue");
            }

            user.IsVerified = true;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            int adminId = 3; // Admin user ID
            var message = new Chat
            {
                SenderId = adminId,
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

        // Reject Verification
        [HttpPost]
        public ActionResult RejectVerification(int id)
        {
            var user = db.Users.Find(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("NIDVerificationQueue");
            }

            user.IsVerified = false;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();

            int adminId = 3; // Admin user ID
            var message = new Chat
            {
                SenderId = adminId,
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

        // -------------------- Post Verification --------------------

        // Show list of posts that are not verified
        public ActionResult PostVerificationQueue()
        {
            var unverifiedPosts = db.Posts.Include("User")
                                         .Where(p => !p.IsVerified)
                                         .ToList();
            return View(unverifiedPosts);
        }

        // Show details of a specific post for verification
        public ActionResult VerifyPost(int id)
        {
            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == id);
            if (post == null) return HttpNotFound();
            return View(post);
        }

        // Approve a post
        [HttpPost]
        public ActionResult ApprovePost(int id)
        {
            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == id);
            if (post == null) return HttpNotFound();

            post.IsVerified = true;
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();

            db.Chats.Add(new Chat
            {
                SenderId = 3, // Admin ID
                ReceiverId = post.UserId,
                MessageText = "✅ Your post verification is successful.",
                SentAt = DateTime.Now,
                IsDelivered = false,
                IsRead = false
            });
            db.SaveChanges();

            TempData["Success"] = "Post verified and user notified.";
            return RedirectToAction("PostVerificationQueue");
        }

        // Reject a post
        [HttpPost]
        public ActionResult RejectPost(int id)
        {
            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == id);
            if (post == null) return HttpNotFound();

            post.IsVerified = false;
            db.Entry(post).State = EntityState.Modified;
            db.SaveChanges();

            db.Chats.Add(new Chat
            {
                SenderId = 3, // Admin ID
                ReceiverId = post.UserId,
                MessageText = "❌ Your post verification was denied. Please check and update your information.",
                SentAt = DateTime.Now,
                IsDelivered = false,
                IsRead = false
            });
            db.SaveChanges();

            TempData["Error"] = "Post rejected and user notified.";
            return RedirectToAction("PostVerificationQueue");
        }

        // -------------------- Dispose --------------------
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
