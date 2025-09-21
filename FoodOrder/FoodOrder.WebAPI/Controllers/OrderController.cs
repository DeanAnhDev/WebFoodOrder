using FoodOrder.Application.DTOs.Orders;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Tạo đơn hàng mới từ giỏ hàng
        /// </summary>
        /// <param name="createOrderDto">Thông tin đơn hàng (LocationId = null cho bán tại quầy, có LocationId sẽ tự động tính phí ship qua Ahamove)</param>
        /// <returns>Thông tin đơn hàng đã tạo bao gồm phí giao hàng</returns>
        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Lấy userId từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Không thể xác định người dùng");
                }

                var order = await _orderService.CreateOrderAsync(createOrderDto, userId);
                return Ok(new
                {
                    success = true,
                    message = "Đặt hàng thành công",
                    data = order
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tạo đơn hàng",
                    error = ex.Message
                });
            }
        }
    }
}
