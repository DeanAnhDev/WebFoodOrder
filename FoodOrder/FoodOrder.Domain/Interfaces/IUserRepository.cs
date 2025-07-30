using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<AppUser?> GetByIdAsync(int id);
    }
}
