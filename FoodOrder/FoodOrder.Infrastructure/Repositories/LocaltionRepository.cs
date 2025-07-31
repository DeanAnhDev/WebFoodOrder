using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class LocaltionRepository : Repository<Location>, ILocaltionRepository
    {
        public LocaltionRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<List<Location>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
