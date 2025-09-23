using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<Cart?> GetCartByUserIdAsync(int userId)
        {
            return await _dbSet
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Promotion)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Promotion)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }



        public async Task<Cart> CreateCartAsync(int userId)
        {
            var cart = new Cart
            {
                UserId = userId,
                Temporary = false
            };

            await _dbSet.AddAsync(cart);
            return cart;
        }

        public async Task<Cart> CreateTemporaryCartAsync()
        {
            var cart = new Cart
            {
                Temporary = true
            };

            await _dbSet.AddAsync(cart);
            return cart;
        }

        public async Task<Cart?> GetCartByIdAsync(int cartId)
        {
            return await _dbSet
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Promotion)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Promotion)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task<bool> UpdateCartUserIdAsync(int cartId, int userId)
        {
            var cart = await _dbSet.FindAsync(cartId);
            if (cart == null || cart.Temporary != true)
                return false;

            cart.UserId = userId;
            cart.Temporary = false;
            return true;
        }

        public async Task<IEnumerable<Cart>> GetAllTemporaryCartsAsync()
        {
            return await _dbSet
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Combo!)
                        .ThenInclude(cb => cb.Promotion)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Images)
                .Include(c => c.CartItems!)
                    .ThenInclude(ci => ci.Food!)
                        .ThenInclude(f => f.Promotion)
                .Where(c => c.Temporary == true)
                .ToListAsync();
        }
    }
}
