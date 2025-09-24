using FoodOrder.Domain.Entities.Orders;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodOrder.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartByUserIdAsync(int userId);
        Task<Cart> CreateCartAsync(int userId);
        Task<Cart> CreateTemporaryCartAsync();
        Task<Cart?> GetCartByIdAsync(int cartId);
        Task<bool> UpdateCartUserIdAsync(int cartId, int userId);
        Task<IEnumerable<Cart>> GetAllTemporaryCartsAsync();
        Task<bool> DeleteCartByIdAsync(int cartId);
        Task<CartItem?> GetCartItemByIdAsync(int cartId, int cartItemId);
        Task<bool> DeleteCartItemAsync(CartItem cartItem);
    }
}
