using MassTransit;
using Microsoft.Extensions.Options;
using MoviesRental.Queries.Application;
using MoviesRental.Queries.Infrastructure;
using MoviesRental.Queries.Infrastructure.Settings;

namespace MoviesRenatal.Consumer.Setup;

public static class ConsumerConfig
{
    public static IServiceCollection AddConsumerConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDbSettings>(configuration.GetSection(nameof(MongoDbSettings)));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<MongoDbSettings>>().Value);
        services.AddReadApplication();
        services.AddReadInfrastructure();

        services.AddMassTransit(config =>
        {

        });

        return services;
    }
}
