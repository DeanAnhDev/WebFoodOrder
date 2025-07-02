
namespace FoodOrder.Domain.Entities.Orders
{
    public class Voucher
    {
        public int VoucherId { get; set; }

        public string Code { get; set; } = string.Empty;
        public string? Description { get; set; }

        public float? DiscountPercent { get; set; }   // Giảm theo %
        public decimal? DiscountAmount { get; set; }    // Giảm cố định

        public bool IsActive { get; set; } = true;
        public int? Quantity { get; set; }              // Số lượt dùng còn lại (nếu có giới hạn)

        public decimal? MinOrderAmount { get; set; }    // Điều kiện áp dụng (vd: đơn > 200k)

        public ICollection<Order>? Orders { get; set; }
    }

}
