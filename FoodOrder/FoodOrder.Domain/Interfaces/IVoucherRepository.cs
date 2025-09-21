using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Domain.Interfaces
{
    public interface IVoucherRepository : IRepository<Voucher>
    {
        IQueryable<Voucher> GetAllVouchers();
        Task<Voucher?> GetByIdAsync(int id);

    }
}
