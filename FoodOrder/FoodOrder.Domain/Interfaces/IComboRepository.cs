
using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IComboRepository : IRepository<Combo>
    {
        IQueryable<Combo> GetComboWithFoodsBySlug(string slug);
    }
}
