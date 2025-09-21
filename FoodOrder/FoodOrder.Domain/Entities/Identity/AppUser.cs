using FoodOrder.Domain.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace FoodOrder.Domain.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public string? FullName { get; set; }
        public string? Location { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Cart { get; set; }
        public ICollection<Location>? Locations { get; set; }
    }
}
