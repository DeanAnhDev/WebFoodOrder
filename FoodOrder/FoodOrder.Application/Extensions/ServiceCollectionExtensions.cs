using FoodOrder.Application.Interfaces;
using FoodOrder.Application.MappingProfiles;
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

            return services;
        }
    }
}
