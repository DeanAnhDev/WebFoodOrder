using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Orders;


namespace FoodOrder.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Food> Foods { get; }
        IFoodCategoryRepository FoodCategories { get; }
        IRepository<Combo> Combos { get; }
        IRepository<ComboDetail> ComboDetails { get; }
        IRepository<AppUser> AppUsers { get; }
        IRepository<AppRole> AppRoles { get; }
        IRepository<Cart> Carts { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderDetail> OrderDetails { get; }
        Task<int> CompleteAsync();
    }
}
