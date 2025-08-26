using FoodOrder.Application.DTOs.Foods.Food.Queries;
using FoodOrder.Application.DTOs.Voucher;
using FoodOrder.Application.Services.Foods.Filter;

namespace FoodOrder.Application.Interfaces
{
    public interface IVoucherServices
    {
        Task<bool> AddAsync(VoucherCreateDto entity);
        Task<bool> UpdateAsync(UpdateVoucherDto entity);
        Task<bool> DeleteAsync(int id);
        Task<VoucherDto> GetByIdAsync(int id);
        Task<PagedResult<VoucherDto>> GetAllVoucherAsync(VoucherQuery query);
    }
}
