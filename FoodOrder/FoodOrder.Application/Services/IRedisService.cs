
namespace FoodOrder.Application.Services
{
    public interface IRedisService
    {
        Task<string> GetJtiAsync(string userId, string Jti);

        Task<bool> SetJtiAsync(string userId, string Jti, double expiryMinutes);

        Task<bool> DeleteJtiAsync(string userId, string Jti);
    }
}
