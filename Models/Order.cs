using System;
using System.Collections.Generic;

namespace AromaAura.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string ShippingAddress { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
