using System;
using System.ComponentModel.DataAnnotations;

namespace ChhayaNirh.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; } // Foreign key to Post

        [Required]
        public int ReportedByUserId { get; set; } // Foreign key to User who reported

        [Required]
        [StringLength(100)]
        public string ReportType { get; set; } // Problem involving someone under 18, Bullying, harassment or abuse, Fraud Post, Others

        [StringLength(1000)]
        public string Description { get; set; } // Optional description provided by user

        public DateTime ReportedAt { get; set; } = DateTime.Now;

        public bool IsResolved { get; set; } = false; // For admin to mark as resolved
    }
}