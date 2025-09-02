using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ChhayaNirh.Models;

namespace ChhayaNirh.Controllers
{
    public class ProfileController : Controller
    {
        // GET: Profile
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = (int)Session["UserId"];
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                return View(user);
            }
        }

        // GET: EditProfile
        public ActionResult EditProfile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = Convert.ToInt32(Session["UserId"]);
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                return View(user);
            }
        }

        // POST: EditProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(HttpPostedFileBase ProfilePicture, FormCollection form, string RemoveProfilePicture)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login", "Login");

            int userId = Convert.ToInt32(Session["UserId"]);
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                // Update Email
                user.Email = form["Email"];

                // Remove profile picture if requested
                if (!string.IsNullOrEmpty(RemoveProfilePicture) && RemoveProfilePicture == "true")
                {
                    user.ProfilePicturePath = null;
                }
                else if (ProfilePicture != null && ProfilePicture.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ProfilePicture.FileName);
                    string folder = Server.MapPath("~/Uploads/ProfilePictures/");
                    Directory.CreateDirectory(folder);
                    string path = Path.Combine(folder, fileName);
                    ProfilePicture.SaveAs(path);

                    user.ProfilePicturePath = "/Uploads/ProfilePictures/" + fileName;
                }

                db.SaveChanges();
            }

            return RedirectToAction("Profile");
        }
    }
}
