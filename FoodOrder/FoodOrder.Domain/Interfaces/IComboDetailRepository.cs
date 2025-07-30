using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IComboDetailRepository : IRepository<ComboDetail>
    {
        Task<bool> DeleteAsync(int comboId, int foodId);
        Task<ComboDetail?> GetByIdAsync(int comboId, int foodId);
        Task<IEnumerable<ComboDetail>> GetComboDetailsByComboIdAsync(int comboId);
        Task<IEnumerable<ComboDetail>> GetAllAsync();
        Task<ComboDetail> GetByIdAsync(int id);

        Task<IEnumerable<ComboDetail>> GetByComboIdAsync(int comboId);
    }
}
