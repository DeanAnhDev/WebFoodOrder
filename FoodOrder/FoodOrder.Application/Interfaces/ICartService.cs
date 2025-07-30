using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.Interfaces
{
    public interface ICartService
    {

        Task<CartResponse> GetOrCreateCartAsync(int userId);

        Task AddToCartAsync(int userId, int? foodId, int? comboId, int quantity);

        Task UpdateCartItemAsync(int userId, int cartItemId, int quantity);
        Task RemoveCartItemAsync(int userId, int cartItemId);
    }
}
