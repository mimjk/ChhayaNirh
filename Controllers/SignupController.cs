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

                // Date of Birth validation
                if (!DateTime.TryParse(dateOfBirthString, out DateTime dateOfBirth))
                {
                    TempData["Error"] = "Please enter a valid date of birth.";
                    return View();
                }

                // 18+ age validation
                var today = DateTime.Today;
                var age = today.Year - dateOfBirth.Year;
                if (dateOfBirth.Date > today.AddYears(-age)) age--;

                if (age < 18)
                {
                    TempData["Error"] = "You must be at least 18 years old to register.";
                    return View();
                }

                // Phone validation
                if (!Regex.IsMatch(phone, @"^01[0-9]{9}$"))
                {
                    TempData["Error"] = "Phone number must be 11 digits and start with 01.";
                    return View();
                }

                // NID validation
                if (!Regex.IsMatch(nidNumber, @"^[0-9]{17}$"))
                {
                    TempData["Error"] = "NID must be exactly 17 digits and contain only numbers.";
                    return View();
                }

                // Password validation
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
                    // Check if user already exists
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

                    User newUser = new User
                    {
                        FullName = fullName,
                        Email = email,
                        Phone = phone,
                        Password = PasswordHelper.HashPassword(password),
                        UserType = "User",
                        NIDNumber = nidNumber,
                        DateOfBirth = dateOfBirth,
                        CreatedAt = DateTime.Now
                    };

                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                TempData["Success"] = "Account created successfully! Please login.";
                return RedirectToAction("Login", "Login");
            }
            catch (Exception)
            {
                TempData["Error"] = "Registration failed. Please try again.";
                return View();
            }
        }
    }
}