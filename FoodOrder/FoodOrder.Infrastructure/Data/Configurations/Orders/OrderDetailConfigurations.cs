using FoodOrder.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Infrastructure.Data.Configurations.Orders
{
    public class OrderDetailConfigurations : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            builder.HasKey(p => p.OrderDetailId);

            builder.HasOne(p => p.Food)
               .WithMany(o => o.OrderDetails)
               .HasForeignKey(p => p.FoodId);

            builder.HasOne(p => p.Combo)
              .WithMany(o => o.OrderDetails)
              .HasForeignKey(p => p.ComboId);

            builder.Property(p => p.Quantity)
                .IsRequired();


            builder.Property(c => c.DiscountedPrice)
                .HasPrecision(18, 2);

            builder.Property(c => c.OriginalPrice)
              .IsRequired()
              .HasPrecision(18, 2);

            builder.Property(c => c.TotalPrice)
              .IsRequired()
              .HasPrecision(18, 2);

            builder.Property(c => c.ItemName)
             .IsRequired();
        }
    }
}