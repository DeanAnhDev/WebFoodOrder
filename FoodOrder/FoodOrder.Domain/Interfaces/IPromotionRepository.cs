using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IPromotionRepository: IRepository<Promotion>
    {
        Task<IEnumerable<Promotion>> GetAllWithRelationsAsync(); 
        Task<Promotion?> GetByIdWithRelationsAsync(int id);
    }
}
