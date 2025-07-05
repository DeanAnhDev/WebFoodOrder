using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FoodCategoryController : ControllerBase
    {
        private readonly IFoodCategoryServices _foodCategoryService;
        public FoodCategoryController(IFoodCategoryServices foodCategoryService)
        {
            _foodCategoryService = foodCategoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFoodCategories()
        {
            try
            {
                var foodCategories = await _foodCategoryService.GetAllAsync();

                return Ok(foodCategories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("with-foods")]
        public async Task<IActionResult> GetFoodsAndCombosCategoriesWithFoods()
        {
            try
            {
                var foodCategoriesWithFoods = await _foodCategoryService.GetFoodCategoriesWithFoodsAsync();

                return Ok(foodCategoriesWithFoods);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetFoodCategoryById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var foodCategory = await _foodCategoryService.GetByIdAsync(id);

                if (foodCategory == null)
                {
                    return NotFound(new { message = "Không tìm thấy danh mục nào." });
                }

                return Ok(foodCategory);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateFoodCategory([FromBody] FoodCategoryDtoCreate categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool isCreated = await _foodCategoryService.AddAsync(categoryDto);

                if (!isCreated)
                {
                    return BadRequest(new { message = "Tạo danh mục thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Tạo danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpPut]
        public async Task<IActionResult> UpdateFoodCategory([FromBody] FoodCategoryDtoUpdate categoryDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                bool isUpdated = await _foodCategoryService.UpdateAsync(categoryDto);

                if (!isUpdated)
                {
                    return BadRequest(new { message = "Cập nhật danh mục thất bại. Vui lòng thử lại!" });
                }

                return Ok(new { message = "Cập nhật danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteFoodCategory(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { message = "ID không hợp lệ!" });
                }

                var existingCategory = await _foodCategoryService.GetByIdAsync(id);
                if (existingCategory == null)
                {
                    return NotFound(new { message = $"Không tìm thấy danh mục với ID {id}." });
                }

                bool isDelete = await _foodCategoryService.DeleteAsync(id);
                if (!isDelete)
                {
                    return BadRequest(new { message = "Xóa danh mục thất bại. Vui lòng thử lại!" });
                }
                return Ok(new { message = "Xóa danh mục thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }

        [HttpGet("slug/{slug}")]
        public async Task<IActionResult> GetFoodsByCategorySlug(string slug)
        {
            try
            {
                var foodsByCategorySlug = await _foodCategoryService.GetFoodsByCategorySlugAsync(slug);

                if (foodsByCategorySlug == null)
                {
                    return NotFound(new { message = "Không tìm thấy món ăn nào." });
                }

                return Ok(foodsByCategorySlug);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi server!", error = ex.Message });
            }
        }


    }
}
