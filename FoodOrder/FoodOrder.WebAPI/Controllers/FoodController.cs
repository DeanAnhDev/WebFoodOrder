
using FoodOrder.Application.DTOs.Foods.Food.Commands;
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

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateFoodStatus(int id, [FromBody] bool isActive)
        {
            try
            {
                var result = await _foodService.UpdateFoodStatusAsync(id, isActive);
                if (!result)
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        new { message = "Cập nhật trạng thái thất bại" });

                return Ok(new { message = "Cập nhật trạng thái thành công" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = ex.Message });
            }
        }

        [HttpGet("all-foods-and-combos")]
        public async Task<IActionResult> GetAllFoodsAndCombos()
        {
            try
            {
                var result = await _foodService.GetAllFoodsAndCombosAsync();

                return Ok(new
                {
                    success = true,
                    message = "Lấy danh sách food và combo thành công",
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi lấy danh sách food và combo",
                    detail = ex.Message
                });
            }
        }

        [HttpGet("search-foods-and-combos")]
        public async Task<IActionResult> SearchFoodsAndCombos([FromQuery] string? name)
        {
            try
            {
                var result = await _foodService.GetAllFoodsAndCombosByNameAsync(name);

                string message = string.IsNullOrWhiteSpace(name)
                    ? "Lấy danh sách food và combo thành công"
                    : $"Tìm kiếm '{name}' thành công";

                return Ok(new
                {
                    success = true,
                    message = message,
                    searchTerm = name,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Có lỗi xảy ra khi tìm kiếm food và combo",
                    detail = ex.Message
                });
            }
        }

    }
}
