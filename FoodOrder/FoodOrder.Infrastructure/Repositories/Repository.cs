using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;


namespace FoodOrder.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly FoodOrderDbContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        public Repository(FoodOrderDbContext context)
        {
            _context = context;
            _dbSet = context.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(dynamic id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<bool> AddAsync(TEntity entity)
        {
            if (entity != null)
            {
                await _dbSet.AddAsync(entity);
                return true;
            }
            return false;
        }

        public Task<bool> UpdateAsync(TEntity entity)
        {
            if (entity == null) return Task.FromResult(false);

            _dbSet.Update(entity);
            return Task.FromResult(true);
        }


        public async Task<bool> DeleteAsync(dynamic id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                
                return true;
            }
            return false;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

    }
}
