
using FoodOrder.Application.DTOs.Voucher;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VoucherController : ControllerBase
    {
        private readonly IVoucherServices _voucherServices;

        public VoucherController(IVoucherServices voucherServices)
        {
            _voucherServices = voucherServices;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetVoucherById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var voucher = await _voucherServices.GetByIdAsync(id);

                if (voucher == null)
                {
                    return NotFound(new { message = "Không tìm thấy voucher nào." });
                }

                return Ok(voucher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFoods([FromQuery] VoucherQuery query)
        {
            try
            {
                var result = await _voucherServices.GetAllVoucherAsync(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateVoucher([FromBody] VoucherCreateDto voucherDto)
        {
            try
            {
                bool isCreated = await _voucherServices.AddAsync(voucherDto);

                if (!isCreated)
                {
                    return BadRequest(new { message = "Tạo voucher thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Tạo voucher thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateVoucher( [FromBody] UpdateVoucherDto voucherDto)
        {
            try
            {

                bool isUpdated = await _voucherServices.UpdateAsync(voucherDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Cập nhật voucher thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Cập nhật voucher thành công!" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVoucher(int id)
        {
            try
            {
                bool isDeleted = await _voucherServices.DeleteAsync(id);

                if (!isDeleted)
                {
                    return NotFound(new { message = "Xóa voucher thất bại hoặc không tồn tại" });
                }

                return Ok(new { message = "Xóa voucher thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

    }
}
