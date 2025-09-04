using ChhayaNirh.Attributes;
using ChhayaNirh.Helpers;
using ChhayaNirh.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace ChhayaNirh.Controllers
{
    public class ProfileController : Controller
    {
        [JwtAuthorize]
        public new ActionResult Profile()
        {
            int userId = GetCurrentUserId();
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                return View(user);
            }
        }

        [JwtAuthorize]
        public ActionResult EditProfile()
        {
            int userId = GetCurrentUserId();
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                return View(user);
            }
        }

        [HttpPost]
        // TEMPORARILY REMOVE [ValidateAntiForgeryToken] to test
        [JwtAuthorize]
        public ActionResult EditProfile(HttpPostedFileBase ProfilePicture, FormCollection form, string RemoveProfilePicture)
        {
            int userId = GetCurrentUserId();

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                try
                {
                    // Update Email
                    string newEmail = form["Email"]?.Trim();
                    if (!string.IsNullOrEmpty(newEmail) && newEmail != user.Email)
                    {
                        if (db.Users.Any(u => u.Email == newEmail && u.Id != userId))
                        {
                            TempData["Error"] = "Email already exists.";
                            return View(user);
                        }
                        user.Email = newEmail;
                    }

                    // Update Present Address
                    user.PresentAddress = form["PresentAddress"]?.Trim();

                    // Update Permanent Address
                    user.PermanentAddress = form["PermanentAddress"]?.Trim();

                    // Remove profile picture if requested
                    if (!string.IsNullOrEmpty(RemoveProfilePicture) && RemoveProfilePicture == "true")
                    {
                        user.ProfilePicturePath = null;
                    }
                    else if (ProfilePicture != null && ProfilePicture.ContentLength > 0)
                    {
                        // Validate file type
                        string[] allowedTypes = { ".jpg", ".jpeg", ".png" };
                        string fileExtension = Path.GetExtension(ProfilePicture.FileName).ToLower();

                        if (!allowedTypes.Contains(fileExtension))
                        {
                            TempData["Error"] = "Only JPG, JPEG, and PNG files are allowed.";
                            return View(user);
                        }

                        string fileName = Guid.NewGuid() + fileExtension;
                        string folder = Server.MapPath("~/Uploads/ProfilePictures/");
                        Directory.CreateDirectory(folder);
                        string path = Path.Combine(folder, fileName);
                        ProfilePicture.SaveAs(path);

                        user.ProfilePicturePath = "/Uploads/ProfilePictures/" + fileName;
                    }

                    db.SaveChanges();
                    TempData["Success"] = "Profile updated successfully!";
                }
                catch (Exception)
                {
                    TempData["Error"] = "Failed to update profile. Please try again.";
                }
            }

            return RedirectToAction("Profile");
        }

        private int GetCurrentUserId()
        {
            // First check session (most reliable)
            if (Session["UserId"] != null)
                return (int)Session["UserId"];

            // Check ClaimsIdentity set by JwtAuthorizeAttribute
            if (User?.Identity?.IsAuthenticated == true && User is ClaimsPrincipal claimsPrincipal)
            {
                var userIdClaim = claimsPrincipal.FindFirst("UserId");
                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                {
                    // Sync back to session for consistency
                    Session["UserId"] = userId;
                    return userId;
                }
            }

            // Last resort: directly validate JWT token
            var token = Request.Cookies["AuthToken"]?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                var principal = JwtHelper.ValidateToken(token);
                if (principal != null)
                {
                    var userIdClaim = principal.FindFirst("UserId");
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
                    {
                        // Sync to session for next time
                        Session["UserId"] = userId;
                        return userId;
                    }
                }
            }

            throw new UnauthorizedAccessException("User not authenticated");
        }
    }
}