using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using ChhayaNirh.Models;

namespace ChhayaNirh.Controllers
{
    public class SignupController : Controller
    {
        // GET: Signup
        public ActionResult Signup()
        {
            return View();
        }

        // POST: Signup
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
                        Password = password,  // ⚠ Hash this in real app
                        UserType = "User",
                        NIDScanPath = "/Uploads/NID/" + nidFileName,
                        ElectricityBillPath = "/Uploads/Bills/" + billFileName,
                        CreatedAt = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                return RedirectToAction("Login", "Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error during signup: " + ex.Message;
                return View();
            }
        }
    }
}
