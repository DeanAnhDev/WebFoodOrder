using FoodOrder.Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace FoodOrder.Infrastructure.Data.Seeders
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // Seed Roles
            await SeedRolesAsync(roleManager);

            // Seed Users
            await SeedUsersAsync(userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            var roles = new[]
            {
                new AppRole { Name = "Admin" },
                new AppRole { Name = "Customer" }
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    await roleManager.CreateAsync(role);
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            // Seed Admin
            var adminEmail = "admin@foodorder.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FullName = "Administrator",
                    PhoneNumber = "0901234567",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            // Seed Customer
            var customerEmail = "customer@example.com";
            if (await userManager.FindByEmailAsync(customerEmail) == null)
            {
                var customer = new AppUser
                {
                    UserName = customerEmail,
                    Email = customerEmail,
                    EmailConfirmed = true,
                    FullName = "Khách Vãng Lai",
                    PhoneNumber = "0987654321",
                    PhoneNumberConfirmed = true
                };

                var result = await userManager.CreateAsync(customer, "Customer@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(customer, "Customer");
                }
            }
        }
    }
}