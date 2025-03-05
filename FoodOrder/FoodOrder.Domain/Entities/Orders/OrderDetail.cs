
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Entities.Orders
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }
        public int OrderId { get; set; }
        public Order? Order { get; set; }
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}
