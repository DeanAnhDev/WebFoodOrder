using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.DTOs.Foods.Image;

namespace FoodOrder.Application.DTOs.Foods.Combo.Queries
{
    public class ComboDto
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public string? Slug { get; set; }
        public int FoodCategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int Quantity { get; set; }
        public int Sold { get; set; } 
        public DateTime CreatedAt { get; set; }
        public ImageDto? Images { get; set; }
        public FoodCategoryDto? FoodCategorys { get; set; }

    }
}
