using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.DTOs.Foods.Image;
using FoodOrder.Application.DTOs.Foods.Promotion;


namespace FoodOrder.Application.DTOs.Foods.Food.Queries
{
    public class FoodDto
    {
        public int FoodId { get; set; }
        public int FoodCategoryId { get; set; }
        public string? FoodName { get; set; }
        public string? Slug { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public bool Status { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; }
        public DateTime CreatedAt { get; set; }
        public ImageDto? Images { get; set; }
        public FoodCategoryDto? FoodCategory { get; set; }
        public PromotionDtoSelect? Promotion { get; set; }

    }
}
