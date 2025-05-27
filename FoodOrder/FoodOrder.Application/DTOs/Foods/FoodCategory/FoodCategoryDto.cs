
namespace FoodOrder.Application.DTOs.Foods.FoodCategory
{
    public class FoodCategoryDto
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
    }
}
