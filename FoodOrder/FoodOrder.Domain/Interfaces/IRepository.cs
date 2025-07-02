
using System.Linq.Expressions;

namespace FoodOrder.Domain.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {


        Task<bool> AddAsync(TEntity entity);
        Task<bool> UpdateAsync(TEntity entity);
        Task<bool> DeleteAsync(dynamic id);
        Task<TEntity?> GetBySlugAsync(string slug);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    }
}
