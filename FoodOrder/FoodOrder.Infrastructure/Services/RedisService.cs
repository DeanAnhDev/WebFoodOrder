using FoodOrder.Application.Services;
using StackExchange.Redis;


namespace FoodOrder.Infrastructure.Services
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _db;
        public RedisService(IConnectionMultiplexer redis)
        {
            _db = redis.GetDatabase();
        }

        public async Task<string> GetJtiAsync(string userId, string jti)
        {
            var value = await _db.StringGetAsync($"jti:{userId}:{jti}");
            return value!;
        }

        public async Task<bool> SetJtiAsync(string userId, string jti, double expiryMinutes)
        {
            var isSet = await _db.StringSetAsync($"jti:{userId}:{jti}", (expiryMinutes * 60).ToString(), TimeSpan.FromMinutes(expiryMinutes));
            return isSet;
        }

        public async Task<bool> DeleteJtiAsync(string userId, string jti)
        {
            var isDeleted = await _db.KeyDeleteAsync($"jti:{userId}:{jti}");
            return isDeleted;
        }
    }
}
