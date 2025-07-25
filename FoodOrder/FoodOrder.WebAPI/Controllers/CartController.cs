using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            return Ok(cart);
        }


        [HttpPost]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest dto)
        {
            try
            {
                var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                    return Unauthorized("User ID không hợp lệ");

                if (dto.FoodId == null && dto.ComboId == null)
                    return BadRequest("Phải chọn một loại sản phẩm.");

                await _cartService.AddToCartAsync(userId, dto.FoodId, dto.ComboId, dto.Quantity);

                return Ok(new { message = "Đã thêm vào giỏ hàng" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi thêm vào giỏ hàng" });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCartItem([FromBody] UpdateCartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIds))
                return Unauthorized("User ID không hợp lệ");

            try
            {
                await _cartService.UpdateCartItemAsync(userIds, dto.CartItemId, dto.Quantity);
                return Ok(new { message = "Cập nhật thành công" });
            }
            catch (InvalidOperationException ex)
            {
                // Lỗi nghiệp vụ => báo về cho frontend hiển thị
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                // Lỗi hệ thống khác
                return StatusCode(500, new { message = "Có lỗi xảy ra", detail = ex.Message });
            }
        }



        [HttpDelete]
        public async Task<IActionResult> RemoveCartItem([FromBody] RemoveCartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIds))
                return Unauthorized("User ID không hợp lệ");
            await _cartService.RemoveCartItemAsync(userIds, dto.CartItemId);
            return Ok(new { message = "Xóa thành công" });
        }


    }
}
