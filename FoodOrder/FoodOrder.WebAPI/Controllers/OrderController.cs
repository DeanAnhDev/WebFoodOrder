using FoodOrder.Application.DTOs.Orders;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Orders;
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
        /// Lấy thống kê số lượng đơn hàng theo từng trạng thái
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng (tìm kiếm)</param>
        /// <param name="userId">ID người dùng</param>
        /// <param name="paymentStatus">Trạng thái thanh toán</param>
        /// <returns>Thống kê số lượng đơn hàng theo từng status</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<OrderStatisticsDto>> GetOrderStatistics(
            [FromQuery] string? orderCode = null,
            [FromQuery] int? userId = null,
            [FromQuery] PaymentStatus? paymentStatus = null)
        {
            try
            {
                var result = await _orderService.GetOrderStatisticsAsync(orderCode, userId, paymentStatus);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order statistics: {ex.Message}");
                return StatusCode(500, "Có lỗi xảy ra khi lấy thống kê đơn hàng");
            }
        }

        /// <summary>
        /// Lấy danh sách đơn hàng với các filter và phân trang
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng (tìm kiếm)</param>
        /// <param name="userId">ID người dùng</param>
        /// <param name="status">Trạng thái đơn hàng</param>
        /// <param name="paymentStatus">Trạng thái thanh toán</param>
        /// <param name="page">Trang hiện tại (default: 1)</param>
        /// <param name="pageSize">Số lượng item mỗi trang (default: 10)</param>
        /// <param name="sortBy">Trường sắp xếp (default: CreatedAt)</param>
        /// <param name="sortOrder">Thứ tự sắp xếp: asc/desc (default: desc)</param>
        /// <returns>Danh sách đơn hàng với thông tin phân trang</returns>
        [HttpGet]
        public async Task<ActionResult<GetOrdersResponseDto>> GetOrders(
            [FromQuery] string? orderCode = null,
            [FromQuery] int? userId = null,
            [FromQuery] OrderStatus? status = null,
            [FromQuery] PaymentStatus? paymentStatus = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = "CreatedAt",
            [FromQuery] string? sortOrder = "desc")
        {
            try
            {
                var request = new GetOrdersRequestDto
                {
                    OrderCode = orderCode,
                    UserId = userId,
                    Status = status,
                    PaymentStatus = paymentStatus,
                    Page = page > 0 ? page : 1,
                    PageSize = pageSize > 0 && pageSize <= 100 ? pageSize : 10, // Giới hạn tối đa 100 items
                    SortBy = sortBy,
                    SortOrder = sortOrder
                };

                var result = await _orderService.GetAllOrdersAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting orders: {ex.Message}");
                return StatusCode(500, "Có lỗi xảy ra khi lấy danh sách đơn hàng");
            }
        }

        /// <summary>
        /// Lấy thông tin chi tiết đơn hàng theo ID
        /// </summary>
        /// <param name="id">ID của đơn hàng</param>
        /// <returns>Thông tin chi tiết đơn hàng</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }

                return Ok(order);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting order by id: {ex.Message}");
                return StatusCode(500, "Có lỗi xảy ra khi lấy thông tin đơn hàng");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái đơn hàng
        /// </summary>
        /// <param name="request">Thông tin cập nhật (OrderId, NewStatus, Reason)</param>
        /// <returns>Kết quả cập nhật và thông tin đơn hàng đã cập nhật</returns>
        [HttpPut("status")]
        public async Task<ActionResult<UpdateOrderStatusResponseDto>> UpdateOrderStatus([FromBody] UpdateOrderStatusDto request)
        {
            try
            {
                var result = await _orderService.UpdateOrderStatusAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return StatusCode(500, new UpdateOrderStatusResponseDto
                {
                    Success = false,
                    Message = "Có lỗi xảy ra khi cập nhật trạng thái đơn hàng"
                });
            }
        }

        /// <summary>
        /// Tạo đơn hàng mới từ giỏ hàng
        /// </summary>
        /// <param name="createOrderDto">Thông tin đơn hàng (LocationId = null cho bán tại quầy, có LocationId sẽ tự động tính phí ship qua Ahamove. PaymentMethod = BankTransfer sẽ tạo VNPay URL)</param>
        /// <returns>Thông tin đơn hàng đã tạo bao gồm phí giao hàng và payment URL (nếu có)</returns>
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponseDto>> CreateOrder([FromBody] CreateOrderDto createOrderDto)
        {
            try
            {
                // Lấy userId từ claims
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized("Không thể xác định người dùng");
                }

                var result = await _orderService.CreateOrderAsync(createOrderDto, userId);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new CreateOrderResponseDto
                {
                    Order = null!,
                    PaymentUrl = null,
                    Message = ex.Message,
                    Success = false
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new CreateOrderResponseDto
                {
                    Order = null!,
                    PaymentUrl = null,
                    Message = ex.Message,
                    Success = false
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, new CreateOrderResponseDto
                {
                    Order = null!,
                    PaymentUrl = null,
                    Message = "Có lỗi xảy ra khi tạo đơn hàng",
                    Success = false
                });
            }
        }

        /// <summary>
        /// Xử lý callback từ VNPay sau khi thanh toán
        /// </summary>
        /// <param name="orderCode">Mã đơn hàng</param>
        /// <returns>Kết quả xử lý payment</returns>
        [HttpGet("payment-callback")]
        [AllowAnonymous] // VNPay callback không cần auth
        public async Task<IActionResult> PaymentCallback([FromQuery] string vnp_TxnRef, [FromQuery] string vnp_ResponseCode, [FromQuery] string vnp_TransactionStatus)
        {
            try
            {
                // Lấy toàn bộ query string để validate
                var queryString = Request.QueryString.Value ?? "";
                if (queryString.StartsWith("?"))
                    queryString = queryString[1..]; // Remove leading ?

                var orderCode = vnp_TxnRef;

                // Chỉ xử lý nếu giao dịch thành công
                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                {
                    var result = await _orderService.ProcessPaymentCallbackAsync(orderCode, queryString);

                    if (result)
                    {
                        // Redirect đến trang thành công (có thể config trong appsettings)
                        return Redirect($"/payment-success?orderCode={orderCode}");
                    }
                    else
                    {
                        return Redirect($"/payment-failed?orderCode={orderCode}&reason=processing_failed");
                    }
                }
                else
                {
                    return Redirect($"/payment-failed?orderCode={orderCode}&reason=payment_failed");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Payment callback error: {ex.Message}");
                return Redirect("/payment-failed?reason=system_error");
            }
        }
    }
}
