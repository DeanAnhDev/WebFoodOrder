using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;


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
        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<TEntity?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<TEntity?> GetBySlugAsync(string slug)
        {
            return await _dbSet.FirstOrDefaultAsync(entity => EF.Property<string>(entity, "Slug") == slug);
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


        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                
                return true;
            }
            return false;
        }
    }
}
