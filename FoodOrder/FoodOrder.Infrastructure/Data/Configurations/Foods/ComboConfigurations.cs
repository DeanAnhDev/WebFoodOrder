using FoodOrder.Domain.Entities.Foods;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class ComboConfiguration : IEntityTypeConfiguration<Combo>
    {
        public void Configure(EntityTypeBuilder<Combo> builder)
        {
            builder.HasKey(p => p.ComboId);

            builder.HasOne(p => p.FoodCategorys).WithMany(f => f.Combos).HasForeignKey(p => p.FoodCategoryId).OnDelete(DeleteBehavior.Restrict);

 
            builder.Property(p => p.ComboName)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(p => p.Price)
              .IsRequired()
              .HasPrecision(18, 2) ;

            builder.Property(p => p.Description)
              .IsRequired()
              .HasMaxLength(500);

            builder.Property(p => p.Status)
             .IsRequired();

            builder.Property(p => p.Slug)
            .IsRequired();

        }

    }
}
