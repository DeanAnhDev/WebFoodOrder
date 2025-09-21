
using FoodOrder.Application.DTOs.Orders;

namespace FoodOrder.Application.Interfaces
{
    public interface IOrderService
    {
        Task<OrderDto> CreateOrderAsync(CreateOrderDto createOrderDto, int userId);
    }
}
