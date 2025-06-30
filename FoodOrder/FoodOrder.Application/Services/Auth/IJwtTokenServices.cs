

using FoodOrder.Domain.Entities.Identity;
using System.Security.Claims;

namespace FoodOrder.Application.Services.Auth
{
    public interface IJwtTokenServices
    {
        Task<string> GenerateTokenAsync(AppUser user, DateTime expiryTime, bool isAccessToken);
        Task<string> GenerateAccessTokenAsync(AppUser user);
        Task<string> GenerateRefreshTokenAsync(AppUser user);
        ClaimsPrincipal? ValidateToken(string token);
    }
}
