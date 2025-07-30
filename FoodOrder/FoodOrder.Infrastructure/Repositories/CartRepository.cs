using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(FoodOrderDbContext context) : base(context) {}

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _dbSet.Include(c => c.CartItems).ThenInclude(ci => ci.Combo).ThenInclude(cb => cb.Images)
                                .Include(c => c.CartItems).ThenInclude(ci => ci.Food).ThenInclude(f => f.Images)
                               .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart> CreateCartAsync(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
            };

            await _dbSet.AddAsync(cart);
            return cart;
        }

    }
}
