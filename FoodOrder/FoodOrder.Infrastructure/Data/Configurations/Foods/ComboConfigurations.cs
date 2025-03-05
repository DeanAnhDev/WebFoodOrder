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
    public class ComboConfiguration : IEntityTypeConfiguration<Combo>
    {
        public void Configure(EntityTypeBuilder<Combo> builder)
        {
            builder.HasKey(p => p.ComboId);

            builder.Property(p => p.ComboName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Price)
              .IsRequired();

            builder.Property(p => p.Image)
              .IsRequired();

            builder.Property(p => p.Description)
              .IsRequired()
              .HasMaxLength(500);

            builder.Property(p => p.Status)
             .IsRequired();


        }

    }
}
