namespace FoodOrder.Application.DTOs.Carts
{
    public class UpdateCartItemByIdRequest
    {
        public int CartId { get; set; }
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}