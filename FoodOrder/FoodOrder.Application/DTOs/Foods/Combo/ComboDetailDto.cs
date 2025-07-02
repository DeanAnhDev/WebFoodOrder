using FoodOrder.Application.DTOs.Foods.Food;

namespace FoodOrder.Application.DTOs.Foods.Combo
{
    public class ComboDetailDto
    {
        public int ComboId { get; set; }
        public int FoodId { get; set; }
        public int Quantity { get; set; }
        public FoodDto Food { get; set; } = new FoodDto();

    }
}
