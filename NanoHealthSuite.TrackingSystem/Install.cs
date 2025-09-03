using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data;
using NanoHealthSuite.TrackingSystem.Helpers;
using NanoHealthSuite.TrackingSystem.Processors;
using NanoHealthSuite.TrackingSystem.Services;

namespace NanoHealthSuite.TrackingSystem;

public static class Install
{
    public static IServiceCollection AddWorkflowServices(
        this IServiceCollection services, 
        IConfiguration configuration, 
        ILogger? logger = null)
    {
        services.AddScoped<IHashingService, HashingService>();
        services.AddScoped<ITokenServiceProvider, TokenServiceProvider>();
        services.AddScoped<AuthenticationService>();
        services.AddScoped<WorkflowService>();
        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

        services.AddWorkflowContextServices(configuration, logger);
        return services;
    }

    public static void ConfigureJwtAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection("JWTSettings").Get<JWTSettings>();
        
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = jwtSettings.ValidAudience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                        {
                            context.Response.Headers.Add("Token-Expired", "true");
                        }

                        return Task.CompletedTask;
                    }
                };
            });
    }

}