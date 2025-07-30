using FoodOrder.Application.DTOs.Foods.Food.Queries;

namespace FoodOrder.Application.DTOs.Foods.Combo.Queries
{
    public class ComboDetailDto
    {
        public int ComboId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public FoodDto Food { get; set; } = new FoodDto();

    }
}
