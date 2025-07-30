using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.FoodCategory.Commands
{
    public class FoodCategoryDtoUpdate
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public ImageDto? Images { get; set; }
    }
}
