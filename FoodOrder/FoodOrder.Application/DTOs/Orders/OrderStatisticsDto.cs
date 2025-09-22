using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Orders
{
    public class OrderStatusCountDto
    {
        public OrderStatus Status { get; set; }
        public int Count { get; set; }
        public string StatusName { get; set; } = string.Empty;
    }

    public class OrderStatisticsDto
    {
        public List<OrderStatusCountDto> StatusCounts { get; set; } = new();
        public int TotalOrders { get; set; }
    }
}