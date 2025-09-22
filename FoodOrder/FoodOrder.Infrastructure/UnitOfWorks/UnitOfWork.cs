
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
        public IUserRepository AppUsers { get; }
        public IRepository<AppRole> AppRoles { get; }
        public ICartItemRepository CartItems { get; }
        public IOrderRepository Orders { get; }
        public ICartRepository Carts { get; }
        public IRepository<OrderDetail> OrderDetails { get; }
        public IRepository<Images> Images { get; }
        public ILocationRepository Locations { get; }
        public IVoucherRepository Vouchers { get; }
        public IPromotionRepository Promotions { get; }

        public UnitOfWork(FoodOrderDbContext context,
                          IFoodRepository foodRepo,
                          IFoodCategoryRepository foodCategoryRepo,
                          IComboRepository comboRepo,
                          IComboDetailRepository comboDetailRepo,
                          IUserRepository appUserRepo,
                          IRepository<AppRole> appRoleRepo,
                          ICartRepository cartRepo,
                          ICartItemRepository cartItemRepo,
                          IOrderRepository orderRepo,
                          IRepository<OrderDetail> orderDetailRepo,
                          IRepository<Images> orderImageRepo,
                          ILocationRepository locations,
                          IVoucherRepository vouchers,
                          IPromotionRepository promotions)
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
            Locations = locations;
            Vouchers = vouchers;
            Promotions = promotions;
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