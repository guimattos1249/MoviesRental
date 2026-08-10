using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MoviesRental.Infrastructure.Context;
using MoviesRental.Queries.Infrastructure.Settings;

namespace MoviesRental.API.Setup;

public static class ApiConfig
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDependencyInjection();
        services.AddDbContext<MoviesRentalWriteContext>(options =>
        {
            options.UseSqlServer(configuration.GetConnectionString("SqlConnection"), opt =>
            {
                opt.EnableRetryOnFailure();
            });
        });
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetValue<string>("CacheSettings:ConnectionString");
        });

        services.AddMassTransit(config =>
        {
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["EventBusSettings:HostAddress"]);
            });
        });

        var cacheSettings = configuration["CacheSettings:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Settings string 'CacheSettings' was not configured.");
        var sqlConnection = configuration.GetConnectionString("SqlConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'SqlConnection' was not configured.");

        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);

        services.AddApiVersioning();
        services.AddHealthChecks()
            .AddRedis(cacheSettings, "Cache HealthCheck", HealthStatus.Degraded)
            .AddMongoDb(
                sp => sp.GetRequiredService<IMongoClient>(),
                sp => sp.GetRequiredService<MongoDbSettings>().DatabaseName,
                name: "MongoDB",
                failureStatus: HealthStatus.Degraded)
            .AddSqlServer(sqlConnection);

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
