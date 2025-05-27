
namespace FoodOrder.Domain.Entities.Foods
{
    public class FoodCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<Food>? Foods { get; set; }
        public ICollection<Combo>? Combos { get; set; }
    }
}
