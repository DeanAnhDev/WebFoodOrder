using FoodOrder.Application.DTOs.Foods.Food;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IFoodServices : IInterfaces<FoodDto>, IInterfaceForCreateUpdateDelete<FoodDto>
    {

    }
}
