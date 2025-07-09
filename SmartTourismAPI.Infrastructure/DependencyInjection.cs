using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartTourismAPI.Application.Abstractions.Providers;
using SmartTourismAPI.Application.Abstractions.Repositories;
using SmartTourismAPI.Infrastructure.Configurations;
using SmartTourismAPI.Infrastructure.DataContext;
using SmartTourismAPI.Infrastructure.DataSeeding;
using SmartTourismAPI.Infrastructure.Providers;
using SmartTourismAPI.Infrastructure.Repositories;

namespace SmartTourismAPI.Infrastructure;

public static class DependencyInjection {
    public static IServiceCollection AddInfrastructure(this IServiceCollection services) {
        services.AddDatabase();
        services.AddServices();
        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services) {
        // Configure EF Core DbContext
        services.AddDbContext<AppDbContext>((sp, options) => {
            options.UseNpgsql(sp.GetRequiredService<IConfiguration>().GetConnectionString("DefaultConnection"));
        });
        services.AddScoped<AppDbSeeder>();

        // Configure Jwt
        services.ConfigureOptions<JwtOptionsSetup>();
        services.ConfigureOptions<JwtBearerOptionsSetup>();

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services) {
        services.AddSingleton<IGisOsmProvider, GisOsmProvider>();
        services.AddScoped<IJwtProvider, JwtProvider>();

        services.AddScoped<IPlaceDetailRepository, PlaceDetailRepository>();
        services.AddScoped<IPlaceTypeRepository, PlaceTypeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IReviewRepository, ReviewRepository>();
        return services;
    }
}
