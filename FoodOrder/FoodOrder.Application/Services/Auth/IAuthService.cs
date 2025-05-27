using FoodOrder.Application.DTOs.Authentication;


namespace FoodOrder.Application.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterUser request);
        Task<AuthResponse> ConfirmEmailAsync(string token, string email);
        Task<AuthResponse> LoginAsync(LoginUser request);
    }
}
