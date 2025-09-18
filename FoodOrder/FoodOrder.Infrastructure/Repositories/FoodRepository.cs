using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        public FoodRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<Food> GetQueryableWithIncludes()
        {
            return _dbSet
                .Include(f => f.Images)
                .Include(f => f.FoodCategory)
                .Include(f => f.Promotion)
                .AsQueryable();
        }
        public async Task<List<Food>> GetAllAsync()
        {
            return await _dbSet
                .Include(f => f.Images).ToListAsync();
        }

        public async Task<Food?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.FoodId == id);
        }

        public async Task<Food?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(f => f.Images)
                .Include(f => f.Promotion)
                .FirstOrDefaultAsync(f => f.Slug == slug);
                
        }
    }
}
