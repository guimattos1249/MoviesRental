using Microsoft.Extensions.DependencyInjection;
using MoviesRental.Application.Contracts;
using MoviesRental.Infrastructure.Context;
using MoviesRental.Infrastructure.Repositories;

namespace MoviesRental.Infrastructure;

public static class InfrastructureServiceCollection
{
    public static void AddWriteInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<MoviesRentalWriteContext>();
        services.AddScoped<IDvdsWriteRepository, DvdsWriteRepository>();
        services.AddScoped<IDirectorsWriteRepository, DirectorsWriteRepository>();
    }
}
