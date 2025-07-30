using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class UserRepository : Repository<AppUser>, IUserRepository
    {
        public UserRepository(FoodOrderDbContext context) : base(context) { }
        public async Task<AppUser?> GetByIdAsync(int id)
        {
            return await _dbSet
               .FirstOrDefaultAsync(u => u.Id == id);
        }
    }
}
