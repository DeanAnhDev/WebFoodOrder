namespace FoodOrder.Application.DTOs.Carts
{
    public class CartResponse
    {
        public CartDto Cart { get; set; } = default!;
        public int TotalQuantity { get; set; }
        public decimal Subtotal { get; set; }
    }
}
