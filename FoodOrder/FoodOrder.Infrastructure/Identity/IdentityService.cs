using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.Common.Models;
using FoodOrder.Application.DTOs.Authentication;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services;
using FoodOrder.Application.Services.Auth;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Infrastructure.Services.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace FoodOrder.Infrastructure.Identity
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly IJwtTokenServices _jwtTokenServices;
        private readonly IRedisService _redisService;
        private readonly JwtOptions _jwtOptions;


        public IdentityService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IConfiguration configuration, IEmailService emailService, IJwtTokenServices jwtTokenServices,
        IRedisService redisService, IOptions<JwtOptions> jwtOptions)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _emailService = emailService;
            _jwtTokenServices = jwtTokenServices;
            _redisService = redisService;
            _jwtOptions = jwtOptions.Value;
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
            if (string.IsNullOrWhiteSpace(loginUser.UserName))
            {
                return new AuthResponse { Status = false, Message = "UserName is required" };
            }

            if (string.IsNullOrWhiteSpace(loginUser.Password))
            {
                return new AuthResponse { Status = false, Message = "Password is required" };
            }

            // Tìm user theo username hoặc email
            var user = await _userManager.FindByNameAsync(loginUser.UserName)
                       ?? await _userManager.FindByEmailAsync(loginUser.UserName);

            if (user == null || !await _userManager.CheckPasswordAsync(user, loginUser.Password))
            {
                return new AuthResponse { Status = false, Message = "Tài khoản hoặc mật khẩu không đúng" };
            }

            // Sinh access & refresh token
            var accessToken = await _jwtTokenServices.GenerateAccessTokenAsync(user);
            var refreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync(user);

            return new AuthResponse
            {
                Status = true,
                Message = "Đăng nhập thành công",
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = _jwtOptions.ExpiryMinutes
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = _jwtTokenServices.ValidateToken(accessToken);
            if (principal == null)
                return new AuthResponse { Status = false, Message = "Access token không hợp lệ." };

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                return new AuthResponse { Status = false, Message = "Token thiếu thông tin." };

            var isValidRefresh = await _redisService.ValidateRefreshTokenAsync(userId, refreshToken);
            if (!isValidRefresh)
                return new AuthResponse { Status = false, Message = "Refresh token không hợp lệ hoặc đã hết hạn." };

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new AuthResponse { Status = false, Message = "Không tìm thấy người dùng." };

            // Xoá refresh token cũ
            await _redisService.DeleteRefreshTokenAsync(userId, refreshToken);

            // Tạo mới
            var newAccessToken = await _jwtTokenServices.GenerateAccessTokenAsync(user);
            var newRefreshToken = await _jwtTokenServices.GenerateRefreshTokenAsync(user);

            return new AuthResponse
            {
                Status = true,
                Message = "Làm mới token thành công",
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresIn = _jwtOptions.ExpiryMinutes
            };
        }

        public async Task<bool> LogoutAsync(string accessToken, string refreshToken)
        {
            var principal = _jwtTokenServices.ValidateToken(accessToken);
            if (principal == null)
                return false;

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(jti))
                return false;

            // Thu hồi cả access và refresh token
            await _redisService.DeleteJtiAsync(userId, jti);
            await _redisService.DeleteRefreshTokenAsync(userId, refreshToken);

            return true;
        }

    }
}
