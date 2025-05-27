using FoodOrder.Domain.Entities.Foods;

namespace FoodOrder.Domain.Interfaces
{
    public interface IFoodCategoryRepository : IRepository<FoodCategory>
    {
        IQueryable<FoodCategory> GetFoodCategoriesWithFoods();
        IQueryable<FoodCategory> GetFoodsByCategorySlug(string categorySlug);
        IQueryable<FoodCategory> GetCombosByCategorySlug(string categorySlug);
    }
}
