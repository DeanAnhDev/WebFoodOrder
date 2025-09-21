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
