using FoodOrder.Application.DTOs.Foods.Combo;
using FoodOrder.Application.Interfaces.InterfacesGeneric;

namespace FoodOrder.Application.Interfaces
{
    public interface IComboDetailServices : IInterfaceForCreateUpdateDelete<ComboDetailDto>
    {
        Task<bool> DeleteAsync(int comboId, int foodId);

        Task<ComboDetailDto> GetByIdAsync(int comboId, int foodId);

        Task<IEnumerable<ComboDetailDto>> GetComboDetailsByComboIdAsync(int comboId);
    }
}
