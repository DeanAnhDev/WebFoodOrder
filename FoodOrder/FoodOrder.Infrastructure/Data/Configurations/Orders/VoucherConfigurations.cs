
using FoodOrder.Domain.Entities.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodOrder.Infrastructure.Data.Configurations.Orders
{
    public class VoucherConfigurations : IEntityTypeConfiguration<Voucher>
    {
        public void Configure(EntityTypeBuilder<Voucher> builder)
        {
            builder.HasKey(p => p.VoucherId);

            builder.HasMany(v => v.Orders)
                    .WithOne(o => o.Voucher)
                    .HasForeignKey(o => o.VoucherId)
                    .OnDelete(DeleteBehavior.SetNull);
            builder.Property(p => p.DiscountAmount)
                   .HasPrecision(18, 2);

            builder.Property(p => p.MaxDiscountPrice)
                    .HasPrecision(18, 2);

            builder.Property(p => p.MinOrderPrice)
                   .HasPrecision(18, 2);

            builder.Property(p => p.Quantity)
                 .IsRequired();

            builder.Property(p => p.Code)
               .IsRequired();
        }
    }
}
