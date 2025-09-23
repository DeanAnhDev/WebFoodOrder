namespace FoodOrder.Application.DTOs.Revenue
{
    public class RevenueRequestDto
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Period { get; set; } = "daily"; // daily, weekly, monthly, yearly
    }

    public class RevenueResponseDto
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalShippingFee { get; set; }
        public decimal TotalDiscount { get; set; }
        public int TotalOrders { get; set; }
        public List<RevenueDetailDto> Details { get; set; } = new();
    }

    public class RevenueDetailDto
    {
        public string Period { get; set; } = string.Empty; // 2024-09-24, 2024-W39, 2024-09, 2024
        public decimal Revenue { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Discount { get; set; }
        public int OrderCount { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}