using System;
using System.ComponentModel.DataAnnotations;

namespace ChhayaNirh.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] public string FullName { get; set; }
        [Required] public string Email { get; set; }
        [Required]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Phone number must be exactly 11 digits")]
        [RegularExpression(@"^01[0-9]{9}$", ErrorMessage = "Phone number must start with 01 and be 11 digits")]
        public string Phone { get; set; }

        [Required] public string Password { get; set; } // Will be hashed
        [Required] public string UserType { get; set; } // Homeowner / Renter

        [StringLength(17, MinimumLength = 17, ErrorMessage = "NID must be exactly 17 digits")]
        [RegularExpression(@"^[0-9]{17}$", ErrorMessage = "NID must be exactly 17 digits and contain only numbers")]
        public string NIDNumber { get; set; }

        [Required]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }


        // Add calculated Age property
        public int Age
        {
            get
            {
                var today = DateTime.Today;
                var age = today.Year - DateOfBirth.Year;
                if (DateOfBirth.Date > today.AddYears(-age)) age--;
                return age;
            }
        }

        // Removed ElectricityBillPath
        public string ProfilePicturePath { get; set; }
        public string PresentAddress { get; set; } // New field
        public string PermanentAddress { get; set; } // Add this line
        public string NIDDocumentPath { get; set; }  // File path of uploaded NID
        public bool IsVerified { get; set; } = false;  // Admin will mark true after approval(checking NID)
        public bool IsEmailVerified { get; set; } = false;
        public string EmailVerificationCode { get; set; }
        public DateTime? EmailVerificationExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}