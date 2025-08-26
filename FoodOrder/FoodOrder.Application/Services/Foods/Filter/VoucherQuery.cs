
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.Services.Foods.Filter
{
    public class VoucherQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Lọc theo mã voucher
        public string? Code { get; set; }

        // Lọc theo trạng thái hoạt động
        public bool? IsActive { get; set; }
        public VoucherType? Type { get; set; }
        // Lọc theo thời gian hiệu lực
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public DateTime? EndDateFrom { get; set; }
        public DateTime? EndDateTo { get; set; }

        // Lọc theo số lượng còn lại
        public bool? IsOutOfStock { get; set; }

        // Lọc theo điều kiện giá trị đơn hàng tối thiểu
        public decimal? MinOrderAmount { get; set; }
    }


}
