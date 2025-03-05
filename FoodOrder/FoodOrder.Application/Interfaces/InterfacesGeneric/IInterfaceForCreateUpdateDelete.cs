namespace FoodOrder.Application.Interfaces.InterfacesGeneric
{
    public interface IInterfaceForCreateUpdateDelete<TEntityDto> where TEntityDto : class
    {
        Task<bool> AddAsync(TEntityDto entity);
        Task<bool> UpdateAsync(TEntityDto entity);
        Task<bool> DeleteAsync(int id);
    }
}
