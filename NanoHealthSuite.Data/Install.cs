using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NanoHealthSuite.Data.Enums;
using WorkflowTracking.Infrastructure.Data;
using WorkflowTracking.Infrastructure.Repositories;

namespace NanoHealthSuite.Data;

public static class Install
{
    public static IServiceCollection AddWorkflowContextServices(
        this IServiceCollection services, 
        IConfiguration configuration, 
        ILogger? logger = null)
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
        RegisterRepositories(services);
        
        return services;
    }
    
    
    private static void RegisterRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IWorkflowStepRepository, WorkflowStepRepository>();
        services.AddScoped<IProcessRepository, ProcessRepository>();
        services.AddScoped<IProcessExecutionRepository, ProcessExecutionRepository>();
        services.AddScoped<ICustomValidationRepository, CustomValidationRepository>();
        services.AddScoped<IValidationLogRepository, ValidationLogRepository>();
    }
}