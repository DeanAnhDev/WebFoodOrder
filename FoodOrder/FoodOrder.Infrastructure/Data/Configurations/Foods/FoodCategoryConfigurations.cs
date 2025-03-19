using FoodOrder.Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class FoodCategoryConfiguration : IEntityTypeConfiguration<FoodCategory>
    {
        public void Configure(EntityTypeBuilder<FoodCategory> builder)
        {
            builder.HasKey(p => p.FoodCategoryId);
            builder.Property(p => p.ImageUrl)
                .IsRequired();
            builder.Property(p => p.CategoryName)
                .HasMaxLength(500)
                .IsRequired();
            builder.Property(p => p.Description)
               .HasMaxLength(500)
               .IsRequired();
        }
    }
}