using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.FoodCategory.Commands
{
    public class FoodCategoryDtoCreate
    {
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public ImageDto? Images { get; set; }
    }
}
