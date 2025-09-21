using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Orders
{
    public class CreateOrderDto
    {
        public int CartId { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? Note { get; set; }
        public int? LocationId { get; set; }
        public int? VoucherId { get; set; }
        public string? Reason { get; set; }
    }
}