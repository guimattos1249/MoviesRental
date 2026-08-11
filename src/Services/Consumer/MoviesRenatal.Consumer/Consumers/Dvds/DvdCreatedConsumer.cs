using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.CreateDvd;

namespace MoviesRenatal.Consumer.Consumers.Dvds;

public class DvdCreatedConsumer(IMediator mediator, ILogger<DvdCreatedConsumer> logger) : IConsumer<DvdCreatedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DvdCreatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DvdCreatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new CreateDvdCommand(@event.Id, @event.Title, @event.Genre, @event.Published, @event.Available, @event.Copies, @event.DirectorId, @event.UpdatedAt, @event.CreatedAt);

            _logger.LogInformation($"Creating Dvd {@event.Title}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Something wrong during the creation of dvd {@event.Title}");
                throw new InvalidOperationException($"Falied to create dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Title} Created");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DvdCreatedEvent");
            throw;
        }
    }
}
