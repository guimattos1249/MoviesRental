using Microsoft.Extensions.DependencyInjection;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Infrastructure.Context;
using MoviesRental.Queries.Infrastructure.Repositories;

namespace MoviesRental.Queries.Infrastructure;

public static class InfrastructureServiceCollection
{
    public static IServiceCollection AddReadInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IMoviesRentalReadContext, MoviesRentalReadContext>();
        services.AddScoped<IDirectorsQueryRepository, DirectorsQueryRepository>();
        services.AddScoped<IDvdsQueryRepository, DvdsQueryRepository>();

        return services;
    }
}
