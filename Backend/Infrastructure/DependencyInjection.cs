
using Core.Interfaces;
using Core.Interfaces.Convo;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Infrastructure.Services;
using Infrastructure.Services.LLM;
using Infrastructure.Services.LLM.Cache;
using Infrastructure.Services.LLM.LMStudio;
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
        services.AddScoped<IChatRepo, ChatRepo>();

        // Mappers
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IUniversityMapper, UniversityMapper>();
        services.AddScoped<IChatMapper, ChatMapper>();
        services.AddScoped<ILMStudioMapper, LMStudioMapper>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatMetadataService, ChatMetadataService>();
        services.AddScoped<IChatMetadataCache, ChatMetadataCache>();

        // LLM
        services.AddScoped<ILLMService, LMStudioLLMService>();

        // extra
        services.AddScoped<ILLMInputGenerator, LLMInputGenerator>();

        // Api
        services.AddScoped<ILMStudioApi, LMStudioApi>();

        // Contexts
        services.AddDbContext<MyContext>();

        // Auth
        services.AddScoped<IAuthService, AuthService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<IEncryptor, Encryptor>();

        return services;
    }
}
