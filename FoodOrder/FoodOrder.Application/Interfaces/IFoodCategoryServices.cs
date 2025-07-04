using FoodOrder.Application.DTOs.Foods.FoodCategory.Commands;
using FoodOrder.Application.DTOs.Foods.FoodCategory.Queries;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IFoodCategoryServices
    {
        Task<bool> AddAsync(FoodCategoryDtoCreate entity);
        Task<bool> UpdateAsync(FoodCategoryDtoUpdate entity);
        Task<bool> DeleteAsync(int id);
        Task<IEnumerable<FoodCategoryListFoodDto?>> GetFoodCategoriesWithFoodsAsync();
        Task<FoodCategoryDto?> GetBySlugAsync(string slug);
        Task<FoodCategoryListFoodDto?> GetFoodsByCategorySlugAsync(string categorySlug);
        Task<IEnumerable<FoodCategoryDto>> GetAllAsync();
        Task<FoodCategoryDto?> GetByIdAsync(int id);
    }
}
