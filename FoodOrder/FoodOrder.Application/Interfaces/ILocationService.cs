using FoodOrder.Application.DTOs.Foods.Food.Commands;
using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Identity.Location;

namespace FoodOrder.Application.Interfaces
{
    public interface ILocationService
    {
        Task<bool> AddAsync(CreateLocationDto entity);
        Task<bool> UpdateAsync(UpdateLocationDto entity);
        Task<bool> DeleteAsync(int id);
        Task<LocationDto> GetByIdAsync(int id);
        Task <List<LocationDto>> GetAllByIdUserAsync(int id);

    }
}
