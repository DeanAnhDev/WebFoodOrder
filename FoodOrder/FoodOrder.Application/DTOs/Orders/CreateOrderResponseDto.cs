namespace FoodOrder.Application.DTOs.Orders
{
    public class CreateOrderResponseDto
    {
        public OrderDto Order { get; set; } = default!;
        public string? PaymentUrl { get; set; }
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; } = true;
    }
}