
using FoodOrder.Application.DTOs.Orders;
using FoodOrder.Application.DTOs.Revenue;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.Interfaces
{
    public interface IOrderService
    {
        Task<CreateOrderResponseDto> CreateOrderAsync(CreateOrderDto createOrderDto);
        Task<bool> ProcessPaymentCallbackAsync(string orderCode, string responseData);
        Task<bool> ProcessPaymentSuccessAsync(string orderCode);
        Task<bool> ProcessPaymentFailureAsync(string orderCode);
        Task<GetOrdersResponseDto> GetAllOrdersAsync(GetOrdersRequestDto request);
        Task<OrderStatisticsDto> GetOrderStatisticsAsync(
            string? orderCode = null,
            int? userId = null,
            PaymentStatus? paymentStatus = null);
        Task<UpdateOrderStatusResponseDto> UpdateOrderStatusAsync(UpdateOrderStatusDto request);
        Task<OrderDto?> GetOrderByIdAsync(int orderId);
        Task<RevenueResponseDto> GetRevenueAsync(RevenueRequestDto request);
    }
}
