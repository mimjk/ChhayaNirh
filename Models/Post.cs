using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChhayaNirh.Models
{
    public class Post
    {
        public int Id { get; set; }

        public int UserId { get; set; } // Foreign key to User

        [Required(ErrorMessage = "Please select post type")]
        public string PostType { get; set; } // "Owner" or "Renter"

        [Required(ErrorMessage = "Please enter area")]
        public string Area { get; set; }

        public string HouseType { get; set; } // Flat, Apartment, etc.
        public int? Bedrooms { get; set; }
        public int? Bathrooms { get; set; }
        public decimal? RentAmount { get; set; } // For Owner
        public decimal? Budget { get; set; } // For Renter

        public string Amenities { get; set; } // Comma-separated list
        public string Description { get; set; }

        public string MediaPaths { get; set; } // Comma-separated image/video paths

        public DateTime CreatedAt { get; set; }
        public int LikeCount { get; set; } = 0;
        [ForeignKey("UserId")]
        public virtual User User { get; set; } // Navigation property
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string ElectricityBillPath { get; set; }


    }
}
