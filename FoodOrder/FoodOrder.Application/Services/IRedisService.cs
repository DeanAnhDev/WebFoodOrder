
namespace FoodOrder.Application.Services
{
    public interface IRedisService
    {
        Task<string> GetJtiAsync(string userId, string Jti);
        Task<bool> SetJtiAsync(string userId, string Jti, double expiryMinutes);
        Task<bool> DeleteJtiAsync(string userId, string Jti);
        Task<bool> SaveRefreshTokenAsync(string userId, string refreshToken, double expiryDays);
        Task<bool> ValidateRefreshTokenAsync(string userId, string refreshToken);
        Task<bool> DeleteRefreshTokenAsync(string userId, string refreshToken);
    }
}
