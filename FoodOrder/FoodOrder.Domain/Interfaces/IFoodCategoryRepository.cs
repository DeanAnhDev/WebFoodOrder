using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IFoodCategoryRepository : IRepository<FoodCategory>
    {
        IQueryable<FoodCategory> GetFoodCategoriesWithFoods();
        IQueryable<FoodCategory> GetFoodsByCategorySlug(string categorySlug);
        Task<IEnumerable<FoodCategory>> GetAllAsync();
        Task<FoodCategory?> GetByIdAsync(int id);
        Task<FoodCategory?> GetBySlugAsync(string slug);
    }
}
