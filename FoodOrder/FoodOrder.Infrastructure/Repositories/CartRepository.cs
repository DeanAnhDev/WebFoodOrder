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
                .FirstOrDefaultAsync(c => c.UserId == userId && c.Temporary == false);
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
            cart.Temporary = true;
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

        public async Task<bool> DeleteCartByIdAsync(int cartId)
        {
            var cart = await _dbSet.FindAsync(cartId);
            if (cart == null)
                return false;

            _dbSet.Remove(cart);
            return true;
        }

        public async Task<CartItem?> GetCartItemByIdAsync(int cartId, int cartItemId)
        {
            return await _context.CartItems
                .Include(ci => ci.Food)
                .Include(ci => ci.Combo)
                .FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.CartItemId == cartItemId);
        }

        public async Task<bool> DeleteCartItemAsync(CartItem cartItem)
        {
            _context.CartItems.Remove(cartItem);
            return await Task.FromResult(true);
        }

    }
}
