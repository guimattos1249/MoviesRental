using MoviesRental.API.Cache;
using MoviesRental.Application;
using MoviesRental.Infrastructure;
using MoviesRental.Queries.Application;
using MoviesRental.Queries.Infrastructure;

namespace MoviesRental.API.Setup;

public static class DependencyInjection
{
    public static IServiceCollection AddDependencyInjection(this IServiceCollection services)
    {
        services.AddWriteApplication();
        services.AddWriteInfrastructure();
        services.AddReadApplication();
        services.AddReadInfrastructure();
        services.AddScoped<ICacheRepository, CacheRepository>();

        return services;
    }
}
