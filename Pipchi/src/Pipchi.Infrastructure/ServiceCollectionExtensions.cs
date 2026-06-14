using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pipchi.Core.Interfaces;
using Pipchi.Infrastructure.Data;
using Pipchi.Infrastructure.Elasticsearch;
using Pipchi.Infrastructure.Outbox;
using Pipchi.SharedKernel.Interfaces;
using Quartz;


namespace Pipchi.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDatabase(configuration)
            .AddServices()
            .AddBackgroundJobs(configuration)
            .AddRedis(configuration)
            .AddElasticSearch();

        return services;
    }

    private static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("Database")!;

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddSingleton<ISqlConnectionFactory>(options =>
            new SqlConnectionFactory(connectionString));

        return services;
    }

    private static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
        services.AddScoped(typeof(IReadRepository<>), typeof(CachedRepository<>));
        services.AddScoped(typeof(EfRepository<>));
        services.AddScoped<IAccountRepository, AccountRepository>();

        return services;
    }

    private static IServiceCollection AddBackgroundJobs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OutboxOptions>(configuration.GetSection("Outbox"));

        services.AddQuartz();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        services.ConfigureOptions<ProcessOutboxMessagesJobSetup>();

        return services;
    }

    private static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("Cache");
        });

        services.AddScoped<ISymbolCacheService, SymbolCacheService>();

        return services;
    }

    private static IServiceCollection AddElasticSearch(this IServiceCollection services)
    {
        services.AddScoped<AccountSearchService>();

        return services;
    }
}