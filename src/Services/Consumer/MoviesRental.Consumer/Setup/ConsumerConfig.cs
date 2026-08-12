using MassTransit;
using Microsoft.Extensions.Options;
using MoviesRental.Consumer.Consumers.Directors;
using MoviesRental.Consumer.Consumers.Dvds;
using MoviesRental.Core.EventBus;
using MoviesRental.Queries.Application;
using MoviesRental.Queries.Infrastructure;
using MoviesRental.Queries.Infrastructure.Settings;

namespace MoviesRental.Consumer.Setup;

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
            config.AddConsumer<DirectorCreatedConsumer>();
            config.AddConsumer<DirectorUpdatedConsumer>();
            config.AddConsumer<DirectorDeletedConsumer>();
            config.AddConsumer<DvdCreatedConsumer>();
            config.AddConsumer<DvdUpdatedConsumer>();
            config.AddConsumer<DvdDeletedConsumer>();
            config.AddConsumer<DvdRentedConsumer>();
            config.AddConsumer<DvdReturnedConsumer>();
            config.UsingRabbitMq((ctx, cfg) =>
            {
                cfg.Host(configuration["EventBusSettings:HostAddress"]);
                cfg.ReceiveEndpoint(EventBusConstants.CREATED_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorCreatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.UPDATED_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorUpdatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.DELETED_DIRECTOR_QUEUE, c =>
                {
                    c.ConfigureConsumer<DirectorDeletedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.CREATED_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdCreatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.UPDATED_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdUpdatedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.DELETED_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdDeletedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.RENTED_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdRentedConsumer>(ctx);
                });
                cfg.ReceiveEndpoint(EventBusConstants.RETURNED_DVD_QUEUE, c =>
                {
                    c.ConfigureConsumer<DvdReturnedConsumer>(ctx);
                });
            });
        });

        return services;
    }
}
