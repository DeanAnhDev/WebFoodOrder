using FoodOrder.Application.DTOs.Identity;

namespace FoodOrder.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
    }
}
