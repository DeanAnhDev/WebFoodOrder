using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class ComboRepository : Repository<Combo>, IComboRepository
    {
        public ComboRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<Combo> GetQueryableWithIncludes()
        {
            return _dbSet
                .Include(c => c.Images)
                .Include(c => c.FoodCategorys)
                .AsQueryable();
        }


        public async Task<Combo?> GetByIdWithDataAsync(int id)
        {
            return await _dbSet
                 .Include(cb => cb.Images)
                 .Include(cb => cb.ComboDetails).ThenInclude(cd => cd.Food)
                 .Include(cb => cb.ComboDetails).ThenInclude(cd => cd.Combo)
                 .FirstOrDefaultAsync(cb => cb.ComboId == id);
        }

        public async Task<Combo?> GetByIdAsync(int id)
        {
            return await _dbSet.Include(cb => cb.Images).FirstOrDefaultAsync(cb => cb.ComboId == id);
        }




        public async Task<Combo?> GetBySlugAsync(string slug)
        {
            return await _dbSet
                 .Include(fc => fc.Images)
                 .FirstOrDefaultAsync(fc => fc.Slug == slug);
        }

        public IQueryable<Combo> GetComboWithFoodsBySlug(string comboSlug)
        {
            var result = _dbSet
               .AsNoTracking()
               .Where(c => c.Slug == comboSlug)
               .Include(c => c.Images)
               .Include(c => c.ComboDetails!)
               .ThenInclude(cd => cd.Food).ThenInclude(f => f.Images)
               .AsSplitQuery();
            return result;
        }
    }
}
