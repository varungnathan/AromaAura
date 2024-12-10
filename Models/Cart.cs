using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AromaAura.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }  // Foreign key to ApplicationUser
        public ApplicationUser User { get; set; }  // Navigation property to User
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<CartItem> CartItems { get; set; }
    }
}
