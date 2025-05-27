using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FoodOrder.Infrastructure.Repositories
{
    public class FoodCategoryRepository : Repository<FoodCategory>, IFoodCategoryRepository
    {
        public FoodCategoryRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<FoodCategory> GetFoodCategoriesWithFoods()
        {
            return _dbSet
                .AsNoTracking()
                .Include(fc => fc.Foods)
                .Include(fc => fc.Combos)
                .AsSplitQuery();
        }

        public IQueryable<FoodCategory> GetFoodsByCategorySlug(string categorySlug)
        {
             var result =  _dbSet
                .AsNoTracking()
                .Where(fc => fc.Slug == categorySlug)
                .Include(fc => fc.Foods)
                .AsSplitQuery();
            return result;
        }

        public IQueryable<FoodCategory> GetCombosByCategorySlug(string categorySlug)
        {
            var result = _dbSet
               .AsNoTracking()
               .Where(fc => fc.Slug == categorySlug)
               .Include(fc => fc.Foods)
               .AsSplitQuery();
            return result;
        }
    }
}
