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

        public async Task<PagedResultDto<StaffDto>> GetStaffAsync(GetStaffRequestDto request)
        {
            var (users, totalCount) = await _unitOfWork.AppUsers.GetStaffAsync(
                request.PageNumber,
                request.PageSize,
                request.SearchTerm,
                request.Email,
                request.PhoneNumber);

            var staffDtos = users.Select(user =>
            {
                var staffDto = _mapper.Map<StaffDto>(user);
                return staffDto;
            });

            return new PagedResultDto<StaffDto>(
                staffDtos,
                totalCount,
                request.PageNumber,
                request.PageSize);
        }

        public async Task<StaffDto> CreateStaffAsync(CreateStaffDto dto)
        {
            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("Email này đã được sử dụng");
            }

            // Tạo user mới
            var newUser = new AppUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email, // Username là email
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = true // Có thể set false nếu cần xác thực email
            };

            var result = await _userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể tạo nhân viên: {errors}");
            }

            // Thêm role Staff cho user
            var roleResult = await _userManager.AddToRoleAsync(newUser, "Staff");
            if (!roleResult.Succeeded)
            {
                // Nếu không thể thêm role, xóa user đã tạo
                await _userManager.DeleteAsync(newUser);
                var errors = string.Join("; ", roleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể gán quyền nhân viên: {errors}");
            }

            return _mapper.Map<StaffDto>(newUser);
        }

        public async Task<StaffDto> UpdateStaffAsync(int id, UpdateStaffDto dto)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy nhân viên");
            }

            // Kiểm tra user có role Staff không
            var isStaff = await _userManager.IsInRoleAsync(user, "Staff");
            if (!isStaff)
            {
                throw new ArgumentException("User này không phải là nhân viên");
            }

            // Kiểm tra email mới có trùng với user khác không (trừ chính user này)
            var existingUser = await _userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null && existingUser.Id != user.Id)
            {
                throw new ArgumentException("Email này đã được sử dụng bởi user khác");
            }

            // Cập nhật thông tin
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.UserName = dto.Email; // Username luôn là email
            user.PhoneNumber = dto.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể cập nhật nhân viên: {errors}");
            }

            // Cập nhật password nếu có
            if (!string.IsNullOrWhiteSpace(dto.Password))
            {
                // Xóa password cũ và set password mới
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (!removePasswordResult.Succeeded)
                {
                    var errors = string.Join("; ", removePasswordResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Không thể xóa mật khẩu cũ: {errors}");
                }

                var addPasswordResult = await _userManager.AddPasswordAsync(user, dto.Password);
                if (!addPasswordResult.Succeeded)
                {
                    var errors = string.Join("; ", addPasswordResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Không thể cập nhật mật khẩu mới: {errors}");
                }
            }

            return _mapper.Map<StaffDto>(user);
        }

        public async Task<bool> DeleteStaffAsync(int id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                throw new ArgumentException("Không tìm thấy nhân viên");
            }

            // Kiểm tra user có role Staff không
            var isStaff = await _userManager.IsInRoleAsync(user, "Staff");
            if (!isStaff)
            {
                throw new ArgumentException("User này không phải là nhân viên");
            }

            // Xóa user
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Không thể xóa nhân viên: {errors}");
            }

            return true;
        }

    }
}
