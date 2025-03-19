using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IComboServices: IInterfaces<ComboDto>, IInterfaceForCreateUpdateDelete<ComboDto>
    {

    }
}
