using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.Food.Commands
{
    public class FoodDtoCreate
    {
        public int FoodCategoryId { get; set; }
        public string? FoodName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public ImageDto? Images { get; set; }
        public int Quantity { get; set; }
    }
}
