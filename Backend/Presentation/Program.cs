
using System.Configuration;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Core.Dtos;
using Core.Dtos.Settings;
using Core.Dtos.Settings.Infrastructure;
using Core.Dtos.Settings.Presentation;
using Core.Interfaces.Convo;
using Core.Settings;
using FluentValidation;
using FluentValidation.AspNetCore;
using Infrastructure;
using Infrastructure.Contexts;
using Infrastructure.Services.LLM.LMStudio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.Filters;
using Presentation.SignalRHubs;
using Presentation.Validation;
using Presentation.Validator;

var builder = WebApplication.CreateBuilder(args);


// Allow Injecting HttpContext via DI
builder.Services.AddHttpContextAccessor();

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionHandlerFilter>();
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(new JsonSerializerOptions
{
    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    PropertyNameCaseInsensitive = true
});

// Http Clients
builder.Services.AddHttpClient<LMStudioApi>();

// DB context
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// Hub
builder.Services.AddSignalR();
builder.Services.AddSingleton<IMessageStreamer, SignalRMessageStreamer>();

// get Settings from Appsettings
builder.Services.Configure<ValidatorSettings>(builder.Configuration.GetSection("ValidatorSettings"));
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.Configure<EncryptorSettings>(builder.Configuration.GetSection("EncryptorSettings"));
builder.Services.Configure<ControllerSettings>(builder.Configuration.GetSection("ControllerSettings"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("RedisSettings"));

// LLM
builder.Services.Configure<LLMApiSettingKeys>(builder.Configuration.GetSection("LLMApiSettingKeys"));

builder.Services.Configure<LLMApiSettings>("ParserModel",
    builder.Configuration.GetSection("LLMApiSettings:ParserModel"));
builder.Services.Configure<LLMApiSettings>("ChatModel",
    builder.Configuration.GetSection("LLMApiSettings:ChatModel"));

builder.Services.Configure<LLMInputSettings>(builder.Configuration.GetSection("LLMInputSettings"));

// ORC
builder.Services.Configure<IronTesseractSettings>(builder.Configuration.GetSection("IronTesseractSettings"));
builder.Services.Configure<FileProcessingSettings>(builder.Configuration.GetSection("FileProcessingSettings"));

// Validaton
builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<CreateChatRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<MessageRequestValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ClientMessageValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<AddressValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ScheduleRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<LecturerRequestDtoValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<SyllabiRequestDtoValidator>();
builder.Services.AddFluentValidationAutoValidation();


// configure Auth Middleware
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwtBearerOptions =>
{
    jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
        RoleClaimType = ClaimTypes.Role,
    };
});

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hub/chat");

app.UseCors("AllowFrontend");
app.Run();



