using System.ComponentModel.DataAnnotations;

namespace ChhayaNirh.Models
{
    public class User
    {
        [Key] // Primary key
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; }

        // Optional: you can add more fields
        // public string Phone { get; set; }
        // public DateTime CreatedAt { get; set; }
    }
}
