using FoodOrder.Domain.Entities.Image;

namespace FoodOrder.Domain.Entities.Foods
{
    public class FoodCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public ICollection<Food>? Foods { get; set; }
        public ICollection<Combo>? Combos { get; set; }
        public Images? Images { get; set; }
    }
}
