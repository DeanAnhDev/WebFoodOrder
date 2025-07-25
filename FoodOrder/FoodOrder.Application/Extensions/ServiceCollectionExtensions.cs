using FoodOrder.Application.Interfaces;
using FoodOrder.Application.MappingProfiles;
using FoodOrder.Application.Services;
using FoodOrder.Application.Services.Auth;
using FoodOrder.Application.Services.Carts;
using FoodOrder.Application.Services.Foods;
using Microsoft.Extensions.DependencyInjection;


namespace FoodOrder.Application.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ApplicationServices(this IServiceCollection services)
        {
            // Add AutoMapper
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            // Add services
            services.AddScoped<IFoodCategoryServices, FoodCategoryServices>();
            // Add services
            services.AddScoped<IFoodServices, FoodServices>();
            // Add services
            services.AddScoped<IComboServices, ComboServices>();
            // Add services
            services.AddScoped<IComboDetailServices, ComboDetailServices>();

            services.AddScoped<SlugService>();

            services.AddScoped<IAuthService, AuthService>();

            services.AddScoped<ICartService, CartService>();

            return services;
        }
    }
}
