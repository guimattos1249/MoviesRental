using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Directors.Commands.CreateDirector;

namespace MoviesRenatal.Consumer.Consumers.Directors;

public class DirectorCreatedConsumer(IMediator mediator, ILogger<DirectorCreatedConsumer> logger) : IConsumer<DirectorCreatedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DirectorCreatedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DirectorCreatedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new CreateDirectorCommand(@event.Id, @event.FullName, @event.CreatedAt, @event.UpdatedAt);

            _logger.LogInformation($"Creating director {command.FullName}");

            var response = await _mediator.Send(command, default);
            if (!response)
            {
                _logger.LogError($"Somenthing wrong happende during the creation of director {@event.FullName}");
                throw new InvalidOperationException($"Falied to create director {@event.Id}");
            }
            _logger.LogInformation($"Director {@event.Id} created");
        }
        catch (Exception ex) 
        {
            _logger.LogError(ex, "An error ocurred while consuming the DirectorCreatedEvent");
            throw;
        }
    }
}
