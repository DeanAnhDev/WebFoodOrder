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
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Unpaid;
        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.CashOnDelivery;
        public string? Note { get; set; }
        public string? Address { get; set; }

        public decimal SubtotalAmount { get; set; }        // Tổng tiền sau khi đã giảm món
        public decimal VoucherDiscountAmount { get; set; } // Số tiền giảm từ voucher
        public decimal TotalAmount { get; set; }           // Subtotal - VoucherDiscount
        public decimal ShipFee { get; set; }
        public int? VoucherId { get; set; }                // Null nếu không áp dụng
        public Voucher? Voucher { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
        public string? Reason { get; set; }
    }

    public enum OrderStatus
    {
        Pending,        // Chờ xử lý
        Accepted,       // Nhà hàng đã xác nhận 
        Processing,     // Đang xử lý
        Done,           //Nhà hàng đã làm xong
        Shipping,       // Đang giao hàng
        Completed,      // Hoàn thành
        Cancelled       // Đã hủy
    }

    public enum PaymentStatus
    {
        Unpaid,         // Chưa thanh toán
        Paid,           // Đã thanh toán
        Fail,           //Thanh toán thất bại   
    }

    public enum PaymentMethod
    {
        CashOnDelivery, // Thanh toán khi nhận hàng
        BankTransfer,   // Chuyển khoản ngân hàng
    }

}
