
namespace FoodOrder.Domain.Entities.Foods
{
    public class FoodCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public virtual ICollection<Food>? Foods { get; set; }
    }
}
