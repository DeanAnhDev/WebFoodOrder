
using FoodOrder.Infrastructure.Services.GoongServices;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GoongController : ControllerBase
    {
        private readonly IGoongService _goongService;

        public GoongController(IGoongService goongService)
        {
            _goongService = goongService;
        }

        [HttpPost("inner-city")]
        public async Task<IActionResult> CheckHanoi([FromBody] LocationDto? dto)
        {
            try
            {
                if (dto == null)
                {
                    return BadRequest(new
                    {
                        message = "Tọa độ không hợp lệ"
                    });
                }

                var result = await _goongService.CheckHanoiAsync(dto.Lat, dto.Lng);

                if (result == null)
                {
                    return StatusCode(500, new
                    {
                        message = "Không gọi được Goong API"
                    });
                }

                return Ok(new
                {
                    isInInnerCity = result.IsInInnerCity,
                    address = result.Address
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Lỗi server khi xử lý dữ liệu",
                    error = ex.Message
                });
            }
        }
    }

    public class LocationDto
    {
        public string? Lat { get; set; }
        public string? Lng { get; set; }
    }
}
