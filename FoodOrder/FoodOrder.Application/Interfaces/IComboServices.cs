using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.DTOs.Foods.Combo.Queries;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Application.Interfaces
{
    public interface IComboServices
    {
        Task<ComboDto?> GetBySlugAsync(string slug);
        Task<ComboWithFoodDto?> GetComboWithFoodsBySlugAsync(string slug);
        Task<PagedResult<ComboDto>> GetPagedCombosAsync(PagedQuery query);
        Task<ComboDtoById?> GetByIdAsync(int id);
        Task<bool> AddAsync(ComboDtoCreate entity);
        Task<bool> UpdateAsync(ComboDtoUpdate entity);
        Task<bool> DeleteAsync(int id);
        Task<List<FoodDto>> GetFoodsNotInComboAsync();
        Task<List<ComboDto>> GetAllComboAsync();
        Task UpdateCombosByFoodIdAsync(int foodId);
    }
}
