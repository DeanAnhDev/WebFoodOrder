
using FoodOrder.Application.DTOs.Foods.Combo;


namespace FoodOrder.Application.DTOs.Foods.FoodCategory
{
    public class CombosByCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public List<ComboDto>? Combos { get; set; }
    }
}
