using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class ComboDetailRepository: Repository<ComboDetail>, IComboDetailRepository
    {
        public ComboDetailRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<bool> DeleteAsync(int comboId, int foodId)
        {

            var entity = await GetByIdAsync(comboId, foodId);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                return true;
            }
            return false;
        }

        public async Task<IEnumerable<ComboDetail>> GetComboDetailsByComboIdAsync(int comboId)
        {
            return await _dbSet.Where(cd => cd.ComboId == comboId).ToListAsync();
        }

        public async Task<ComboDetail?> GetByIdAsync(int comboId, int foodId)
        {
            return await _dbSet
                .Where(cd => cd.ComboId == comboId && cd.FoodId == foodId)
                .FirstOrDefaultAsync();
        }

        public Task<IEnumerable<ComboDetail>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ComboDetail> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ComboDetail>> GetByComboIdAsync(int comboId)
        {
            return await _dbSet
                .Where(cd => cd.ComboId == comboId)
                .ToListAsync();
        }

    }
}
