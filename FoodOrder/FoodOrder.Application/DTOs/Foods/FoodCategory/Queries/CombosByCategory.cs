using FoodOrder.Application.DTOs.Foods.Combo.Queries;


namespace FoodOrder.Application.DTOs.Foods.FoodCategory.Queries
{
    public class CombosByCategory
    {
        public int FoodCategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Slug { get; set; }
        public List<ComboDto>? Combos { get; set; }
    }
}
