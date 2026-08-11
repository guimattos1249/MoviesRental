using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.DeleteDvd;

namespace MoviesRenatal.Consumer.Consumers.Dvds;

public class DvdDeletedConsumer(IMediator mediator, ILogger<DvdDeletedConsumer> logger) : IConsumer<DvdDeletedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DvdDeletedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DvdDeletedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new DeleteDvdCommand(@event.Id, @event.DeletedAt);

            _logger.LogInformation($"Deleting dvd {@event.Id}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Something wrong during the deletion of dvd {@event.Id}");
                throw new InvalidOperationException($"Falied to delete dvd {@event.Id}");
            }
            _logger.LogInformation($"Delete dvd {@event.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DvdDeletedEvent");
            throw;
        }
    }
}
}
