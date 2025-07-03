
using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.Combo
{
    public class ComboWithFoodDto
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public string? Slug { get; set; }
        public int FoodCategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "true";
        public ImageDto? Images { get; set; }
        public List<ComboDetailDto>? ComboDetails { get; set; }
    }
}
