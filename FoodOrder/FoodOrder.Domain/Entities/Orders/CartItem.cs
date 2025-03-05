using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Entities.Orders
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        public Cart? Cart { get; set; }
        public int? FoodId { get; set; }
        public Food? Food { get; set; }
        public int? ComboId { get; set; }
        public Combo? Combo { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
