using FoodOrder.Application.DTOs.Identity;

namespace FoodOrder.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task ChangePasswordAsync(int id, UserPasswordUpdateDto dto);
        Task<PagedResultDto<CustomerDto>> GetCustomersAsync(GetCustomersRequestDto request);
    }
}
