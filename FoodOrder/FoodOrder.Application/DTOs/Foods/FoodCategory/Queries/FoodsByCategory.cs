using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Food.Queries;

namespace FoodOrder.Application.DTOs.Foods.FoodCategory.Queries
{
    public class FoodsByCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public List<FoodDto>? Foods { get; set; }
    }
}
