
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class CartItemRepository : Repository<CartItem>, ICartItemRepository
    {
        public CartItemRepository(FoodOrderDbContext context) : base(context) { }

        public async Task<CartItem?> GetByCartAndFoodAsync(int cartId, int foodId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.CartId == cartId && x.FoodId == foodId);
        }

        public async Task<CartItem?> GetByCartAndComboAsync(int cartId, int comboId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(x => x.CartId == cartId && x.ComboId == comboId);
        }


        public Task RemoveAsync(CartItem item)
        {
            _dbSet.Remove(item);
            return Task.CompletedTask;
        }

        public async Task<CartItem?> GetByIdAsync(int cartItemId)
        {
            return await _dbSet
                  .FirstOrDefaultAsync(x => x.CartItemId == cartItemId);
        }
    }
}
