using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(FoodOrderDbContext context) : base(context)
        {
        }

        public async Task<(IEnumerable<Order> Orders, int TotalCount)> GetOrdersWithPaginationAsync(
            string? orderCode = null,
            int? userId = null,
            OrderStatus? status = null,
            PaymentStatus? paymentStatus = null,
            int page = 1,
            int pageSize = 10,
            string sortBy = "CreatedAt",
            string sortOrder = "desc")
        {
            // Build query with filters
            var query = _dbSet
                .Include(o => o.Users)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food!)
                        .ThenInclude(f => f.Images)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo!)
                        .ThenInclude(c => c.Images)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(o => o.OrderCode.Contains(orderCode));
            }

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (status.HasValue)
            {
                query = query.Where(o => o.Status == status.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
            }

            // Get total count before pagination
            var totalCount = await query.CountAsync();

            // Apply sorting - default is by CreatedAt descending (newest first)
            if (!string.IsNullOrEmpty(sortBy))
            {
                var sortOrderLower = sortOrder?.ToLower() == "asc" ? "asc" : "desc";

                query = sortBy.ToLower() switch
                {
                    "createdat" => sortOrderLower == "asc"
                        ? query.OrderBy(o => o.CreatedAt)
                        : query.OrderByDescending(o => o.CreatedAt),
                    "ordercode" => sortOrderLower == "asc"
                        ? query.OrderBy(o => o.OrderCode)
                        : query.OrderByDescending(o => o.OrderCode),
                    "totalamount" => sortOrderLower == "asc"
                        ? query.OrderBy(o => o.TotalAmount)
                        : query.OrderByDescending(o => o.TotalAmount),
                    "status" => sortOrderLower == "asc"
                        ? query.OrderBy(o => o.Status)
                        : query.OrderByDescending(o => o.Status),
                    _ => query.OrderByDescending(o => o.CreatedAt) // Default sorting
                };
            }
            else
            {
                query = query.OrderByDescending(o => o.CreatedAt); // Default sorting
            }

            // Apply pagination
            var orders = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (orders, totalCount);
        }

        public async Task<Dictionary<OrderStatus, int>> GetOrderCountByStatusAsync(
            string? orderCode = null,
            int? userId = null,
            PaymentStatus? paymentStatus = null)
        {
            // Build base query with filters (excluding status filter)
            var query = _dbSet.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(orderCode))
            {
                query = query.Where(o => o.OrderCode.Contains(orderCode));
            }

            if (userId.HasValue)
            {
                query = query.Where(o => o.UserId == userId.Value);
            }

            if (paymentStatus.HasValue)
            {
                query = query.Where(o => o.PaymentStatus == paymentStatus.Value);
            }

            // Group by status and count
            var statusCounts = await query
                .GroupBy(o => o.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync();

            // Convert to dictionary and ensure all statuses are included
            var result = new Dictionary<OrderStatus, int>();

            // Initialize all statuses with 0
            foreach (OrderStatus status in Enum.GetValues<OrderStatus>())
            {
                result[status] = 0;
            }

            // Fill in actual counts
            foreach (var item in statusCounts)
            {
                result[item.Status] = item.Count;
            }

            return result;
        }

        public async Task<Order?> GetOrderByIdWithDetailsAsync(int orderId)
        {
            return await _dbSet
                .Include(o => o.Users)
                .Include(o => o.Voucher)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Food!)
                        .ThenInclude(f => f.Images)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Combo!)
                        .ThenInclude(c => c.Images)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }
    }
}