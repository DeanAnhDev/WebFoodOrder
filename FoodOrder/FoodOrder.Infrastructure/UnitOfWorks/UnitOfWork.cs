using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using FoodOrder.Infrastructure.Repositories;

namespace FoodOrder.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodOrderDbContext _context;
        public IRepository<Food> Foods { get; private set; }
        public IFoodCategoryRepository FoodCategories { get; private set; }
        public IRepository<Combo> Combos { get; private set; }
        public IRepository<ComboDetail> ComboDetails { get; private set; }
        public IRepository<AppUser> AppUsers { get; private set; }
        public IRepository<AppRole> AppRoles { get; private set; }
        public IRepository<CartItem> CartItems { get; private set; }
        public IRepository<Order> Orders { get; private set; }
        public IRepository<Cart> Carts { get; private set; }
        public IRepository<OrderDetail> OrderDetails { get; private set; }

        public UnitOfWork(FoodOrderDbContext context)
        {
            _context = context;
            Foods = new Repository<Food>(_context);
            FoodCategories = new FoodCategoryRepository(_context);
            Combos = new Repository<Combo>(_context);
            ComboDetails = new Repository<ComboDetail>(_context);
            AppUsers = new Repository<AppUser>(_context);
            AppRoles = new Repository<AppRole>(_context);
            Carts = new Repository<Cart>(_context);
            CartItems = new Repository<CartItem>(_context);
            Orders = new Repository<Order>(_context);
            OrderDetails = new Repository<OrderDetail>(_context);
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
