using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data;

namespace NanoHealthSuite.TrackingSystem;

public static class Install
{
    public static IServiceCollection AddWorkflowServices(
        this IServiceCollection services, 
        IConfiguration configuration, 
        ILogger? logger = null)
    {
        services.AddWorkflowContextServices(configuration, logger);
        return services;
    }

}