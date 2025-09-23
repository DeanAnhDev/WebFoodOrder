using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Orders
{
    public class GetOrdersRequestDto
    {
        public string? OrderCode { get; set; }
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        // Pagination parameters
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Sorting parameters
        public string? SortBy { get; set; } = "CreatedAt";
        public string? SortOrder { get; set; } = "desc"; // asc or desc
    }
}