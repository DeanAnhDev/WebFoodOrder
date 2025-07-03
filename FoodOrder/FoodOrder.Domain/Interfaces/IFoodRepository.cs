using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IFoodRepository : IRepository<Food>
    {
        IQueryable<Food> GetQueryableWithIncludes();
        Task<Food?> GetByIdAsync(int id);
        Task<Food?> GetBySlugAsync(string slug);
    }
}
