
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Entities.Orders;


namespace FoodOrder.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IFoodRepository Foods { get; }
        IFoodCategoryRepository FoodCategories { get; }
        IComboRepository Combos { get; }
        IComboDetailRepository ComboDetails { get; }
        IUserRepository AppUsers { get; }
        IRepository<AppRole> AppRoles { get; }
        ICartRepository Carts { get; }
        ICartItemRepository CartItems { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        IRepository<Images> Images { get; }
        ILocationRepository Locations { get; }
        IVoucherRepository Vouchers { get; }
        IPromotionRepository Promotions { get; }
        Task<int> CompleteAsync();
    }
}