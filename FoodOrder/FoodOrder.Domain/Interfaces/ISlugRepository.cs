namespace FoodOrder.Domain.Interfaces
{
    public interface ISlugRepository
    {
        Task<bool> SlugExistsAsync<T>(string slug) where T : class;
    }
}
