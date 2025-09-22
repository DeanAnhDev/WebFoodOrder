using FoodOrder.Domain.Entities.Foods;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Domain.Entities.Image;
using FoodOrder.Domain.Entities.Orders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace FoodOrder.Infrastructure.Data.Context
{
    public class FoodOrderDbContext(DbContextOptions<FoodOrderDbContext> options)
       : IdentityDbContext<AppUser, AppRole, int>(options)
    {
        public DbSet<Food> Foods { get; set; }
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<Combo> Combos { get; set; }
        public DbSet<ComboDetail> ComboDetails { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<AppRole> AppRoles { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Voucher> Vouchers { get; set; }
        public DbSet<Images> Images { get; set; }
        public DbSet<Location> Locations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            SeedRoles(modelBuilder);
        }

        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<AppRole>().HasData(
                new AppRole { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
                new AppRole { Id = 2, Name = "Customer", NormalizedName = "CUSTOMER" }
            );
        }
    }
}