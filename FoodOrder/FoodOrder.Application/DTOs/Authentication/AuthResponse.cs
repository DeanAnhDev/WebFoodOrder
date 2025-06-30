namespace FoodOrder.Application.DTOs.Authentication
{
    public class AuthResponse
    {
        public bool Status { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int? ExpiresIn { get; set; }
    }

}
