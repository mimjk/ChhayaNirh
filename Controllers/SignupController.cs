using System;
using System.Linq;
using System.Web.Mvc;
using ChhayaNirh.Models;
using ChhayaNirh.Helpers;
using System.Text.RegularExpressions;

namespace ChhayaNirh.Controllers
{
    public class SignupController : Controller
    {
        public ActionResult Signup()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Signup(FormCollection form)
        {
            try
            {
                string fullName = form["FullName"]?.Trim();
                string email = form["Email"]?.Trim();
                string phone = form["Phone"]?.Trim();
                string password = form["Password"];
                string confirmPassword = form["ConfirmPassword"];
                string nidNumber = form["NIDNumber"]?.Trim();
                string dateOfBirthString = form["DateOfBirth"];

                // Basic validation
                if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(email) ||
                    string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(password) ||
                    string.IsNullOrEmpty(nidNumber) || string.IsNullOrEmpty(dateOfBirthString))
                {
                    TempData["Error"] = "All fields are required.";
                    return View();
                }

                if (!DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
                {
                    TempData["Error"] = "Please enter a valid date of birth.";
                    return View();
                }

                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age)) age--;
                if (age < 18)
                {
                    TempData["Error"] = "You must be at least 18 years old to register.";
                    return View();
                }

                if (!Regex.IsMatch(phone, @"^01[0-9]{9}$"))
                {
                    TempData["Error"] = "Phone number must be 11 digits and start with 01.";
                    return View();
                }

                if (!Regex.IsMatch(nidNumber, @"^[0-9]{17}$"))
                {
                    TempData["Error"] = "NID must be exactly 17 digits and contain only numbers.";
                    return View();
                }

                if (password != confirmPassword)
                {
                    TempData["Error"] = "Passwords do not match.";
                    return View();
                }

                if (password.Length < 6)
                {
                    TempData["Error"] = "Password must be at least 6 characters long.";
                    return View();
                }

                using (var db = new ApplicationDbContext())
                {
                    if (db.Users.Any(u => u.Phone == phone))
                    {
                        TempData["Error"] = "A user with this phone number already exists.";
                        return View();
                    }

                    if (db.Users.Any(u => u.Email == email))
                    {
                        TempData["Error"] = "A user with this email already exists.";
                        return View();
                    }

                    if (db.Users.Any(u => u.NIDNumber == nidNumber))
                    {
                        TempData["Error"] = "A user with this NID already exists.";
                        return View();
                    }

                    // Generate 6-digit verification code
                    var code = new Random().Next(100000, 999999).ToString();

                    User newUser = new User
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Password = PasswordHelper.HashPassword(password),
                        UserType = "User",
                        NIDNumber = nidNumber,
                        DateOfBirth = dateOfBirth,
                        CreatedAt = DateTime.Now,
                        EmailVerificationCode = code,
                        EmailVerificationExpiry = DateTime.Now.AddMinutes(5),
                        IsEmailVerified = false
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();

                    // Send email with verification code
                    EmailHelper.SendEmail(newUser.Email, "Email Verification Code",
                        $"Your verification code is: {code}. It will expire in 5 minutes.");

                    // Redirect to verification page
                    return RedirectToAction("VerifyEmail", new { id = newUser.Id });
                }
            }
            catch (Exception)
            {
                TempData["Error"] = "Registration failed. Please try again.";
                return View();
            }
        }

        // GET: Verify Email Page
        public ActionResult VerifyEmail(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null) return HttpNotFound();

                return View(user);
            }
        }

        // POST: Verify Email
        [HttpPost]
        public ActionResult VerifyEmail(int id, string code)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null) return HttpNotFound();

                if (user.EmailVerificationCode == code && user.EmailVerificationExpiry > DateTime.Now)
                {
                    user.IsEmailVerified = true;
                    db.SaveChanges();
                    TempData["Success"] = "Email verified successfully! You can now log in.";
                    return RedirectToAction("Login", "Login");
                }

                TempData["Error"] = "Invalid or expired verification code.";
                return RedirectToAction("VerifyEmail", new { id = id });
            }
        }

        // POST: Resend Code
        [HttpPost]
        public ActionResult ResendVerificationCode(int id)
        {
            using (var db = new ApplicationDbContext())
            {
                var user = db.Users.Find(id);
                if (user == null) return HttpNotFound();

                var code = new Random().Next(100000, 999999).ToString();
                user.EmailVerificationCode = code;
                user.EmailVerificationExpiry = DateTime.Now.AddMinutes(5);
                db.SaveChanges();

                EmailHelper.SendEmail(user.Email, "New Verification Code",
                    $"Your new verification code is: {code}. It will expire in 5 minutes.");

                TempData["Success"] = "A new verification code has been sent to your email.";
                return RedirectToAction("VerifyEmail", new { id = id });
            }
        }
    }
}
