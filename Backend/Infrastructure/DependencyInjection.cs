
using Core.Interfaces;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services)
    {
        // Add services here for Infrastructure

        // Repos
        services.AddScoped<IUserRepo, UserRepo>();
        services.AddScoped<IUniversityRepo, UniversityRepo>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IUniversityMapper, UniversityMapper>();

        // Contexts
        services.AddDbContext<MyContext>();

        // Auth
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IEncryptor, Encryptor>();

        return services;
    }
}
