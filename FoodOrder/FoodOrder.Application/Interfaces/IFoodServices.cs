using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.DTOs.Foods.Food.Commands;


namespace FoodOrder.Application.Interfaces
{
    public interface IFoodServices
    {
        Task<FoodDto?> GetBySlugAsync(string slug);
        Task<bool> AddAsync(FoodDtoCreate entity);
        Task<bool> UpdateAsync(FoodDtoUpdate entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FoodDto>> GetAllAsync();
        Task<FoodDto> GetByIdAsync(int id);
    }
}
