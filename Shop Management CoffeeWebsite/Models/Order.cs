using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace CafeManagement.Models
{
    // Hóa đơn của khách. Status: Open (đang gọi món) | Paid (đã thanh toán)
    [Table("Orders")]
    public class Order
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        [StringLength(100)]
        public string CustomerName { get; set; }

        [Required, StringLength(20)]
        public string Status { get; set; }

        [StringLength(30)]
        public string PaymentMethod { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? PaidAt { get; set; }

        public decimal? PaidAmount { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }

        [NotMapped]
        public decimal Total
        {
            get
            {
                if (Status == "Paid" && PaidAmount.HasValue) return PaidAmount.Value;
                return Items == null ? 0m : Items.Sum(i => i.UnitPrice * i.Quantity);
            }
        }

        [NotMapped]
        public int ItemCount
        {
            get { return Items == null ? 0 : Items.Sum(i => i.Quantity); }
        }
    }

    [Table("OrderItems")]
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        [Required, StringLength(150)]
        public string ProductName { get; set; }

        // Giá chốt tại thời điểm đặt món
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }

        [NotMapped]
        public decimal LineTotal
        {
            get { return UnitPrice * Quantity; }
        }
    }
}
