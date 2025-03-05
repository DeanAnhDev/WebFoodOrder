using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Domain.Entities.Foods
{
    public class Combo
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public int FoodCategoryId { get; set; }
        public FoodCategory? FoodCategorys { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "true";
        public ICollection<ComboDetail>? ComboDetails { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
