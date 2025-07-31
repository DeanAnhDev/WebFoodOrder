using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> GetByIdAsync(int id);
    }
}
