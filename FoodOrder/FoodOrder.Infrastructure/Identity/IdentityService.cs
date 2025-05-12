using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.Common.Models;
using FoodOrder.Application.DTOs.Authentication;
using FoodOrder.Application.Interfaces;
using FoodOrder.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FoodOrder.Infrastructure.Identity
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public IdentityService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration, IEmailService emailService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task<AuthResponse> ConfirmEmailAsync(string token, string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new AuthResponse { Status = false, Message = "User không tồn tại" };
            }

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return new AuthResponse { Status = true, Message = "Xác nhận Email thành công" };
            }

            return new AuthResponse { Status = false, Message = "Xác nhận Email thất bại" };
        }


        public async Task<AuthResponse> CreateUserAsync(RegisterUser registerUser, string role)
        {
            if (string.IsNullOrEmpty(registerUser.Email))
            {
                return new AuthResponse { Status = false, Message = "Email is required" };
            }

            var userExist = await _userManager.FindByEmailAsync(registerUser.Email);
            if (userExist != null)
            {
                return new AuthResponse { Status = false, Message = "Email đã được sử dụng" };
            }

            var user = new AppUser
            {
                UserName = registerUser.UserName,
                FullName = registerUser.FullName,
                Email = registerUser.Email,
                PhoneNumber = registerUser.PhoneNumber,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            if (!await _roleManager.RoleExistsAsync(role))
            {
                return new AuthResponse { Status = false, Message = "Vai trò không tồn tại" };
            }

            if (string.IsNullOrEmpty(registerUser.Password))
            {
                return new AuthResponse { Status = false, Message = "Password is required" };
            }

            var result = await _userManager.CreateAsync(user, registerUser.Password);
            if (!result.Succeeded)
            {
                return new AuthResponse { Status = false, Message = "Tạo tài khoản thất bại: " + string.Join(", ", result.Errors.Select(e => e.Description)) };
            }

            await _userManager.AddToRoleAsync(user, role);

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var apiUrl = _configuration["AppSettings:ApiBaseUrl"];
            var confirmationLink = $"{apiUrl}/api/auth/confirm-email?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            var message = new EmailDto(
                new List<string> { user.Email! },
                "Xác nhận tài khoản",
                $"Vui lòng nhấp vào link sau để xác nhận tài khoản: {confirmationLink}"
            );
            await _emailService.SendEmailAsync(message);

            return new AuthResponse { Status = true, Message = $"Tạo tài khoản thành công, Email xác nhận đã được gửi đến {user.Email}." };
        }

        public async Task<AuthResponse> LoginUserAsync(LoginUser loginUser)
        {
            //checking the user  
            var user = await _userManager.FindByNameAsync(loginUser.UserName);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                return new AuthResponse { Status = false, Message = "Tài khoản không tồn tại" };
            }
            // Add additional logic for successful login if needed  
            return new AuthResponse { Status = true, Message = "Đăng nhập thành công" };
        }
    }
}
