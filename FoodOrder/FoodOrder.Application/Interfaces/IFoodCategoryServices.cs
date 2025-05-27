using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IFoodCategoryServices : IInterfaces<FoodCategoryDto>, IInterfaceForCreateUpdateDelete<FoodCategoryDto>
    {
        Task<IEnumerable<FoodCategoryListFoodDto?>> GetFoodCategoriesWithFoodsAsync();
        Task<FoodCategoryDto?> GetBySlugAsync(string slug);
        Task<FoodsByCategory?> GetFoodsByCategorySlugAsync(string categorySlug);
        Task<CombosByCategory?> GetCombosByCategorySlugAsync(string categorySlug);
    }
}
