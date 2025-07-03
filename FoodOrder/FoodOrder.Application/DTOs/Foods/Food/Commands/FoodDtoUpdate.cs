using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.Food.Commands
{
    public class FoodDtoUpdate
    {
        public int FoodId { get; set; }
        public string? FoodName { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int FoodCategoryId { get; set; }
        public bool Status { get; set; }
        public bool IsOutOfStock { get; set; }
        public ImageDto? Images { get; set; }
       
    }
}
