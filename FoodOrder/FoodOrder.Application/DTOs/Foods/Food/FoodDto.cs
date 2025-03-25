
namespace FoodOrder.Application.DTOs.Foods.Food
{
    public class FoodDto
    {
        public int FoodId { get; set; }
        public int FoodCategoryId { get; set; }
        public string? FoodName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string Status { get; set; } = "true";
    }
}
