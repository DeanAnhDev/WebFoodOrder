using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComboController : ControllerBase
    {
        private readonly IComboServices _comboServices;
        public ComboController(IComboServices comboServices)
        {
            _comboServices = comboServices;
        }

        //[Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<IActionResult> GetAllCombos([FromQuery] PagedQuery query)
        {
            try
            {
                var result = await _comboServices.GetPagedCombosAsync(query);

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("Combos")]
        public async Task<IActionResult> GetAllCombosStatusTrue()
        {
            try
            {
                var result = await _comboServices.GetAllComboAsync();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }




        [HttpGet("{id}")]
        public async Task<IActionResult> GetComboById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var combo = await _comboServices.GetByIdAsync(id);

                if (combo == null)
                {
                    return NotFound(new { message = "Không tìm thấy combo nào." });
                }

                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetComboBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequest(new { message = "Slug không hợp lệ!" });
                }

                var combo = await _comboServices.GetBySlugAsync(slug);

                if (combo == null)
                {
                    return NotFound(new { message = "Không tìm thấy combo nào." });
                }

                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


        [HttpGet("get-combo-with-food/slug/{slug}")]
        public async Task<IActionResult> GetComboWithFoodsBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequest(new { message = "Slug không hợp lệ!" });
                }

                var combo = await _comboServices.GetComboWithFoodsBySlugAsync(slug);

                if (combo == null)
                {
                    return NotFound(new { message = "Không tìm thấy combo nào." });
                }

                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CreateCombo([FromBody] ComboDtoCreate comboDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isCreated = await _comboServices.AddAsync(comboDto);

                if (!isCreated)
                {
                    return BadRequest(new { message = "Tạo combo thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Tạo combo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateCombo([FromBody] ComboDtoUpdate comboDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool isUpdated = await _comboServices.UpdateAsync(comboDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Cập nhật combo thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Cập nhật combo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCombo(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var existingCombo= await _comboServices.GetByIdAsync(id);
                if (existingCombo == null)
                {
                    return NotFound(new { message = $"Không tìm thấy combo với ID {id}." });
                }

                bool isDelete = await _comboServices.DeleteAsync(id);
                if (!isDelete)
                {
                    return BadRequest(new { message = "Xóa combo thất bại. Vui lòng thử lại!" });
                }
                return Ok(new { message = "Xóa combo thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("foods")]
        public async Task<IActionResult> GetAvailableFoodsForCombo()
        {

            var foods = await _comboServices.GetFoodsNotInComboAsync();
            return Ok(foods);
        }


    }
}
