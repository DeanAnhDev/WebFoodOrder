using FoodOrder.Application.DTOs.Foods.Promotion;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Application.Interfaces
{
    public interface IPromotionService
    {
        Task<bool> CreatePromotionAsync(PromotionCreateDto dto);
        Task<bool> UpdatePromotionAsync(int id, PromotionUpdateDto dto);
        Task<bool> DeletePromotionAsync(int id);
        Task<PagedResult<PromotionDto>> GetAllPromotionsAsync(PromotionQuery query);
        Task<PromotionDto> GetPromotionByIdAsync(int id);
    }
}
