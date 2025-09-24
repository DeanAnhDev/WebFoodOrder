using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;

namespace FoodOrder.Application.DTOs.Foods
{
    public class FoodAndComboResponse
    {
        public List<FoodDto> Foods { get; set; } = new List<FoodDto>();
        public List<ComboDto> Combos { get; set; } = new List<ComboDto>();
        public int TotalFoods { get; set; }
        public int TotalCombos { get; set; }
        public int TotalItems => TotalFoods + TotalCombos;
    }
}