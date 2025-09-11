using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChhayaNirh.Models
{
    public class Chat
    {
        public int Id { get; set; }

        [Required]
        public int SenderId { get; set; }

        [Required]
        public int ReceiverId { get; set; }

        [Required]
        [StringLength(1000)]
        public string MessageText { get; set; }

        [Required]
        public DateTime SentAt { get; set; } = DateTime.Now;

        // Message status tracking
        public bool IsDelivered { get; set; } = false;
        public DateTime? DeliveredAt { get; set; }

        public bool IsRead { get; set; } = false;
        public DateTime? ReadAt { get; set; }

        // Navigation properties
        [ForeignKey("SenderId")]
        public virtual User Sender { get; set; }

        [ForeignKey("ReceiverId")]
        public virtual User Receiver { get; set; }

        // Helper property to get message status
        [NotMapped]
        public string MessageStatus
        {
            get
            {
                if (IsRead) return "read";
                if (IsDelivered) return "delivered";
                return "sent";
            }
        }
    }
}