using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Interfaces
{
    public interface ILocaltionRepository : IRepository<Location>
    {
        Task<List<Location>> GetAllAsync();
    }
}
