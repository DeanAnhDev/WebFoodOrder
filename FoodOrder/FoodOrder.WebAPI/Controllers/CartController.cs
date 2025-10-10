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

        #region tại quầy
        [AllowAnonymous]
        [HttpPost("temporary")]
        public async Task<IActionResult> CreateTemporaryCart()
        {
            try
            {
                var cart = await _cartService.CreateTemporaryCartAsync();
                return Ok(cart);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo giỏ hàng tạm thời", detail = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("assign/{cartId}")]
        public async Task<IActionResult> AssignCartToUser(int cartId, [FromQuery] int userId)
        {
            try
            {

                var result = await _cartService.AssignCartToUserAsync(cartId, userId);
                if (result)
                {
                    return Ok(new { message = "Giỏ hàng đã được gán cho người dùng thành công" });
                }
                else
                {
                    return BadRequest(new { message = "Không thể gán giỏ hàng cho người dùng. Giỏ hàng không tồn tại hoặc không phải là giỏ hàng tạm thời, hoặc người dùng đã có giỏ hàng." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi gán giỏ hàng cho người dùng", detail = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("temporary")]
        public async Task<IActionResult> GetAllTemporaryCarts()
        {
            try
            {
                var carts = await _cartService.GetAllTemporaryCartsBasicAsync();
                return Ok(carts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy danh sách giỏ hàng tạm thời", detail = ex.Message });
            }
        }

        #endregion
        #region
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetCart()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");

            var cart = await _cartService.GetOrCreateCartAsync(userId);
            return Ok(cart);
        }

        [Authorize]
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

        #endregion



        /// <summary>
        /// Thêm food hoặc combo vào giỏ hàng theo cartId
        /// </summary>
        /// <param name="dto">Thông tin sản phẩm cần thêm vào cart</param>
        /// <returns>Kết quả thêm sản phẩm vào cart</returns>
        [AllowAnonymous]
        [HttpPost("add-by-id")]
        public async Task<IActionResult> AddToCartById([FromBody] AddToCartByIdRequest dto)
        {
            try
            {
                if (dto.FoodId == null && dto.ComboId == null)
                    return BadRequest(new { message = "Phải chọn một loại sản phẩm (FoodId hoặc ComboId)." });

                if (dto.FoodId != null && dto.ComboId != null)
                    return BadRequest(new { message = "Chỉ được chọn một loại sản phẩm (Food hoặc Combo)." });

                if (dto.Quantity <= 0)
                    return BadRequest(new { message = "Số lượng phải lớn hơn 0." });

                await _cartService.AddToCartByIdAsync(dto.CartId, dto.FoodId, dto.ComboId, dto.Quantity);

                return Ok(new
                {
                    success = true,
                    message = "Đã thêm sản phẩm vào giỏ hàng thành công",
                    data = new
                    {
                        cartId = dto.CartId,
                        foodId = dto.FoodId,
                        comboId = dto.ComboId,
                        quantity = dto.Quantity
                    }
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
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi thêm sản phẩm vào giỏ hàng",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Cập nhật số lượng food hoặc combo trong giỏ hàng theo cartId
        /// </summary>
        /// <param name="dto">Thông tin cập nhật số lượng sản phẩm</param>
        /// <returns>Kết quả cập nhật</returns>
        [AllowAnonymous]
        [HttpPut("update-by-id")]
        public async Task<IActionResult> UpdateCartItemById([FromBody] UpdateCartItemByIdRequest dto)
        {
            try
            {
                if (dto.FoodId == null && dto.ComboId == null)
                    return BadRequest(new { message = "Phải chọn một loại sản phẩm (FoodId hoặc ComboId)." });

                if (dto.FoodId != null && dto.ComboId != null)
                    return BadRequest(new { message = "Chỉ được chọn một loại sản phẩm (Food hoặc Combo)." });

                await _cartService.UpdateCartItemByIdAsync(dto.CartId, dto.FoodId, dto.ComboId, dto.Quantity);

                if (dto.Quantity <= 0)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Đã xóa sản phẩm khỏi giỏ hàng",
                        data = new
                        {
                            cartId = dto.CartId,
                            foodId = dto.FoodId,
                            comboId = dto.ComboId,
                            quantity = 0
                        }
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Đã cập nhật số lượng sản phẩm thành công",
                    data = new
                    {
                        cartId = dto.CartId,
                        foodId = dto.FoodId,
                        comboId = dto.ComboId,
                        quantity = dto.Quantity
                    }
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
            catch (ArgumentException ex)
            {
                return NotFound(new
                {
                    success = false,
                    message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi cập nhật sản phẩm",
                    detail = ex.Message
                });
            }
        }
        #region

        [Authorize]
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


        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> RemoveCartItem([FromBody] RemoveCartItemDto dto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var userIds))
                return Unauthorized("User ID không hợp lệ");
            await _cartService.RemoveCartItemAsync(userIds, dto.CartItemId);
            return Ok(new { message = "Xóa thành công" });
        }

        #endregion
        /// <summary>
        /// Lấy cart theo ID bao gồm đầy đủ thông tin food, combo, hình ảnh và promotion
        /// </summary>
        /// <param name="cartId">ID của cart cần lấy</param>
        /// <returns>Thông tin đầy đủ của cart</returns>
        [HttpGet("{cartId}")]
        public async Task<IActionResult> GetCartById(int cartId)
        {
            try
            {
                var cartResponse = await _cartService.GetCartByIdAsync(cartId);

                if (cartResponse == null)
                {
                    return NotFound(new { message = "Không tìm thấy cart với ID này" });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy thông tin cart thành công",
                    data = cartResponse
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy thông tin cart",
                    detail = ex.Message
                });
            }
        }

        /// <summary>
        /// Xóa cart theo ID
        /// </summary>
        /// <param name="cartId">ID của cart cần xóa</param>
        /// <returns>Kết quả xóa cart</returns>
        [Authorize]
        [HttpDelete("{cartId}")]
        public async Task<IActionResult> DeleteCartById(int cartId)
        {
            try
            {
                var result = await _cartService.DeleteCartByIdAsync(cartId);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa cart thành công"
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy cart với ID này"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi xóa cart",
                    detail = ex.Message
                });
            }
        }

        [AllowAnonymous]
        [HttpDelete("delete-item")]
        public async Task<IActionResult> DeleteCartItem([FromBody] DeleteCartItemRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Dữ liệu không hợp lệ",
                        errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)
                    });
                }

                var result = await _cartService.DeleteCartItemByIdAsync(request.CartId, request.CartItemId);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Xóa sản phẩm khỏi giỏ hàng thành công",
                        data = new
                        {
                            cartId = request.CartId,
                            cartItemId = request.CartItemId
                        }
                    });
                }
                else
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy cart hoặc cart item với ID đã cung cấp"
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi xóa sản phẩm khỏi giỏ hàng",
                    detail = ex.Message
                });
            }
        }

    }
}
