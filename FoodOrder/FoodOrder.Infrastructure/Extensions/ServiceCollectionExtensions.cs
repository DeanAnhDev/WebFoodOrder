using FoodOrder.Application.Common.Interfaces;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services;
using FoodOrder.Application.Services.Auth;
using FoodOrder.Domain.Interfaces;
using FoodOrder.Infrastructure.Data.Context;
using FoodOrder.Infrastructure.Identity;
using FoodOrder.Infrastructure.Repositories;
using FoodOrder.Infrastructure.Services;
using FoodOrder.Infrastructure.Services.CloudinaryServices;
using FoodOrder.Infrastructure.Services.GoongServices;
using FoodOrder.Infrastructure.Services.VnPayServices;
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

            services.AddScoped<IComboDetailRepository, ComboDetailRepository>();

            services.AddScoped<IComboRepository, ComboRepository>();

            services.AddScoped<ICartItemRepository, CartItemRepository>();

            services.AddScoped<ISlugRepository, SlugRepository>();

            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IIdentityService, IdentityService>();

            services.AddScoped<ICartRepository, CartRepository>();

            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped<IOrderRepository, OrderRepository>();

            services.AddScoped<IRedisService, RedisService>();

            services.AddScoped<Application.Interfaces.IVNPayService, VNPayService>();

            services.AddScoped<IJwtTokenServices, JwtTokenServices>();

            services.AddScoped<IVoucherRepository, VoucherRepository>();

            services.AddScoped<ILocationRepository, LocationRepository>();

            services.AddScoped<IPromotionRepository, PromotionRepository>();

            services.AddScoped<IRedisService, RedisService>();

            var cloudinarySetting = configuration.GetSection("Cloudinary").Get<CloudinarySetting>();
            if (cloudinarySetting == null)
            {
                throw new InvalidOperationException("Cloudinary configuration section is missing or invalid.");
            }
            services.AddSingleton(cloudinarySetting);

            services.AddScoped<ICloudinaryService, CloudinaryService>();
            return services;
        }
    }
}
