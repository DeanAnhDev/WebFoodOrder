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
                .WithOne(o => o.OrderDetail)
                .HasForeignKey<OrderDetail>(p => p.FoodId);

            builder.HasOne(p => p.Combo)
               .WithOne(o => o.OrderDetail)
               .HasForeignKey<OrderDetail>(p => p.ComboId);

            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.Property(p => p.UnitPrice)
              .IsRequired();

            builder.Property(p => p.TotalPrice)
            .IsRequired();
        }
    }
}