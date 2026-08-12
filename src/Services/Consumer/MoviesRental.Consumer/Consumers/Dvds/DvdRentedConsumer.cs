using DnsClient.Internal;
using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.RentDvd;

namespace MoviesRental.Consumer.Consumers.Dvds;

public class DvdRentedConsumer(IMediator mediator, ILogger<DvdRentedConsumer> logger) : IConsumer<DvdRentedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DvdRentedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DvdRentedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new RentDvdCommand(@event.Id, @event.UpdatedAt);

            _logger.LogInformation($"Renting Dvd {@event.Id}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Something wrong during the renting of dvd {@event.Id}");
                throw new InvalidOperationException($"Falied to create dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} Rented");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DvdRentedEvent");
            throw;
        }
    }
}
