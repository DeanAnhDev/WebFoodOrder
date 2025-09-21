using FoodOrder.Application.DTOs.Identity.Location;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LocationController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [HttpGet("userId")]
        public async Task<IActionResult> GetAllByUserId()
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");

            var locations = await _locationService.GetAllByIdUserAsync(userId);
            return Ok(locations);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var location = await _locationService.GetByIdAsync(id);
            if (location == null)
                return NotFound();
            return Ok(location);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateLocationDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");
            dto.UserId = userId;
            var result = await _locationService.AddAsync(dto);
            if (!result)
                return BadRequest(new { message = "Thêm địa chỉ thất bại" });

            return Ok(new { message = "Thêm địa chỉ thành công" });
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateLocationDto dto)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");
            dto.UserId = userId;
            var result = await _locationService.UpdateAsync(dto);
            if (!result)
                return BadRequest(new { message = "Cập nhật địa chỉ thất bại" });

            return Ok(new { message = "Cập nhật địa chỉ thành công" });
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateIsDefault([FromQuery]int id, [FromQuery] bool isDefault)
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
                return Unauthorized("User ID không hợp lệ");
            var user = userId;
            var result = await _locationService.UpdateIsDefault(user,id, isDefault);
            if (!result)
                return BadRequest(new { message = "Cập nhật địa chỉ thất bại" });

            return Ok(new { message = "Cập nhật địa chỉ thành công" });
        }


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _locationService.DeleteAsync(id);
            if (!result)
                return BadRequest(new { message = "Xoá địa chỉ thất bại" });

            return Ok(new { message = "Xoá địa chỉ thành công" });
        }
    }
}
