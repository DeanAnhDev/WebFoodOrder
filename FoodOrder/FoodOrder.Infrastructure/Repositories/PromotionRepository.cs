using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class PromotionRepository : Repository<Promotion>, IPromotionRepository
    {
        public PromotionRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<IEnumerable<Promotion>> GetAllWithRelationsAsync()
        {
            return await _context.Promotions
                .Include(p => p.Foods)
                .ThenInclude(f => f.Images)
                .Include(p => p.Combos)
                .ThenInclude(c => c.Images)
                .ToListAsync();
        }

        public async Task<Promotion?> GetByIdWithRelationsAsync(int id)
        {
            return await _context.Promotions
                .Include(p => p.Foods)
                .ThenInclude(f => f.Images)
                .Include(p => p.Combos)
                .ThenInclude(c => c.Images)
                .FirstOrDefaultAsync(p => p.PromotionId == id);
        }
    }
}
