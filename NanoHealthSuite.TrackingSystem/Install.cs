using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data;
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


        services.AddWorkflowContextServices(configuration, logger);
        return services;
    }

}