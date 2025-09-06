using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Services.Description;

namespace ChhayaNirh.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; } // Foreign key to Post

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [Required]
        [StringLength(1000)]
        public string CommentText { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}