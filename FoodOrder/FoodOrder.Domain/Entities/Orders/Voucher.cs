
using System.Text.Json.Serialization;

namespace FoodOrder.Domain.Entities.Orders
{
    public class Voucher
    {
        public int VoucherId { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal DiscountAmount { get; set; }
        public VoucherType Type { get; set; }
        public int Quantity { get; set; }
        public decimal MinOrderPrice { get; set; }
        public decimal MaxDiscountPrice { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }

        public ICollection<Order>? Orders { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum VoucherType
    {
        Amount = 1,
        Percentage = 2
    }
}
