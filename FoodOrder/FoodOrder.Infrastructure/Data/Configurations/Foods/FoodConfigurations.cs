using FoodOrder.Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class FoodConfigurations : IEntityTypeConfiguration<Food>
    {
        public void Configure(EntityTypeBuilder<Food> builder)
        {
            builder.HasKey(p => p.FoodId);

            builder.HasOne(p => p.FoodCategory)
                .WithMany(c => c.Foods)
                .HasForeignKey(p => p.FoodCategoryId);

            builder.Property(p => p.FoodName)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(p => p.Description)
               .HasMaxLength(500)
               .IsRequired();

            builder.Property(p => p.Price)
               .IsRequired()
               .HasPrecision(18, 2);

            builder.Property(p => p.Status)
                .IsRequired();
            builder.Property(p => p.Slug)
           .IsRequired();
        }
    }
}
