using FoodOrder.Application.DTOs.Ahamove;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AhamoveController : ControllerBase
    {
        private readonly IAhamoveService _ahamoveService;

        public AhamoveController(IAhamoveService ahamoveService)
        {
            _ahamoveService = ahamoveService;
        }

        [HttpPost("estimate-shipping-fee")]
        public async Task<IActionResult> EstimateShippingFee([FromBody] EstimateShippingFeeRequestDto request)
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

                var result = await _ahamoveService.EstimateShippingFeeAsync(request);

                if (!result.Success)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = result.Message
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = result.Message,
                    data = new
                    {
                        fee = result.Fee,
                        currency = result.Currency,
                        formattedFee = result.Fee.ToString("N0") + " " + result.Currency
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tính phí giao hàng",
                    error = ex.Message
                });
            }
        }

        [HttpPost("get-coordinates")]
        public async Task<IActionResult> GetCoordinates([FromBody] GetCoordinatesRequestDto request)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Address))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Địa chỉ không được để trống"
                    });
                }

                var result = await _ahamoveService.GetCoordinateFromAddressAsync(request.Address);

                if (result == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "Không tìm thấy tọa độ cho địa chỉ này"
                    });
                }

                return Ok(new
                {
                    success = true,
                    message = "Lấy tọa độ thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy tọa độ",
                    error = ex.Message
                });
            }
        }
    }

    public class GetCoordinatesRequestDto
    {
        public string Address { get; set; } = string.Empty;
    }
}