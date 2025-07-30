using FoodOrder.Domain.Entities.Image;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Foods
{
    public class ImageConfigurations : IEntityTypeConfiguration<Images>
    {
        public void Configure(EntityTypeBuilder<Images> builder)
        {

            builder.Property(p => p.Url)
                .IsRequired();
            builder.Property(p => p.ThumbnailUrl)
                .IsRequired();
            builder.Property(p => p.Name)
                .IsRequired();

            builder.HasOne(i => i.Foods)
                .WithOne(f => f.Images)
                .HasForeignKey<Images>(i => i.FoodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.Combos)
                 .WithOne(f => f.Images)
                 .HasForeignKey<Images>(i => i.ComboId)
                 .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(i => i.FoodCategory)
                 .WithOne(f => f.Images)
                 .HasForeignKey<Images>(i => i.CategoryId)
                 .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
