using FoodOrder.Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class PromotionConfigurations : IEntityTypeConfiguration<Promotion>
    {
        public void Configure(EntityTypeBuilder<Promotion> builder)
        {
            builder.HasKey(p => p.PromotionId);


            builder.HasOne(p => p.Food)
                    .WithOne(f => f.Promotion)
                    .HasForeignKey<Promotion>(p => p.FoodId)
                    .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(p => p.Combo)
                    .WithOne(c => c.Promotion)
                    .HasForeignKey<Promotion>(p => p.ComboId)
                    .OnDelete(DeleteBehavior.SetNull);

            builder.Property(p => p.DiscountAmount)
                    .HasPrecision(18, 2);

        }
    }
}
