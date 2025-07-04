
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IComboRepository : IRepository<Combo>
    {
        IQueryable<Combo> GetComboWithFoodsBySlug(string slug);
        IQueryable<Combo> GetQueryableWithIncludes();
        Task<Combo?> GetByIdAsync( int id);
        Task<Combo?> GetBySlugAsync(string slug);

        Task<List<Combo>> GetCombosByFoodIdAsync(int foodId);
        Task<List<Food>> GetFoodsInComboAsync(int comboId);
    }
}

