using FoodOrder.Application.DTOs.Dashboard;
using System.Threading.Tasks;

namespace FoodOrder.Application.Interfaces
{
    public interface IDashboardService
    {
        Task<TopProductsResponseDto> GetTopProductsAsync(TopProductsRequestDto request);
        Task<DashboardSummaryDto> GetDashboardSummaryAsync(TopProductsRequestDto request);
    }
}