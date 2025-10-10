using FoodOrder.Application.DTOs.Dashboard;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace FoodOrder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Chỉ Admin mới có quyền truy cập dashboard
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Lấy top sản phẩm bán chạy nhất
        /// </summary>
        /// <param name="limit">Số lượng sản phẩm trả về (mặc định 5)</param>
        /// <param name="startDate">Ngày bắt đầu (mặc định đầu tháng hiện tại)</param>
        /// <param name="endDate">Ngày kết thúc (mặc định cuối tháng hiện tại)</param>
        /// <returns>Top sản phẩm bán chạy</returns>
        [HttpGet("top-products")]
        public async Task<ActionResult<TopProductsResponseDto>> GetTopProducts(
            [FromQuery] int limit = 5,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var request = new TopProductsRequestDto
                {
                    Limit = Math.Max(1, Math.Min(limit, 20)), // Giới hạn từ 1-20
                    StartDate = startDate,
                    EndDate = endDate
                };

                var result = await _dashboardService.GetTopProductsAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy top 5 food bán chạy nhất tháng hiện tại
        /// </summary>
        /// <returns>Top 5 food bán chạy</returns>
        [HttpGet("top-foods")]
        public async Task<ActionResult<TopProductsResponseDto>> GetTopFoods()
        {
            try
            {
                var request = new TopProductsRequestDto
                {
                    Limit = 5
                };

                var result = await _dashboardService.GetTopProductsAsync(request);

                // Chỉ trả về foods
                var response = new TopProductsResponseDto
                {
                    TopFoods = result.TopFoods,
                    TopCombos = new(), // Empty list
                    PeriodStart = result.PeriodStart,
                    PeriodEnd = result.PeriodEnd,
                    TotalFoodsSold = result.TotalFoodsSold,
                    TotalCombosSold = 0
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy top 5 combo bán chạy nhất tháng hiện tại
        /// </summary>
        /// <returns>Top 5 combo bán chạy</returns>
        [HttpGet("top-combos")]
        public async Task<ActionResult<TopProductsResponseDto>> GetTopCombos()
        {
            try
            {
                var request = new TopProductsRequestDto
                {
                    Limit = 5
                };

                var result = await _dashboardService.GetTopProductsAsync(request);

                // Chỉ trả về combos
                var response = new TopProductsResponseDto
                {
                    TopFoods = new(), // Empty list
                    TopCombos = result.TopCombos,
                    PeriodStart = result.PeriodStart,
                    PeriodEnd = result.PeriodEnd,
                    TotalFoodsSold = 0,
                    TotalCombosSold = result.TotalCombosSold
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        /// <summary>
        /// Lấy tổng quan dashboard tháng hiện tại
        /// </summary>
        /// <param name="startDate">Ngày bắt đầu (mặc định đầu tháng hiện tại)</param>
        /// <param name="endDate">Ngày kết thúc (mặc định cuối tháng hiện tại)</param>
        /// <returns>Tổng quan dashboard</returns>
        [HttpGet("summary")]
        public async Task<ActionResult<DashboardSummaryDto>> GetDashboardSummary(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var request = new TopProductsRequestDto
                {
                    Limit = 5,
                    StartDate = startDate,
                    EndDate = endDate
                };

                var result = await _dashboardService.GetDashboardSummaryAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}