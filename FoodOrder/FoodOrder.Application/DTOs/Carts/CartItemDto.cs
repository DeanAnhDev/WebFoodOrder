using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;

namespace FoodOrder.Application.DTOs.Carts
{
    public class CartItemDto
    {
        public int CartItemId { get; set; }
        public int CartId { get; set; }
        //public Cart? Cart { get; set; }
        public int? FoodId { get; set; }
        public FoodDto? Food { get; set; }
        public int? ComboId { get; set; }
        public ComboDto? Combo { get; set; }
        public int Quantity { get; set; }
    }
}
