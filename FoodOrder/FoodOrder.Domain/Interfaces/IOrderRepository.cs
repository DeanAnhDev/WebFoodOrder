using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;

namespace FoodOrder.Domain.Interfaces
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersWithPaginationAsync(
            string? orderCode = null,
            int? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc");

        Task<Dictionary<OrderStatus, int>> GetOrderCountByStatusAsync(
            string? orderCode = null,
            int? userId = null,
            PaymentStatus? paymentStatus = null);

        Task<Order?> GetOrderByIdWithDetailsAsync(int orderId);
    }
}