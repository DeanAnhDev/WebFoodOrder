using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboDetailController : ControllerBase
    {
        private readonly IComboDetailServices _comboDetailServices;

        public ComboDetailController(IComboDetailServices comboDetailServices)
        {
            _comboDetailServices = comboDetailServices;
        }

        [HttpPost]
        public async Task<IActionResult> CreateComboDeatail([FromBody] ComboDetailDto comboDetailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isCreated = await _comboDetailServices.AddAsync(comboDetailDto);

                if (!isCreated)
                {
                    return BadRequest(new { message = "Thêm món ăn vào combo thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Thêm món ăn vào combo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCombo([FromBody] ComboDetailDto comboDetailDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool isUpdated = await _comboDetailServices.UpdateAsync(comboDetailDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Cập nhật món ăn vào combo thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Cập nhật món ăn vào combo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpDelete("{comboId:int}/{foodId:int}")]
        public async Task<IActionResult> DeleteFood(int comboId, int foodId)
        {
            try
            {
                if (comboId <= 0 && foodId <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var existingComboDetail = await _comboDetailServices.GetByIdAsync(comboId, foodId);
                if (existingComboDetail == null)
                {
                    return NotFound(new { message = $"Không tìm thấy combo detail với comboID {comboId} foodID {foodId}." });
                }

                bool isDelete = await _comboDetailServices.DeleteAsync(comboId, foodId);
                if (!isDelete)
                {
                    return BadRequest(new { message = "Xóa combo detail thất bại. Vui lòng thử lại!" });
                }
                return Ok(new { message = "Xóa combo detail thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }



        [HttpGet("{comboId:int}/{foodId:int}")]
        public async Task<IActionResult> GetComboDetaiByComboIdAndFoodId(int comboId, int foodId)
        {
            try
            {
                var existingComboDetail = await _comboDetailServices.GetByIdAsync(comboId, foodId);

                if (existingComboDetail == null)
                {
                    return NotFound(new { message = "Không tìm thấy combo detail nào." });
                }

                return Ok(existingComboDetail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("get-combo-details/{comboId:int}")]
        public async Task<IActionResult> GetComboDetaisByComboId(int comboId)
        {
            try
            {
                var existingComboDetails = await _comboDetailServices.GetComboDetailsByComboIdAsync(comboId);

                if (existingComboDetails == null)
                {
                    return NotFound(new { message = "Không tìm thấy combo detail nào." });
                }

                return Ok(existingComboDetails);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }








    }
}
