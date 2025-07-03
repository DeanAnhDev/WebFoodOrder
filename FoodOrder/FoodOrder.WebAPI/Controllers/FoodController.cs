using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodController : Controller
    {
        private readonly IFoodServices _foodService;
        public FoodController(IFoodServices foodService)
        {
            _foodService = foodService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFoods([FromQuery] PagedQuery query)
        {
            try
            {
                var result = await _foodService.GetPagedFoodsAsync(query);

                if (result == null || !result.Items.Any())
                {
                    return NotFound(new { message = "Không tìm thấy món ăn nào." });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var food = await _foodService.GetByIdAsync(id);

                if (food == null)
                {
                    return NotFound(new { message = "Không tìm thấy món ăn nào." });
                }

                return Ok(food);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetFoodBySlug(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                {
                    return BadRequest(new { message = "Slug không hợp lệ!" });
                }

                var combo = await _foodService.GetBySlugAsync(slug);

                if (combo == null)
                {
                    return NotFound(new { message = "Không tìm thấy food nào." });
                }

                return Ok(combo);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFood([FromBody] FoodDtoCreate foodDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                bool isCreated = await _foodService.AddAsync(foodDto);

                if (!isCreated)
                {
                    return BadRequest(new { message = "Tạo món ăn thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Tạo món ăn thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFood([FromBody] FoodDtoUpdate foodDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool isUpdated = await _foodService.UpdateAsync(foodDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Cập nhật món thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Cập nhật món ăn thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var existingCategory = await _foodService.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new { message = $"Không tìm thấy món ăn với ID {id}." });
                }

                bool isDelete = await _foodService.DeleteAsync(id);
                if (!isDelete)
                {
                    return BadRequest(new { message = "Xóa món ăn thất bại. Vui lòng thử lại!" });
                }
                return Ok(new { message = "Xóa món ăn thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

    }
}
