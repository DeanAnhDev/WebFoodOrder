using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IFoodRepository : IRepository<Food>
    {
        Task<IEnumerable<Food>> GetAllAsync();
        Task<Food> GetByIdAsync(int id);
    }
}
