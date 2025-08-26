using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Infrastructure.Repositories
{
    internal class VoucherRepository : Repository<Voucher>, IVoucherRepository
    {
        public VoucherRepository(FoodOrderDbContext context) : base(context) { }

        public IQueryable<Voucher> GetAllVouchers()
        {
            return _dbSet.AsQueryable(); 
        }

        public async Task<Voucher?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(v => v.VoucherId == id);
        }
    }
}
