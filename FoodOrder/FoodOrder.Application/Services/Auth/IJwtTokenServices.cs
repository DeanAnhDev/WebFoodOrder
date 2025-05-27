

using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Application.Services.Auth
{
    public interface IJwtTokenServices
    {
        Task<string> GenerateTokenAsync(AppUser user, DateTime expiryTime, bool isAccessToken);
        Task<string> GenerateAccessTokenAsync(AppUser user);
    }
}
