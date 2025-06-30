using FoodOrder.Application.DTOs.Authentication;


namespace FoodOrder.Application.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthResponse> CreateUserAsync(RegisterUser registerUser, string role);

        Task<AuthResponse> ConfirmEmailAsync(string token, string email);

        Task<AuthResponse> LoginUserAsync(LoginUser loginUser);

        Task<AuthResponse> RefreshTokenAsync(string accessToken, string refreshToken);
        Task<bool> LogoutAsync(string accessToken, string refreshToken);
    }
}
