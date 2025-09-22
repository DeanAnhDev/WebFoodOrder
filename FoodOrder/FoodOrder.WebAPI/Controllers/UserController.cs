using FoodOrder.Application.DTOs.Identity;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var user = await _userService.GetByIdAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy user nào." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                {
                    return Unauthorized(new { message = "User ID không hợp lệ!" });
                }

                var user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng." });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [Authorize]
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out var id))
                {
                    return Unauthorized(new { message = "User ID không hợp lệ!" });
                }

                if (string.IsNullOrWhiteSpace(updateUserDto.FullName))
                {
                    return BadRequest(new { message = "Tên mới không được để trống." });
                }

                var success = await _userService.UpdateAsync(id, updateUserDto);
                if (!success)
                {
                    return NotFound(new { message = "Không tìm thấy người dùng." });
                }

                return Ok(new { message = "Cập nhật tên thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] UserPasswordUpdateDto dto)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdStr, out var userId))
                return Unauthorized(new { message = "User ID không hợp lệ" });

            try
            {
                await _userService.ChangePasswordAsync(userId, dto);
                return Ok(new { message = "Đổi mật khẩu thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch
            {
                return StatusCode(500, new { message = "Lỗi server!" });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("customers")]
        public async Task<IActionResult> GetCustomers([FromQuery] GetCustomersRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0 || request.PageSize > 100)
                    request.PageSize = 10;

                var result = await _userService.GetCustomersAsync(request);

                return Ok(new
                {
                    message = "Lấy danh sách khách hàng thành công",
                    data = result.Items,
                    totalCount = result.TotalCount,
                    pageNumber = result.PageNumber,
                    pageSize = result.PageSize,
                    totalPages = result.TotalPages,
                    hasNextPage = result.HasNextPage,
                    hasPreviousPage = result.HasPreviousPage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }
    }


}

