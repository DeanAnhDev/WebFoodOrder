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

        #region

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

        #endregion



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

        [Authorize(Roles = "Admin")]
        [HttpGet("staff")]
        public async Task<IActionResult> GetStaff([FromQuery] GetStaffRequestDto request)
        {
            try
            {
                if (request.PageNumber <= 0)
                    request.PageNumber = 1;

                if (request.PageSize <= 0 || request.PageSize > 100)
                    request.PageSize = 10;

                var result = await _userService.GetStaffAsync(request);

                return Ok(new
                {
                    message = "Lấy danh sách nhân viên thành công",
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

        [Authorize(Roles = "Admin")]
        [HttpPost("staff")]
        public async Task<IActionResult> CreateStaff([FromBody] CreateStaffDto createStaffDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
                }

                var result = await _userService.CreateStaffAsync(createStaffDto);

                return CreatedAtAction(
                    nameof(GetUserById),
                    new { id = result.Id },
                    new
                    {
                        message = "Tạo nhân viên thành công",
                        data = result
                    });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("staff/{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] UpdateStaffDto updateStaffDto)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => new { Field = x.Key, Errors = x.Value?.Errors.Select(e => e.ErrorMessage) })
                        .ToList();

                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
                }

                var result = await _userService.UpdateStaffAsync(id, updateStaffDto);

                return Ok(new
                {
                    message = "Cập nhật nhân viên thành công",
                    data = result
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("staff/{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var success = await _userService.DeleteStaffAsync(id);

                if (success)
                {
                    return Ok(new { message = "Xóa nhân viên thành công" });
                }

                return BadRequest(new { message = "Không thể xóa nhân viên" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPost("customers")]
        public async Task<IActionResult> CreateCustomer([FromBody] CreateCustomerDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    return BadRequest(new { message = "Dữ liệu không hợp lệ", errors });
                }

                var customer = await _userService.CreateCustomerAsync(dto);
                return Ok(new
                {
                    message = "Tạo khách hàng thành công",
                    data = customer
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

    }

}

