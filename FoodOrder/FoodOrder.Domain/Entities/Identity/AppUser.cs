using FoodOrder.Domain.Entities.Orders;
using Microsoft.AspNetCore.Identity;

namespace FoodOrder.Domain.Entities.Identity
{
    public class AppUser : IdentityUser<int>
    {
        public ICollection<Order>? Orders { get; set; }
        public Cart? Cart { get; set; }
    }
}
