using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.UpdateDvd;

namespace MoviesRental.Consumer.Consumers.Dvds;

public class DvdUpdatedConsumer(IMediator mediator, ILogger<DvdUpdatedConsumer> logger) : IConsumer<DvdUpdatedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DvdUpdatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DvdUpdatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new UpdateDvdCommand(@event.Id, @event.Title, @event.Genre, @event.Published, @event.Copies, @event.DirectorId, @event.UpdatedAt);

            _logger.LogInformation($"Updating Dvd {@event.Title}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Something wrong during the update of dvd {@event.Title}");
                throw new InvalidOperationException($"Falied to update dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Title} Updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DvdUpdatedEvent");
            throw;
        }
    }
}
