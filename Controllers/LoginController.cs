using System.Linq;
using System.Web.Mvc;
using ChhayaNirh.Models;

namespace ChhayaNirh.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Login()
        {
            return View("Login"); // Looks for Views/Login/Login.cshtml
        }

        // POST: Login
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.Error = "Invalid phone number or password.";
                    return View("Login"); // Point again to Login.cshtml
                }
            }
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}
