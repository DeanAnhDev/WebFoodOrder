
namespace FoodOrder.Application.Services.Foods.Filter
{
    public class PromotionQuery
    {
        public DateTime? StartDateFrom { get; set; }
        public DateTime? StartDateTo { get; set; }
        public bool? IsActive { get; set; }

        // Phân trang
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
