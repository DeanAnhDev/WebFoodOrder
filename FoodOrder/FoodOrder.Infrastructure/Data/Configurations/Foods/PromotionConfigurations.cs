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

            // Promotion 1-nhiều Food
            builder.HasMany(p => p.Foods)
                   .WithOne(f => f.Promotion)
                   .HasForeignKey(f => f.PromotionId)
                   .OnDelete(DeleteBehavior.SetNull);

            // Promotion 1-nhiều Combo
            builder.HasMany(p => p.Combos)
                   .WithOne(c => c.Promotion)
                   .HasForeignKey(c => c.PromotionId)
                   .OnDelete(DeleteBehavior.SetNull);


            builder.Property(p => p.DiscountAmount)
                    .HasPrecision(18, 2);

        }
    }
}
