
namespace FoodOrder.Application.DTOs.Carts
{
    public class AddToCartRequest
    {
        public int? FoodId { get; set; }
        public int? ComboId { get; set; }
        public int Quantity { get; set; }
    }
}
