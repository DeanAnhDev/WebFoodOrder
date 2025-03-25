using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;

namespace FoodOrder.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FoodOrderDbContext _context;

        public IRepository<Food> Foods { get; }
        public IFoodCategoryRepository FoodCategories { get; }
        public IRepository<Combo> Combos { get; }
        public IRepository<ComboDetail> ComboDetails { get; }
        public IRepository<AppUser> AppUsers { get; }
        public IRepository<AppRole> AppRoles { get; }
        public IRepository<CartItem> CartItems { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<Cart> Carts { get; }
        public IRepository<OrderDetail> OrderDetails { get; }

        public UnitOfWork(FoodOrderDbContext context,
                          IRepository<Food> foodRepo,
                          IFoodCategoryRepository foodCategoryRepo,
                          IRepository<Combo> comboRepo,
                          IRepository<ComboDetail> comboDetailRepo,
                          IRepository<AppUser> appUserRepo,
                          IRepository<AppRole> appRoleRepo,
                          IRepository<Cart> cartRepo,
                          IRepository<CartItem> cartItemRepo,
                          IRepository<Order> orderRepo,
                          IRepository<OrderDetail> orderDetailRepo)
        {
            _context = context;
            Foods = foodRepo;
            FoodCategories = foodCategoryRepo;
            Combos = comboRepo;
            ComboDetails = comboDetailRepo;
            AppUsers = appUserRepo;
            AppRoles = appRoleRepo;
            Carts = cartRepo;
            CartItems = cartItemRepo;
            Orders = orderRepo;
            OrderDetails = orderDetailRepo;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
