using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class LocationRepository : Repository<Location>, ILocationRepository
    {
        public LocationRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<List<Location>> GetAllByIdUserAsync(int id)
        {
            return await _dbSet.Where(l => l.UserId == id).ToListAsync();
        }

        public async Task<Location?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
