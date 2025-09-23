using FoodOrder.Application.DTOs.Identity;

namespace FoodOrder.Application.Interfaces
{
    public interface IUserService
    {
        Task<UserDto> GetByIdAsync(int id);
        Task<bool> UpdateAsync(int id, UpdateUserDto dto);
        Task ChangePasswordAsync(int id, UserPasswordUpdateDto dto);
        Task<PagedResultDto<CustomerDto>> GetCustomersAsync(GetCustomersRequestDto request);
        Task<PagedResultDto<StaffDto>> GetStaffAsync(GetStaffRequestDto request);
        Task<StaffDto> CreateStaffAsync(CreateStaffDto dto);
        Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto dto);
        Task<bool> DeleteStaffAsync(int id);
    }
}
