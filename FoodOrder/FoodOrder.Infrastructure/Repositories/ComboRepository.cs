using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class ComboRepository : Repository<Combo>, IComboRepository
    {
        public ComboRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<Combo> GetComboWithFoodsBySlug(string comboSlug)
        {
            var result = _dbSet
               .AsNoTracking()
               .Where(c => c.Slug == comboSlug)
               .Include(c => c.ComboDetails!)
               .ThenInclude(cd => cd.Food)
               .AsSplitQuery();
            return result;
        }
    }
}
