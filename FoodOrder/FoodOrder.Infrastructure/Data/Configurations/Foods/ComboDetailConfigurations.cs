using FoodOrder.Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class ComboDetailConfiguration : IEntityTypeConfiguration<ComboDetail>
    {
        public void Configure(EntityTypeBuilder<ComboDetail> builder)
        {
            builder.HasKey(p => new { p.FoodId, p.ComboId });

            builder.Property(p => p.Quantity)
                .IsRequired();

            builder.HasOne(p => p.Food)
              .WithMany(c => c.ComboDetails)
              .HasForeignKey(p => p.FoodId);
            builder.HasOne(p => p.Combo)
             .WithMany(c => c.ComboDetails)
             .HasForeignKey(p => p.ComboId);
        }

    }
}
