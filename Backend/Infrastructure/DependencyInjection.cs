
using Core.Interfaces;
using Core.Interfaces.Auth;
using Core.Interfaces.Convo;
using Core.Interfaces.Convo.FileServices;
using Core.Interfaces.LLM;
using Core.Interfaces.LLM.Cache;
using Core.Interfaces.LLM.LMStudio;
using Core.Interfaces.Portal;
using Infrastructure.Contexts;
using Infrastructure.Repos;
using Infrastructure.Services;
using Infrastructure.Services.Auth;
using Infrastructure.Services.Convo.FileServices;
using Infrastructure.Services.Convo.Ocr;
using Infrastructure.Services.LLM;
using Infrastructure.Services.LLM.Cache;
using Infrastructure.Services.LLM.LMStudio;
using Infrastructure.Services.Portal;
using IronOcr;
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
        services.AddScoped<IPortalRepo, PortalRepo>();

        // Mappers
        services.AddScoped<IUserMapper, UserMapper>();
        services.AddScoped<IUniversityMapper, UniversityMapper>();
        services.AddScoped<IChatMapper, ChatMapper>();
        services.AddScoped<ILMStudioMapper, LMStudioMapper>();
        services.AddScoped<IPortalMapper, PortalMapper>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUniversityService, UniversityService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IChatMetadataService, ChatMetadataService>();
        services.AddScoped<IChatMetadataCache, ChatMetadataCache>();
        services.AddScoped<IFileTextParser, FileTextParser>();
        services.AddScoped<IPortalService, PortalService>();

        // LLM
        services.AddScoped<ILLMChatService, LMStudioChatService>();

        // ORC
        services.AddSingleton(new IronTesseract
        {
            Language = OcrLanguage.EnglishBest
        });
        services.AddSingleton<IOcrService, IronTesseractOcrService>();
        services.AddScoped<IFileProcessingService, FileProcessingService>();

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
        services.AddSingleton<ICurrentContext, CurrentContext>();
        services.AddScoped<IAuthMapper, AuthMapper>();

        return services;
    }
}
