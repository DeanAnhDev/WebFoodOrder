using FoodOrder.Application.DTOs.Authentication;
using FoodOrder.Application.Interfaces;


namespace FoodOrder.Application.Services.Auth
{
    internal class AuthService : IAuthService
    {
        private readonly IIdentityService _identityService;

        public AuthService(IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<AuthResponse> RegisterAsync(RegisterUser request)
        {
            string role = "Customer";
            return await _identityService.CreateUserAsync(request, role);
        }

        public async Task<AuthResponse> ConfirmEmailAsync(string token, string email)
        {
            return await _identityService.ConfirmEmailAsync(token, email);
        }

        public Task<AuthResponse> LoginAsync(LoginUser request)
        {
            throw new NotImplementedException();
        }
    }
}
