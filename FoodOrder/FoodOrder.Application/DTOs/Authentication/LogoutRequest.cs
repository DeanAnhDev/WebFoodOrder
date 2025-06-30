

namespace FoodOrder.Application.DTOs.Authentication
{
    public class LogoutRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
