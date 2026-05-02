using System;
using System.Collections.Generic;

namespace CuaHangQuanAo.Data.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Trạng thái: Pending, Paid...
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}