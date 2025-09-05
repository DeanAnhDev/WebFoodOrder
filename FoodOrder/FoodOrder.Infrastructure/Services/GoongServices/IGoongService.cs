namespace FoodOrder.Infrastructure.Services.GoongServices
{
    public interface IGoongService
    {
        Task<LocationResult?> CheckHanoiAsync(string lat, string lng);
    }
}
