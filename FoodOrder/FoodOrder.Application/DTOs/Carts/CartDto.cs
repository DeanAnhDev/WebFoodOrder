

using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Orders;

namespace FoodOrder.Application.DTOs.Carts
{
    public class CartDto
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public ICollection<CartItemDto>? CartItems { get; set; }
    }
}
