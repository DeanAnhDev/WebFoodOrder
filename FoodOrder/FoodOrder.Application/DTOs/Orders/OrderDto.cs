using FoodOrder.Application.DTOs.Foods.Image;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Orders
{
    public class OrderDto
    {
        public int OrderId { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Note { get; set; }
        public string? Address { get; set; }
        public decimal SubtotalAmount { get; set; }
        public decimal VoucherDiscountAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ShipFee { get; set; }
        public int? VoucherId { get; set; }
        public string? Reason { get; set; }
        public List<OrderDetailDto> OrderDetails { get; set; } = new();
    }

    public class OrderDetailDto
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public decimal OriginalPrice { get; set; }
        public decimal DiscountedPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public ImageDto? ItemImage { get; set; } // Ảnh của Food hoặc Combo
    }
}