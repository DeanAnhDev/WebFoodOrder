using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Domain.Interfaces
{
    public interface  ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);
    }
}
