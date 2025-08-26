

using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Voucher
{
    public class VoucherCreateDto
    {
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public VoucherType Type { get; set; }
        public int Quantity { get; set; }
        public decimal MinOrderPrice { get; set; }
        public decimal MaxDiscountPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}
