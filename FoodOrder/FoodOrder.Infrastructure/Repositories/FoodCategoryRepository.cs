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
                .Include(fc => fc.Foods);
        }
    }
}
