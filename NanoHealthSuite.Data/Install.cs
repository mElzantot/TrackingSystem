using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WorkflowTracking.Infrastructure.Data;

namespace NanoHealthSuite.Data;

public static class Install
{
    public static IServiceCollection AddWorkflowContextServices(this IServiceCollection services, IConfiguration configuration, ILogger? logger = null)
    {
        var connString = configuration.GetConnectionString("WorkflowDb");

        logger?.LogInformation("*** Using Connection string: " + connString);
        services.AddDbContext<WorkflowDbContext>(options =>
        {
            options
                .UseNpgsql(connString, config => config.EnableRetryOnFailure(3))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
                .ConfigureWarnings(warn =>
                    warn.Throw(RelationalEventId.QueryPossibleUnintendedUseOfEqualsWarning)
                );
        }, ServiceLifetime.Transient);

        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        return services;
    }
}