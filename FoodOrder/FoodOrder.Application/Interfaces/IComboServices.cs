using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.DTOs.Foods.Combo.Commands;
using FoodOrder.Application.Services.Foods.Filter;

namespace FoodOrder.Application.Interfaces
{
    public interface IComboServices
    {
        Task<ComboDto?> GetBySlugAsync(string slug);
        Task<ComboWithFoodDto?> GetComboWithFoodsBySlugAsync(string slug);
        Task<PagedResult<ComboDto>> GetPagedCombosAsync(PagedQuery query);
        Task<ComboDto?> GetByIdAsync(int id);
        Task<bool> AddAsync(ComboDtoCreate entity);
        Task<bool> UpdateAsync(ComboDtoUpdate entity);
        Task<bool> DeleteAsync(int id);
    }
}
