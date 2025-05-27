using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using FoodOrder.Infrastructure.Identity;
using FoodOrder.Infrastructure.Repositories;
using FoodOrder.Infrastructure.Services;
using FoodOrder.Infrastructure.UnitOfWorks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace FoodOrder.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection InfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<FoodOrderDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                 b => b.MigrationsAssembly(typeof(FoodOrderDbContext).Assembly.FullName)
            ));

            //add scoped
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            services.AddScoped<IFoodCategoryRepository, FoodCategoryRepository>();

            services.AddScoped<IFoodRepository, FoodRepository>();

            services.AddScoped<ISlugRepository, SlugRepository>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<IRedisService, RedisService>();



            return services;
        }
    }
}
