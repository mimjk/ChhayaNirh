using System;
using System.ComponentModel.DataAnnotations;

namespace ChhayaNirh.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required] public string FullName { get; set; }
        [Required] public string Email { get; set; }
        [Required] public string Phone { get; set; }
        [Required] public string Password { get; set; }
        [Required] public string UserType { get; set; } // Homeowner / Renter

        public string NIDScanPath { get; set; }
        public string ElectricityBillPath { get; set; }
        public string ProfilePicturePath { get; set; } // Stores the uploaded picture path
        public DateTime CreatedAt { get; set; }

    }
}
