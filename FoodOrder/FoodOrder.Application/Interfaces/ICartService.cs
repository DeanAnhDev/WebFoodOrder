using FoodOrder.Application.DTOs.Carts;
using FoodOrder.Domain.Entities.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrder.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetOrCreateCartAsync(int userId);

        Task<CartResponse> CreateTemporaryCartAsync();

        Task AddToCartAsync(int userId, int? foodId, int? comboId, int quantity);

        Task AddToCartByIdAsync(int cartId, int? foodId, int? comboId, int quantity);

        Task UpdateCartItemAsync(int userId, int cartItemId, int quantity);

        Task UpdateCartItemByIdAsync(int cartId, int? foodId, int? comboId, int quantity);

        Task RemoveCartItemAsync(int userId, int cartItemId);

        Task<bool> AssignCartToUserAsync(int cartId, int userId);

        Task<IEnumerable<CartResponse>> GetAllTemporaryCartsAsync();

        Task<IEnumerable<CartDto>> GetAllTemporaryCartsBasicAsync();

        Task<CartResponse?> GetCartByIdAsync(int cartId);

        Task<bool> DeleteCartByIdAsync(int cartId);

        Task<bool> DeleteCartItemByIdAsync(int cartId, int cartItemId);
    }
}
