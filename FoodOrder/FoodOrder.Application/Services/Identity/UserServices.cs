using AutoMapper;
using FoodOrder.Application.DTOs.Identity;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Identity
{
    internal class UserServices : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly UserManager<AppUser> _userManager;
        public UserServices(IUnitOfWork unitOfWork, IMapper mapper, UserManager<AppUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
        }

        public async Task ChangePasswordAsync(int id, UserPasswordUpdateDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                throw new ArgumentException("Xác nhận mật khẩu không khớp");

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                throw new ArgumentException("Không tìm thấy người dùng");

            var result = await _userManager.ChangePasswordAsync(user, dto.CurrentPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể đổi mật khẩu: {errors}");
            }
        }

        public async Task<UserDto> GetByIdAsync(int id)
        {
            var foods = await _unitOfWork.AppUsers.GetByIdAsync(id);
            return _mapper.Map<UserDto>(foods);
        }

        public async Task<bool> UpdateAsync(int id, UpdateUserDto dto)
        {
            try
            {
                var existing = await _unitOfWork.AppUsers.GetByIdAsync(id);
                if (existing == null)
                    throw new ArgumentException("Không tìm thấy");
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    existing.FullName = dto.FullName;
                }

                var updated = await _unitOfWork.AppUsers.UpdateAsync(existing);
                if (!updated) return false;

                return await _unitOfWork.CompleteAsync() > 0;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception($"EF Save Error: {dbEx.InnerException?.Message ?? dbEx.Message}");
            }
        }

        public async Task<PagedResultDto<CustomerDto>> GetCustomersAsync(GetCustomersRequestDto request)
        {
            var (users, totalCount) = await _unitOfWork.AppUsers.GetCustomersAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.Email,
                request.PhoneNumber);

            var customerDtos = users.Select(user =>
            {
                var customerDto = _mapper.Map<CustomerDto>(user);
                // Set created date if available (assuming you have this field or can derive it)
                // customerDto.CreatedDate = user.CreatedDate; // Uncomment if you have this field
                return customerDto;
            });

            return new PagedResultDto<CustomerDto>(
                customerDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }

    }
}
