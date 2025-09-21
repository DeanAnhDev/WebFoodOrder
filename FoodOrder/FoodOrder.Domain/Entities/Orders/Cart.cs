
using FoodOrder.Domain.Entities.Identity;

namespace FoodOrder.Domain.Entities.Orders
{
    public class Cart
    {
        public int CartId { get; set; }
        public int UserId { get; set; }
        public bool? Temporary { get; set; }
        public AppUser? AppUser { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
    }
}
