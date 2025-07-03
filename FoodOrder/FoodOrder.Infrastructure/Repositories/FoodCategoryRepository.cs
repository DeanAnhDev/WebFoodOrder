using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class FoodCategoryRepository : Repository<FoodCategory>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<FoodCategory> GetFoodCategoriesWithFoods()
        {
            return _dbSet
                .AsNoTracking()
                .Include(fc => fc.Foods!)
                .ThenInclude((Food f) => f.Images)
                .Include(fc => fc.Combos!)
                .ThenInclude((Combo cb) => cb.Images)
                .AsSplitQuery();
        }

        public IQueryable<FoodCategory> GetFoodsByCategorySlug(string categorySlug)
        {
             var result =  _dbSet
                .AsNoTracking()
                .Where(fc => fc.Slug == categorySlug)
                .Include(fc => fc.Foods!).ThenInclude(f => f.Images)
                .AsSplitQuery();
            return result;
        }

        public IQueryable<FoodCategory> GetCombosByCategorySlug(string categorySlug)
        {
            var result = _dbSet
               .AsNoTracking()
               .Where(fc => fc.Slug == categorySlug)
               .Include(fc => fc.Combos!).ThenInclude(c => c.Images)
               .AsSplitQuery();
            return result;
        }

        public async Task<IEnumerable<FoodCategory>> GetAllAsync()
        {
            var result = await _dbSet
                .Include(fc => fc.Images)
                .ToListAsync();

            return result;
        }

        public async Task<FoodCategory?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.FoodCategoryId == id);
        }

        public async Task<FoodCategory?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.Slug == slug);
        }
    }
}
