using System;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChhayaNirh.Models;

namespace ChhayaNirh.Controllers
{
    public class HomeController : Controller
    {
        // ✅ GET: Signup
        public ActionResult Signup()
        {
            return View();
        }

        // ✅ POST: Signup
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(HttpPostedFileBase NIDScan, HttpPostedFileBase ElectricityBill, FormCollection form)
        {
            try
            {
                string fullName = form["FullName"];
                string email = form["Email"];
                string phone = form["Phone"];
                string password = form["Password"];
                string confirmPassword = form["ConfirmPassword"];

                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match.";
                    return View();
                }

                string nidFileName = null;
                string billFileName = null;

                if (NIDScan != null && NIDScan.ContentLength > 0)
                {
                    nidFileName = Path.GetFileName(NIDScan.FileName);
                    string nidPath = Path.Combine(Server.MapPath("~/Uploads/NID"), nidFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(nidPath));
                    NIDScan.SaveAs(nidPath);
                }

                if (ElectricityBill != null && ElectricityBill.ContentLength > 0)
                {
                    billFileName = Path.GetFileName(ElectricityBill.FileName);
                    string billPath = Path.Combine(Server.MapPath("~/Uploads/Bills"), billFileName);
                    Directory.CreateDirectory(Path.GetDirectoryName(billPath));
                    ElectricityBill.SaveAs(billPath);
                }

                using (var db = new ApplicationDbContext())
                {
                    User newUser = new User
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Password = password,   // ⚠️ Hash in production
                        UserType = "User",
                        NIDScanPath = "/Uploads/NID/" + nidFileName,
                        ElectricityBillPath = "/Uploads/Bills/" + billFileName,
                        CreatedAt = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error during signup: " + ex.Message;
                return View();
            }
        }

        // ✅ GET: Login
        public ActionResult Login()
        {
            return View();
        }

        // ✅ POST: Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(FormCollection form)
        {
            string phone = form["Phone"];
            string password = form["Password"];

            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.FirstOrDefault(u => u.Phone == phone && u.Password == password);

                if (user != null)
                {
                    Session["UserId"] = user.Id;
                    Session["UserName"] = user.FullName;
                    Session["UserType"] = user.UserType;

                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.Error = "Invalid phone number or password.";
                    return View();
                }
            }
        }

        // ✅ Logout
        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index");
        }

        // GET: Profile
        public ActionResult Profile()
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

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
                return RedirectToAction("Login");

            int userId = Convert.ToInt32(Session["UserId"]);
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(userId);
                if (user == null)
                    return HttpNotFound();

                return View(user);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditProfile(HttpPostedFileBase ProfilePicture, FormCollection form, string RemoveProfilePicture)
        {
            if (Session["UserId"] == null)
                return RedirectToAction("Login");

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
                // Handle Profile Picture Upload
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


        // Existing pages
        public ActionResult Index() => View();

        public ActionResult About()
        {
            ViewBag.Message = "About ChhayaNirh-ছায়ানীড়.";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact us.";
            return View();
        }

        public ActionResult Post() => View();
        public ActionResult Chat() => View();
    }
}
