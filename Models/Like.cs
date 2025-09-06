using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChhayaNirh.Models
{
    public class Like
    {
        public int Id { get; set; }

        [Required]
        public int PostId { get; set; }

        [Required]
        public int UserId { get; set; }

        public DateTime CreatedAt { get; set; }

        // Navigation properties
        [ForeignKey("PostId")]
        public virtual Post Post { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}