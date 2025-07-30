
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Entities.Orders
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; } = null!;

        public int? FoodId { get; set; }          // Món lẻ (có thể null nếu là Combo)
        public Food? Food { get; set; }

        public int? ComboId { get; set; }         // Combo (có thể null nếu là Food)
        public Combo? Combo { get; set; }

        public string ItemName { get; set; } = null!;        // Snapshot tên tại thời điểm đặt
        public decimal OriginalPrice { get; set; }           // Giá gốc
        public decimal DiscountedPrice { get; set; }         // Giá sau khuyến mãi
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; } 
    }

}
