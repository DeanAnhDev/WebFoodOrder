using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;

namespace FoodOrder.Infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly FoodOrderDbContext _context;

        public IFoodRepository Foods { get; }
        public IFoodCategoryRepository FoodCategories { get; }
        public IComboRepository Combos { get; }
        public IComboDetailRepository ComboDetails { get; }
        public IRepository<AppUser> AppUsers { get; }
        public IRepository<AppRole> AppRoles { get; }
        public IRepository<CartItem> CartItems { get; }
        public IRepository<Order> Orders { get; }
        public IRepository<Cart> Carts { get; }
        public IRepository<OrderDetail> OrderDetails { get; }
        public IRepository<Images> Images { get; }

        public UnitOfWork(FoodOrderDbContext context,
                          IFoodRepository foodRepo,
                          IFoodCategoryRepository foodCategoryRepo,
                          IComboRepository comboRepo,
                          IComboDetailRepository comboDetailRepo,
                          IRepository<AppUser> appUserRepo,
                          IRepository<AppRole> appRoleRepo,
                          IRepository<Cart> cartRepo,
                          IRepository<CartItem> cartItemRepo,
                          IRepository<Order> orderRepo,
                          IRepository<OrderDetail> orderDetailRepo,
                          IRepository<Images> orderImageRepo)
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
            Images = orderImageRepo;
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
