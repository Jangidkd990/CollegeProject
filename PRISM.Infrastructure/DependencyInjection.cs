namespace PRISM.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PRISM.Core.Interfaces;
using PRISM.Data;
using PRISM.Data.Repositories;
using PRISM.Infrastructure.Services;

/// <summary>
/// Extension methods for setting up infrastructure services in the DI container.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Add database context
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        // Register repositories
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        // Register services
        services.AddScoped<IPowerBIService, PowerBIService>();
        services.AddScoped<IAuditService, AuditService>();

        return services;
    }
}