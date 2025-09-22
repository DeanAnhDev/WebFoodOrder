using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Orders
{
    public class UpdateOrderStatusDto
    {
        public int OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
        public string? Reason { get; set; }
    }

    public class UpdateOrderStatusResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public OrderDto? Order { get; set; }
    }
}