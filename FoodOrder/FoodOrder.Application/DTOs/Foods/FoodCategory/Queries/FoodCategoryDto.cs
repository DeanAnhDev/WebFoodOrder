using FoodOrder.Application.DTOs.Foods.Image;


namespace FoodOrder.Application.DTOs.Foods.FoodCategory.Queries
{
    public class FoodCategoryDto
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public ImageDto? Images { get; set; }
    }
}
