using AutoMapper;
using FoodOrder.Application.DTOs.Voucher;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services.Foods.Filter;
using FoodOrder.Domain.Entities.Orders;
using FoodOrder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FoodOrder.Application.Services.Orders
{
    internal class VoucherService : IVoucherServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public VoucherService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<bool> AddAsync(VoucherCreateDto entity)
        {
            var existVoucher = await _unitOfWork.Vouchers
             .FirstOrDefaultAsync(vc => vc.Code == entity.Code.Trim());
            if (existVoucher != null)
                throw new InvalidOperationException("Mã voucher đã tồn tại");

            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new ArgumentException("Mã voucher không được để trống");

            if (entity.Quantity <= 0)
                throw new ArgumentException("Số lượng phải > 0");

            if (entity.DiscountAmount < 0)
                throw new ArgumentException("Giá trị giảm không được < 0");

            if(entity.Type == VoucherType.Percentage)
            {
                if (entity.DiscountAmount > 100 || entity.DiscountAmount <0)
                    throw new ArgumentException("Giá trị giảm từ 0% đến 100%");
            }

            if (entity.MinOrderPrice < 0)
                throw new ArgumentException("Giá trị đơn tối thiểu không được < 0");

            if (entity.MaxDiscountPrice < 0)
                throw new ArgumentException("Giá trị giảm tối đa không được < 0");

            if (entity.StartDate < DateTime.Now.Date)
                throw new ArgumentException("Ngày bắt đầu phải lớn hơn hoặc bằng ngày hiện tại");

            if (entity.StartDate > entity.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");

            if (entity.EndDate.Date < DateTime.Now.Date)
                throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại");

            var voucher = _mapper.Map<Voucher>(entity);
            var result = await _unitOfWork.Vouchers.AddAsync(voucher);
            if (!result) return false;
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var result = await _unitOfWork.Vouchers.DeleteAsync(id);
            if (result)
            {
                return await _unitOfWork.CompleteAsync() > 0;
            }
            return false;
        }

        public async Task<PagedResult<VoucherDto>> GetAllVoucherAsync(VoucherQuery query)
        {
            var voucherQuery = _unitOfWork.Vouchers.GetAllVouchers();

            if (!string.IsNullOrWhiteSpace(query.Code))
            {
                voucherQuery = voucherQuery.Where(v => v.Code.Contains(query.Code));
            }
            if (query.Type.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.Type == query.Type);
            }

            if (query.IsActive.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.IsActive == query.IsActive.Value);
            }

            if (query.StartDateFrom.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.StartDate >= query.StartDateFrom.Value);
            }

            if (query.StartDateTo.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.StartDate <= query.StartDateTo.Value);
            }

            if (query.EndDateFrom.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.EndDate >= query.EndDateFrom.Value);
            }

            if (query.EndDateTo.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.EndDate <= query.EndDateTo.Value);
            }

            if (query.IsOutOfStock.HasValue)
            {
                voucherQuery = query.IsOutOfStock.Value
                    ? voucherQuery.Where(v => v.Quantity == 0 || v.Quantity == null)
                    : voucherQuery.Where(v => v.Quantity > 0);
            }

            if (query.MinOrderAmount.HasValue)
            {
                voucherQuery = voucherQuery.Where(v => v.MinOrderPrice <= query.MinOrderAmount.Value);
            }

            var totalCount = await voucherQuery.CountAsync();

            var items = await voucherQuery
                .Skip((query.Page - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToListAsync();

            var voucherDtos = _mapper.Map<List<VoucherDto>>(items);

            return new PagedResult<VoucherDto>(voucherDtos, totalCount, query.Page, query.PageSize);
        }


        public async Task<VoucherDto> GetByIdAsync(int id)
        {
            var voucher = await _unitOfWork.Vouchers.GetByIdAsync(id);
            return _mapper.Map<VoucherDto>(voucher);
        }

        public async Task<bool> UpdateAsync(UpdateVoucherDto entity)
        {
            var existing = await _unitOfWork.Vouchers.GetByIdAsync(entity.VoucherId);
            if (existing == null)
                throw new ArgumentException("Không tìm thấy voucher nào");

            var existVoucher = await _unitOfWork.Vouchers.FirstOrDefaultAsync(vc => vc.Code == entity.Code.Trim());
            if (existVoucher != null && existVoucher.VoucherId != entity.VoucherId)
                throw new InvalidOperationException("Mã voucher đã tồn tại");

            if (string.IsNullOrWhiteSpace(entity.Code))
                throw new ArgumentException("Mã voucher không được để trống");

            if (entity.Quantity <= 0)
                throw new ArgumentException("Số lượng phải > 0");

            if (entity.DiscountAmount < 0)
                throw new ArgumentException("Giá trị giảm không được < 0");

            if (entity.Type == VoucherType.Percentage)
            {
                if (entity.DiscountAmount > 100 || entity.DiscountAmount < 0)
                    throw new ArgumentException("Giá trị giảm từ 0% đến 100%");
            }

            if (entity.MinOrderPrice < 0)
                throw new ArgumentException("Giá trị đơn tối thiểu không được < 0");

            if (entity.MaxDiscountPrice < 0)
                throw new ArgumentException("Giá trị giảm tối đa không được < 0");

            if (entity.StartDate > entity.EndDate)
                throw new ArgumentException("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc");

            if (entity.EndDate.Date < DateTime.Now.Date)
                throw new ArgumentException("Ngày kết thúc phải lớn hơn hoặc bằng ngày hiện tại");



            var voucher = _mapper.Map(entity, existing);
            var updated = await _unitOfWork.Vouchers.UpdateAsync(voucher);
            if (!updated) return false;
            return await _unitOfWork.CompleteAsync() > 0;
        }
    }
}
