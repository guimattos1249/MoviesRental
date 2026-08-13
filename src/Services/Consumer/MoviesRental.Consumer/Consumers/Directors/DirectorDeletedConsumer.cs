using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Directors.Commands.DeleteDirector;

namespace MoviesRental.Consumer.Consumers.Directors;

public class DirectorDeletedConsumer(IMediator mediator, ILogger<DirectorDeletedConsumer> logger) : IConsumer<DirectorDeletedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DirectorDeletedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DirectorDeletedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new DeleteDirectorCommand(@event.Id);
            _logger.LogInformation($"Deleting director {command.Id}");

            var response = await _mediator.Send(command, default);

            if (!response)
            {
                _logger.LogError($"Somenthing wrong happende during the deletion of director {@event.Id}");
                throw new InvalidOperationException($"Falied to delete director {@event.Id}");
            }
            _logger.LogInformation($"Director {@event.Id} deleted");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DirectorDeletedEvent");
            throw;
        }
    }
}
