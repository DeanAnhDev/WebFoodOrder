using FoodOrder.Application.Common.Settings;
using FoodOrder.Application.Extensions;
using FoodOrder.Application.Interfaces;
using FoodOrder.Application.Services;
using FoodOrder.Domain.Entities.Identity;
using FoodOrder.Infrastructure.Data.Context;
using FoodOrder.Infrastructure.Data.Seeders;
using FoodOrder.Infrastructure.Extensions;
using FoodOrder.Infrastructure.Services.GoongServices;
using FoodOrder.Infrastructure.Services.AhamoveServices;
using FoodOrder.Infrastructure.Services.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var emailConfig = builder.Configuration
    .GetSection("EmailConfiguration")
    .Get<EmailConfiguration>() ?? throw new InvalidOperationException("Email configuration is missing.");
builder.Services.AddSingleton(emailConfig);

// Add Redis configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var config = builder.Configuration.GetConnectionString("Redis");
    return ConnectionMultiplexer.Connect(config!);
});

//goong services
builder.Services.Configure<GoongSettings>(builder.Configuration.GetSection("GoongSettings"));

// Đăng ký HttpClient cho GoongService
builder.Services.AddHttpClient<IGoongService, GoongService>();

//ahamove services
builder.Services.Configure<AhamoveSettings>(builder.Configuration.GetSection("AhamoveSettings"));

// Đăng ký HttpClient cho AhamoveService
builder.Services.AddHttpClient<IAhamoveService, AhamoveService>();

builder.Services.InfrastructureServices(builder.Configuration);
builder.Services.ApplicationServices();

//add config for JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));


builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger to DI container
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add Identity
builder.Services.AddIdentity<AppUser, AppRole>()
          .AddEntityFrameworkStores<FoodOrderDbContext>()
          .AddDefaultTokenProviders();

//Add config for required Email
builder.Services.Configure<IdentityOptions>(
    opts => opts.SignIn.RequireConfirmedEmail = true
    );

//Add Authentication    
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    var jwtOptions = builder.Configuration.GetSection("JwtOptions").Get<JwtOptions>();

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = jwtOptions.Issuer,
        ValidAudience = jwtOptions.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;

            var redisService = context.HttpContext.RequestServices.GetRequiredService<IRedisService>();
            var storedJti = await redisService.GetJtiAsync(userId, jti);

            if (storedJti == null)
            {
                context.Fail("Token đã bị thu hồi hoặc hết hạn.");
            }
        }
    };
});



builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        Scheme = "bearer",
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Description = "Nhập token vào đây: Bearer {token}",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };

    c.AddSecurityDefinition(jwtSecurityScheme.Reference.Id, jwtSecurityScheme);

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            jwtSecurityScheme,
            Array.Empty<string>()
        }
    });
});


var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyOrigin() // Đổi thành URL của Vue.js
                 .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();


// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Seed default data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<AppRole>>();

        await IdentitySeeder.SeedAsync(userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Lỗi khi seed dữ liệu mặc định");
    }
}

app.Run();
