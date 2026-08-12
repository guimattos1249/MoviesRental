using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Directors.Commands.UpdateDirector;

namespace MoviesRental.Consumer.Consumers.Directors;

public class DirectorUpdatedConsumer(IMediator mediator, ILogger<DirectorUpdatedConsumer> logger) : IConsumer<DirectorUpdatedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DirectorUpdatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DirectorUpdatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new UpdateDirectorCommand(@event.Id, @event.FullName, @event.UpdatedAt);

            _logger.LogInformation($"Updating director {@event.FullName}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Somenthing wrong happende during the update of director {@event.FullName}");
                throw new InvalidOperationException($"Falied to update director {@event.Id}");
            }
            _logger.LogInformation($"Director {@event.FullName} updated");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DirectorUpdatedEvent");
            throw;
        }
    }
}
