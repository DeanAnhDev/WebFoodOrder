using FoodOrder.Application.DTOs.Foods.Food;

namespace FoodOrder.Application.DTOs.Foods.FoodCategory
{
    public class FoodCategoryListFoodDto
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<FoodDto>? Foods { get; set; }
    }
}
