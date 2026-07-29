using AnonymousApplication.DTOs;
using AnonymousApplication.Interfaces;
using AnonymousApplication.Kafka;
using AnonymousApplication.Middleware;
using AnonymousApplication.Services;
using AnonymousInfrastructure.Data;
using AnonymousInfrastructure.Migrations;
using Hangfire;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Serilog;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;


Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog();

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSingleton<IDbConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<ISendMessage, SendMessageService>();
builder.Services.AddScoped<IMessages, MessagesService>();
builder.Services.AddScoped<IUserAuthentication, UserAuthenticationService>();
builder.Services.AddScoped<ICommonUtilities, CommonUtilityService>();
builder.Services.AddScoped<ISuperAdmin, SuperAdminService>();
builder.Services.AddScoped<IEmail, EmailService>();
builder.Services.AddScoped<IRegisteruser, RegisterUserService>();
builder.Services.AddScoped<ICompanyAdminInvite, InviteService>();
builder.Services.AddScoped<ICompanyEmployeeInvite, InviteService>();
builder.Services.AddScoped<IInviteVerify, InviteService>();
builder.Services.AddScoped<IOutbox, OutboxService>();
builder.Services.Configure<KafkaOptions>(builder.Configuration.GetSection(KafkaOptions.SectionName));
builder.Services.AddSingleton<IKafkaProducer, KafkaProducerService>();
builder.Services.AddHostedService<OutboxPublisher>();
builder.Services.AddHostedService<KafkaConsumer>();
builder.Services.AddSingleton<KafkaTopicInitializer>();
builder.Services.AddHostedService<KafkaTopicInitializerHostedService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Anonymous APIs",
        Version = "v1",
        Description = "Sample API with JWT auth",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // JWT Bearer auth scheme
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Enter your JWT token with Bearer prefix"
    });

    options.AddSecurityRequirement(document =>
    {
        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        };
        return requirement;
    });

    options.EnableAnnotations();
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200")
                  .WithHeaders("Content-Type", "Authorization")
                  .WithMethods("GET", "POST", "PUT", "DELETE")
                  .AllowCredentials();
        });
});


builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("Default")
    )
);

builder.Services.AddRateLimiter(options =>
{
    options.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = 429;

        await context.HttpContext.Response.WriteAsync("Too many requests. Please try after sometime.", token);
    };

    options.AddPolicy("loginLimiter", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(
            partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",

            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0
            }));

    options.AddPolicy("deleteLimiter", httpContext =>
        RateLimitPartition.GetTokenBucketLimiter(
            partitionKey:
                httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",

            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 2,
                TokensPerPeriod = 2,
                ReplenishmentPeriod = TimeSpan.FromSeconds(10),
                QueueLimit = 0,
                AutoReplenishment = true
            }));

    // =========================
    // MESSAGE / POST LIMITER
    // =========================
    options.AddPolicy("messageLimiter", httpContext =>
        RateLimitPartition.GetSlidingWindowLimiter(

            partitionKey:
                httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "anonymous",

            factory: _ => new SlidingWindowRateLimiterOptions
            {
                PermitLimit = 10, // 10 posts
                Window = TimeSpan.FromMinutes(1),
                SegmentsPerWindow = 6,
                QueueLimit = 0,
                AutoReplenishment = true
            }));


    // =========================
    // REACTION / LIKE LIMITER
    // =========================
    options.AddPolicy("reactionLimiter", httpContext =>
        RateLimitPartition.GetTokenBucketLimiter(

            partitionKey:
                httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? httpContext.Connection.RemoteIpAddress?.ToString()
                ?? "anonymous",

            factory: _ => new TokenBucketRateLimiterOptions
            {
                TokenLimit = 30,
                TokensPerPeriod = 30,
                ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

// After pulling the latest code, please run the migration command ONLY if new migration scripts have been added.
// Important: Do not run migrations unnecessarily — execute them only when new scripts are present after the latest pull.

// dotnet run --project AnonymousMigrator\AnonymousMigrator.csproj --environment Local


var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseCors("AllowAngular");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseHangfireDashboard("/hangfire");
app.MapControllers();
try
{
    Log.Information("Application Starting");

    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
