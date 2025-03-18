using FoodOrder.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Orders
{
    public class CartItemConfigurations : IEntityTypeConfiguration<CartItem>
    {
        public void Configure(EntityTypeBuilder<CartItem> builder)
        {
            builder.HasKey(p => p.CartItemId);

            builder.HasOne(p => p.Food)
                .WithMany(o => o.CartItems)
                .HasForeignKey(p => p.FoodId);

            builder.Property(c => c.TotalPrice)
                .HasPrecision(18, 2);

            builder.Property(c => c.UnitPrice)
               .HasPrecision(18, 2);

            builder.HasOne(p => p.Combo)
              .WithMany(o => o.CartItems)
              .HasForeignKey(p => p.ComboId);
        }
    }
}
