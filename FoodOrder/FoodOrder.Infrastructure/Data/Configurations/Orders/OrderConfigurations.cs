using FoodOrder.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace FoodOrder.Infrastructure.Data.Configurations.Orders
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.HasKey(p => p.OrderId);

            builder.HasMany(p => p.OrderDetails)
                .WithOne(o => o.Order)
                .HasForeignKey(p => p.OrderId);

            builder.Property(p => p.OrderDate)
                .IsRequired();

            builder.Property(p => p.Status)
              .IsRequired();

        }

    }
}
