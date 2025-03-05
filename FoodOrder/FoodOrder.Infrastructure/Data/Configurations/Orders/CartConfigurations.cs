using FoodOrder.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodOrder.Infrastructure.Data.Configurations.Orders
{
    public class CartConfigurations : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(p => p.CartId);

            builder.HasMany(p => p.CartItems)
                .WithOne(o => o.Cart)
                .HasForeignKey(p => p.CartId);

            builder.HasOne(p => p.AppUser)
              .WithOne(o => o.Cart)
              .HasForeignKey<Cart>(p => p.UserId);
        }
    }
}
