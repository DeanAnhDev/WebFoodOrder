
using FoodOrder.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace FoodOrder.Domain.Interfaces
{
    public interface ILocationRepository : IRepository<Location>
    {
        Task<Location?> GetByIdAsync(int id);
        Task<List<Location>> GetAllByIdUserAsync(int id);
    }
}
