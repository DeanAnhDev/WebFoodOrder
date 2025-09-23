using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Interfaces
{
    public interface IUserRepository : IRepository<AppUser>
    {
        Task<AppUser?> GetByIdAsync(int id);
        Task<(IEnumerable<AppUser> users, int totalCount)> GetCustomersAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? email = null,
            string? phoneNumber = null);
        Task<(IEnumerable<AppUser> users, int totalCount)> GetStaffAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm = null,
            string? email = null,
            string? phoneNumber = null);
    }
}
