

namespace FoodOrder.Application.DTOs.Foods.Combo
{
    public class ComboDto
    {
        public int ComboId { get; set; }
        public string? ComboName { get; set; }
        public int FoodCategoryId { get; set; }
        public decimal Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = "true";
    }
}
