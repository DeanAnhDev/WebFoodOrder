using FoodOrder.Application.DTOs.Foods.FoodCategory;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IFoodCategoryServices : IInterfaces<FoodCategoryDto>, IInterfaceForCreateUpdateDelete<FoodCategoryDto>
    {
        Task<IEnumerable<FoodCategoryListFoodDto>> GetFoodCategoriesWithFoodsAsync();
    }
}
