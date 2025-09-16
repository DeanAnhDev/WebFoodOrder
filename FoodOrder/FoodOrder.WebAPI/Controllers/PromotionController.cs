using FoodOrder.Application.DTOs.Foods.Promotion;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FoodOrder.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PromotionController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PromotionCreateDto dto)
        {
            try
            {
                var success = await _promotionService.CreatePromotionAsync(dto);
                if (success)
                    return Ok(new { message = "Tạo khuyến mãi thành công" });

                return StatusCode(500, new { message = "Có lỗi xảy ra khi tạo khuyến mãi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PromotionUpdateDto dto)
        {
            try
            {
                var success = await _promotionService.UpdatePromotionAsync(id, dto);
                if (success)
                    return Ok(new { message = "Cập nhật khuyến mãi thành công" });

                return StatusCode(500, new { message = "Có lỗi xảy ra khi cập nhật khuyến mãi" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var success = await _promotionService.DeletePromotionAsync(id);
                if (success)
                    return Ok(new { message = "Xóa khuyến mãi thành công" });

                return StatusCode(500, new { message = "Có lỗi xảy ra khi xóa khuyến mãi" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PromotionQuery query)
        {
            try
            {
                var result = await _promotionService.GetAllPromotionsAsync(query);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var promotion = await _promotionService.GetPromotionByIdAsync(id);
                return Ok(promotion);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

    }
}
