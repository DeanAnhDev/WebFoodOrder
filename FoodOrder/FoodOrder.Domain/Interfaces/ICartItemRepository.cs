using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Domain.Interfaces
{
    public interface ICartItemRepository : IRepository<CartItem>
    {
        Task RemoveAsync(CartItem item);
        Task<CartItem?> GetByCartAndFoodAsync(int cartId, int foodId);
        Task<CartItem?> GetByCartAndComboAsync(int cartId, int comboId);
        Task<CartItem?> GetByIdAsync(int cartId);
    }
}
