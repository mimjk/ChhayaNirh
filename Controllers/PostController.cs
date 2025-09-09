using ChhayaNirh.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;

namespace ChhayaNirh.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: Post
        [HttpGet]
        public ActionResult CreatePost()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreatePost(Post post, HttpPostedFileBase[] MediaFiles)
        {
            // Get the current user ID from session
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            post.UserId = userId;
            post.CreatedAt = DateTime.Now;
            post.LikeCount = 0; // Initialize like count

            // Save Latitude and Longitude from form
            if (Request.Form["Latitude"] != null && Request.Form["Longitude"] != null)
            {
                if (double.TryParse(Request.Form["Latitude"], out double lat))
                {
                    post.Latitude = lat;
                }
                if (double.TryParse(Request.Form["Longitude"], out double lng))
                {
                    post.Longitude = lng;
                }
            }

            // ✅ Check for required fields before saving
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Please fill up all required information.";
                return View(post); // Return to form with entered data
            }

            // Handle media upload
            if (MediaFiles != null && MediaFiles.Length > 0)
            {
                var paths = new List<string>();
                foreach (var file in MediaFiles)
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Uploads"), fileName);
                        file.SaveAs(path);
                        paths.Add("/Uploads/" + fileName);
                    }
                }
                post.MediaPaths = string.Join(",", paths);
            }


            db.Posts.Add(post);
            db.SaveChanges();

            return RedirectToAction("Post");
        }

        public ActionResult Index()
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            ViewBag.ProfilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                ? Url.Content("~/Content/Images/default-profile2.png")
                : Url.Content(user.ProfilePicturePath);

            return View();
        }

        public ActionResult Post(string postType, string[] areas, decimal? minBudget, decimal? maxBudget)
        {
            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            ViewBag.ProfilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                ? Url.Content("~/Content/Images/default-profile2.png")
                : Url.Content(user.ProfilePicturePath);

            var posts = db.Posts.Include("User").AsQueryable();

            if (!string.IsNullOrEmpty(postType))
            {
                posts = posts.Where(p => p.PostType == postType);
            }

            if (areas != null && areas.Length > 0)
            {
                posts = posts.Where(p => areas.Contains(p.Area));
            }

            if (minBudget.HasValue)
            {
                posts = posts.Where(p =>
                    (p.PostType == "Owner" && p.RentAmount >= minBudget) ||
                    (p.PostType == "Renter" && p.Budget >= minBudget));
            }

            if (maxBudget.HasValue)
            {
                posts = posts.Where(p =>
                    (p.PostType == "Owner" && p.RentAmount <= maxBudget) ||
                    (p.PostType == "Renter" && p.Budget <= maxBudget));
            }

            var filteredPosts = posts.OrderByDescending(p => p.CreatedAt).ToList();

            // Get current user's likes to determine liked status
            var currentUserLikes = db.Likes
                .Where(l => l.UserId == userId)
                .Select(l => l.PostId)
                .ToList();

            ViewBag.UserLikes = currentUserLikes;

            return View(filteredPosts);
        }

        // : Post Details Action
        public ActionResult PostDetails(int? id) // make id nullable
        {
            if (id == null)
            {
                // If no id provided, redirect to Post list or show error
                return RedirectToAction("Post");
            }

            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == id.Value);
            if (post == null)
            {
                return HttpNotFound();
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            ViewBag.ProfilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                ? Url.Content("~/Content/Images/default-profile2.png")
                : Url.Content(user.ProfilePicturePath);

            // Check if current user liked this post
            var isLiked = db.Likes.Any(l => l.PostId == id.Value && l.UserId == userId);
            ViewBag.IsLiked = isLiked;

            // Pass current user ID to check if they can like the post (can't like own post)
            ViewBag.CurrentUserId = userId;
            ViewBag.CanLike = post.UserId != userId; // User cannot like their own post

            // Add IsAdmin check - you'll need to implement this based on your User model
            // For now, setting it to false. Replace this with your actual admin check logic
            ViewBag.IsAdmin = false; // TODO: Implement actual admin check logic

            // Alternative admin check examples (uncomment and modify based on your implementation):
            // ViewBag.IsAdmin = user.UserType == "Admin"; 
            // ViewBag.IsAdmin = user.IsAdmin; // if you have an IsAdmin property

            ViewBag.PostId = id.Value;
            return View(post);
        }

        //For Delete Post
        [HttpPost]
        public JsonResult DeletePost(int id)
        {
            try
            {
                if (Session["UserId"] == null)
                    return Json(new { success = false, error = "User not logged in" });

                int userId = Convert.ToInt32(Session["UserId"]);
                var post = db.Posts.Include("Comments").Include("Likes").FirstOrDefault(p => p.Id == id);

                if (post == null)
                    return Json(new { success = false, error = "Post not found" });

                if (post.UserId != userId)
                    return Json(new { success = false, error = "You are not authorized to delete this post" });

                // Delete related comments
                if (post.Comments != null && post.Comments.Any())
                {
                    db.Comments.RemoveRange(post.Comments);
                }

                // Delete related likes
                if (post.Likes != null && post.Likes.Any())
                {
                    db.Likes.RemoveRange(post.Likes);
                }

                db.Posts.Remove(post);
                db.SaveChanges();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // Add this GET method to your PostController.cs

        [HttpGet]
        public ActionResult VerifyPost(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Post");
            }

            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == id.Value);

            if (post == null)
            {
                return HttpNotFound();
            }

            // Check if the current user is the owner of the post
            if (post.UserId != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to verify this post.";
                return RedirectToAction("PostDetails", new { id = id.Value });
            }

            return View(post);
        }

        // Update your existing POST method to redirect back to PostDetails
        [HttpPost]
        public ActionResult VerifyPost(int PostId, HttpPostedFileBase ElectricityBill)
        {
            if (Session["UserId"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var post = db.Posts.Find(PostId);

            if (post == null || post.UserId != userId)
            {
                TempData["ErrorMessage"] = "You are not authorized to verify this post.";
                return RedirectToAction("PostDetails", new { id = PostId });
            }

            if (ElectricityBill != null && ElectricityBill.ContentLength > 0)
            {
                // Check if uploaded file is an image
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                string fileExtension = Path.GetExtension(ElectricityBill.FileName).ToLower();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["ErrorMessage"] = "Please upload a valid image file (jpg, jpeg, png, gif, bmp).";
                    return View(post);
                }

                try
                {
                    // Create unique filename to prevent conflicts
                    string fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ElectricityBill.FileName);
                    string uploadPath = Server.MapPath("~/Uploads");

                    // Create directory if it doesn't exist
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    string filePath = Path.Combine(uploadPath, fileName);
                    ElectricityBill.SaveAs(filePath);

                    // Save relative path to database
                    post.ElectricityBillPath = "/Uploads/" + fileName;
                    db.SaveChanges();

                    TempData["SuccessMessage"] = "Electricity bill uploaded successfully!";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Error uploading file: " + ex.Message;
                    return View(post);
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Please select an image file to upload.";
                return View(post);
            }

            return RedirectToAction("PostDetails", new { id = PostId });
        }


        // AJAX method to toggle likes (like/unlike)
        [HttpPost]
        public JsonResult ToggleLike(int postId)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, error = "User not logged in" });
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if the post exists
                var post = db.Posts.Find(postId);
                if (post == null)
                {
                    return Json(new { success = false, error = "Post not found" });
                }

                // Prevent users from liking their own posts
                if (post.UserId == userId)
                {
                    return Json(new { success = false, error = "You cannot like your own post" });
                }

                // Check if user already liked this post
                var existingLike = db.Likes
                    .FirstOrDefault(l => l.PostId == postId && l.UserId == userId);

                bool isLiked;

                if (existingLike != null)
                {
                    // User already liked, so unlike it
                    db.Likes.Remove(existingLike);
                    isLiked = false;
                }
                else
                {
                    // User hasn't liked, so add like
                    var like = new Like
                    {
                        PostId = postId,
                        UserId = userId,
                        CreatedAt = DateTime.Now
                    };
                    db.Likes.Add(like);
                    isLiked = true;
                }

                db.SaveChanges();

                // Get updated like count
                var likeCount = db.Likes.Count(l => l.PostId == postId);

                // Update post like count in the database
                post.LikeCount = likeCount;
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    likeCount = likeCount,
                    isLiked = isLiked
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // AJAX method to get comments
        [HttpGet]
        public PartialViewResult GetComments(int postId)
        {
            try
            {
                var comments = db.Comments
                    .Include(c => c.User) // Use lambda expression instead of string
                    .Where(c => c.PostId == postId)
                    .OrderBy(c => c.CreatedAt) // Show oldest first, or use OrderByDescending for newest first
                    .ToList();

                System.Diagnostics.Debug.WriteLine($"Found {comments.Count} comments for post {postId}");

                // Debug: Check if users are loaded
                foreach (var comment in comments)
                {
                    System.Diagnostics.Debug.WriteLine($"Comment: {comment.CommentText}, User: {comment.User?.FullName ?? "NULL"}");
                }

                return PartialView("_Comments", comments);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading comments: {ex.Message}");
                // Return empty list instead of null to prevent errors
                return PartialView("_Comments", new List<Comment>());
            }
        }

        // Also update your AddComment method to ensure proper saving:
        [HttpPost]
        public JsonResult AddComment(int postId, string commentText)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, error = "User not logged in" });
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                if (string.IsNullOrWhiteSpace(commentText))
                {
                    return Json(new { success = false, error = "Comment text cannot be empty" });
                }

                // Verify post exists
                var post = db.Posts.Find(postId);
                if (post == null)
                {
                    return Json(new { success = false, error = "Post not found" });
                }

                var comment = new Comment
                {
                    PostId = postId,
                    UserId = userId,
                    CommentText = commentText.Trim(),
                    CreatedAt = DateTime.Now
                };

                db.Comments.Add(comment);
                var result = db.SaveChanges();

                System.Diagnostics.Debug.WriteLine($"Comment saved. Rows affected: {result}");

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error adding comment: {ex.Message}");
                return Json(new { success = false, error = ex.Message });
            }
        }
         // ✅ NEW METHOD: Submit Report
        [HttpPost]
        public JsonResult SubmitReport(int postId, string reportType, string description)
        {
            try
            {
                if (Session["UserId"] == null)
                {
                    return Json(new { success = false, error = "User not logged in" });
                }

                int userId = Convert.ToInt32(Session["UserId"]);

                // Check if the post exists
                var post = db.Posts.Find(postId);
                if (post == null)
                {
                    return Json(new { success = false, error = "Post not found" });
                }

                // Prevent users from reporting their own posts
                if (post.UserId == userId)
                {
                    return Json(new { success = false, error = "You cannot report your own post" });
                }

                // Check if user already reported this post
                var existingReport = db.Reports
                    .FirstOrDefault(r => r.PostId == postId && r.ReportedByUserId == userId);

                if (existingReport != null)
                {
                    return Json(new { success = false, error = "You have already reported this post" });
                }

                // Create new report
                var report = new Report
                {
                    PostId = postId,
                    ReportedByUserId = userId,
                    ReportType = reportType,
                    Description = description?.Trim(),
                    ReportedAt = DateTime.Now,
                    IsResolved = false
                };

                db.Reports.Add(report);
                db.SaveChanges();

                return Json(new { success = true, message = "Report submitted successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }
        // Action for comment modal
        public ActionResult CommentModal(int postId)
        {
            var post = db.Posts.Include("User").FirstOrDefault(p => p.Id == postId);
            if (post == null)
            {
                return HttpNotFound();
            }

            int userId = Convert.ToInt32(Session["UserId"]);
            var user = db.Users.Find(userId);

            ViewBag.ProfilePicturePath = string.IsNullOrEmpty(user.ProfilePicturePath)
                ? Url.Content("~/Content/Images/default-profile2.png")
                : Url.Content(user.ProfilePicturePath);

            ViewBag.PostId = postId;
            return View(post);
        }
    }
}