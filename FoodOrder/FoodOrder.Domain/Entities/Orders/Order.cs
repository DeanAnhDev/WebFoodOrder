

using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Entities.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = Guid.NewGuid().ToString("N")[..10].ToUpper();

        public int UserId { get; set; }
        public AppUser? Users { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";

        public string? Note { get; set; }

        public decimal SubtotalAmount { get; set; }        // Tổng tiền sau khi đã giảm món
        public decimal VoucherDiscountAmount { get; set; } // Số tiền giảm từ voucher
        public decimal TotalAmount { get; set; }           // Subtotal - VoucherDiscount

        public int? VoucherId { get; set; }                // Null nếu không áp dụng
        public Voucher? Voucher { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }

}
