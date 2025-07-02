using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class FoodRepository : Repository<Food>, IFoodRepository
    {
        public FoodRepository(FoodOrderDbContext context) : base(context) { }

        public Task<IEnumerable<Food>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Food> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(fc => fc.Images)
                .FirstOrDefaultAsync(fc => fc.FoodCategoryId == id);
        }
    }
}
