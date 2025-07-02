using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Domain.Entities.Foods
{
    public class Food
    {
        public int FoodId { get; set; }
        public int FoodCategoryId { get; set; }
        public FoodCategory? FoodCategory { get; set; }
        public string? FoodName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = "true";
        public bool IsOutOfStock { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ComboDetail>? ComboDetails { get; set; }
        public OrderDetail? OrderDetail { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public Promotion? Promotion { get; set; }
        public Images? Images { get; set; }
    }
}
