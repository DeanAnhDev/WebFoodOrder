
namespace FoodOrder.Infrastructure.Services.Models
{
    public class JwtOptions
    {
        public bool IsEnabled { get; set; }
        public string SecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int ExpiryMinutes { get; set; }
    }
}
