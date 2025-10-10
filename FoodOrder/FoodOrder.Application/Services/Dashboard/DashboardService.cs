using AutoMapper;
using FoodOrder.Application.DTOs.Dashboard;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FoodOrder.Application.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<TopProductsResponseDto> GetTopProductsAsync(TopProductsRequestDto request)
        {
            try
            {
                // Mặc định lấy dữ liệu tháng hiện tại
                var startDate = request.StartDate?.Date ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = request.EndDate?.Date ?? startDate.AddMonths(1).AddDays(-1);

                // Query các đơn hàng đã hoàn thành và thanh toán trong khoảng thời gian
                var ordersQuery = _unitOfWork.Orders.GetQueryable()
                    .Where(o => o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Paid)
                    .Where(o => o.CreatedAt.Date >= startDate && o.CreatedAt.Date <= endDate);

                var orders = await ordersQuery.ToListAsync();

                // Lấy tất cả OrderDetails từ các đơn hàng
                var orderIds = orders.Select(o => o.OrderId).ToList();
                var orderDetails = await _unitOfWork.Orders.GetQueryable()
                    .Where(o => orderIds.Contains(o.OrderId))
                    .SelectMany(o => o.OrderDetails)
                    .Include(od => od.Food)
                        .ThenInclude(f => f!.Images)
                    .Include(od => od.Combo)
                        .ThenInclude(c => c!.Images)
                    .ToListAsync();

                // Thống kê Food bán chạy
                var foodStats = orderDetails
                    .Where(od => od.FoodId.HasValue && od.Food != null)
                    .GroupBy(od => new { od.FoodId, od.Food })
                    .Select(g => new
                    {
                        FoodId = g.Key.FoodId!.Value,
                        Food = g.Key.Food!,
                        SoldQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(x => x.SoldQuantity)
                    .Take(request.Limit)
                    .ToList();

                // Thống kê Combo bán chạy
                var comboStats = orderDetails
                    .Where(od => od.ComboId.HasValue && od.Combo != null)
                    .GroupBy(od => new { od.ComboId, od.Combo })
                    .Select(g => new
                    {
                        ComboId = g.Key.ComboId!.Value,
                        Combo = g.Key.Combo!,
                        SoldQuantity = g.Sum(od => od.Quantity)
                    })
                    .OrderByDescending(x => x.SoldQuantity)
                    .Take(request.Limit)
                    .ToList();

                // Chuyển đổi sang DTO
                var topFoods = foodStats.Select(fs => new TopProductDto
                {
                    Id = fs.FoodId,
                    Name = fs.Food.FoodName ?? string.Empty,
                    Description = fs.Food.Description,
                    Price = fs.Food.Price,
                    ImageUrl = fs.Food.Images?.Url,
                    SoldQuantity = fs.SoldQuantity,
                    ProductType = "Food"
                }).ToList();

                var topCombos = comboStats.Select(cs => new TopProductDto
                {
                    Id = cs.ComboId,
                    Name = cs.Combo.ComboName ?? string.Empty,
                    Description = cs.Combo.Description,
                    Price = cs.Combo.Price,
                    ImageUrl = cs.Combo.Images?.Url,
                    SoldQuantity = cs.SoldQuantity,
                    ProductType = "Combo"
                }).ToList();

                // Tính tổng số lượng bán
                var totalFoodsSold = orderDetails.Where(od => od.FoodId.HasValue).Sum(od => od.Quantity);
                var totalCombosSold = orderDetails.Where(od => od.ComboId.HasValue).Sum(od => od.Quantity);

                return new TopProductsResponseDto
                {
                    TopFoods = topFoods,
                    TopCombos = topCombos,
                    PeriodStart = startDate,
                    PeriodEnd = endDate,
                    TotalFoodsSold = totalFoodsSold,
                    TotalCombosSold = totalCombosSold
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu sản phẩm bán chạy: {ex.Message}", ex);
            }
        }

        public async Task<DashboardSummaryDto> GetDashboardSummaryAsync(TopProductsRequestDto request)
        {
            try
            {
                // Mặc định lấy dữ liệu tháng hiện tại
                var startDate = request.StartDate?.Date ?? new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var endDate = request.EndDate?.Date ?? startDate.AddMonths(1).AddDays(-1);

                // Query tất cả đơn hàng trong khoảng thời gian
                var allOrdersQuery = _unitOfWork.Orders.GetQueryable()
                    .Where(o => o.CreatedAt.Date >= startDate && o.CreatedAt.Date <= endDate);

                var allOrders = await allOrdersQuery.ToListAsync();

                // Query đơn hàng đã hoàn thành
                var completedOrders = allOrders.Where(o => o.Status == OrderStatus.Completed && o.PaymentStatus == PaymentStatus.Paid).ToList();

                // Thống kê cơ bản
                var totalRevenue = completedOrders.Sum(o => o.SubtotalAmount - o.VoucherDiscountAmount);
                var totalOrders = allOrders.Count;

                // Thống kê đầy đủ tất cả trạng thái đơn hàng
                var pendingOrders = allOrders.Count(o => o.Status == OrderStatus.Pending);
                var acceptedOrders = allOrders.Count(o => o.Status == OrderStatus.Accepted);
                var processingOrders = allOrders.Count(o => o.Status == OrderStatus.Processing);
                var doneOrders = allOrders.Count(o => o.Status == OrderStatus.Done);
                var shippingOrders = allOrders.Count(o => o.Status == OrderStatus.Shipping);
                var completedOrdersCount = allOrders.Count(o => o.Status == OrderStatus.Completed);
                var cancelledOrders = allOrders.Count(o => o.Status == OrderStatus.Cancelled);

                var averageOrderValue = completedOrdersCount > 0 ? totalRevenue / completedOrdersCount : 0;

                // Lấy tất cả sản phẩm để đếm tổng số
                var totalFoods = await _unitOfWork.Foods.GetQueryable().CountAsync(f => f.Status);
                var totalCombos = await _unitOfWork.Combos.GetQueryable().CountAsync(c => c.Status);
                var totalProducts = totalFoods + totalCombos;

                // Lấy top sản phẩm bán chạy (kết hợp Food và Combo)
                var topProductsData = await GetTopProductsAsync(new TopProductsRequestDto
                {
                    Limit = 5,
                    StartDate = startDate,
                    EndDate = endDate
                });

                // Kết hợp top foods và combos, sắp xếp theo số lượng bán
                var allTopProducts = new List<TopProductDto>();
                allTopProducts.AddRange(topProductsData.TopFoods);
                allTopProducts.AddRange(topProductsData.TopCombos);

                var topProducts = allTopProducts
                    .OrderByDescending(p => p.SoldQuantity)
                    .Take(5)
                    .ToList();

                return new DashboardSummaryDto
                {
                    TotalRevenue = totalRevenue,
                    TotalOrders = totalOrders,
                    TotalProducts = totalProducts,
                    PendingOrders = pendingOrders,
                    AcceptedOrders = acceptedOrders,
                    ProcessingOrders = processingOrders,
                    DoneOrders = doneOrders,
                    ShippingOrders = shippingOrders,
                    CompletedOrders = completedOrdersCount,
                    CancelledOrders = cancelledOrders,
                    AverageOrderValue = averageOrderValue,
                    TopProducts = topProducts,
                    PeriodStart = startDate,
                    PeriodEnd = endDate
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy dữ liệu dashboard: {ex.Message}", ex);
            }
        }
    }
}