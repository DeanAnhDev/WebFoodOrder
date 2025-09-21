using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.Services.Foods.Filter;


namespace FoodOrder.Application.Interfaces
{
    public interface IFoodServices
    {
        Task<FoodDto> GetBySlugAsync(string slug);
        Task<bool> AddAsync(FoodDtoCreate entity);
        Task<bool> UpdateAsync(FoodDtoUpdate entity);
        Task<bool> DeleteAsync(int id);
        Task<PagedResult<FoodDto>> GetPagedFoodsAsync(PagedQuery query);
        Task<FoodDto> GetByIdAsync(int id);
        Task<bool> UpdateFoodStatusAsync(int id, bool isActive);
    }
}
