
using FoodOrder.Application.DTOs.Orders;

namespace FoodOrder.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto, int userId);
        Task<bool> ProcessPaymentCallbackAsync(string orderCode, string responseData);
    }
}
