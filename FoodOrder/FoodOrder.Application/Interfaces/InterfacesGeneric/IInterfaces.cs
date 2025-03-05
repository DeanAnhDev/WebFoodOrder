namespace FoodOrder.Application.Interfaces.InterfacesGeneric
{
    public interface IInterfaces<TEntityDto> where TEntityDto : class
    {
        Task<IEnumerable<TEntityDto>> GetAllAsync();
        Task<TEntityDto?> GetByIdAsync(int id);
    }
}
