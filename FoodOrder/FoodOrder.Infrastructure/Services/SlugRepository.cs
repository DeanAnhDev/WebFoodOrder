using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Services
{
    public class SlugRepository : ISlugRepository
    {
        private readonly FoodOrderDbContext _context;

        public SlugRepository(FoodOrderDbContext context)
        {
            _context = context;
        }

        public async Task<bool> SlugExistsAsync<T>(string slug) where T : class
        {
            var dbSet = _context.Set<T>();
            return await dbSet.AnyAsync(e => EF.Property<string>(e, "Slug") == slug);
        }
    }
}