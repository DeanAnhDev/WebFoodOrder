

using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Entities.Orders
{
    public class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public ICollection<OrderDetail>? OrderDetails { get; set; }
    }
}
