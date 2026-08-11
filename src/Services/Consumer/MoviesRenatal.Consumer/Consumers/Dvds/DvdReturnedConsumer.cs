using MassTransit;
using MediatR;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.RentDvd;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.ReturnDvd;

namespace MoviesRenatal.Consumer.Consumers.Dvds;

public class DvdReturnedConsumer(IMediator mediator, ILogger<DvdReturnedConsumer> logger) : IConsumer<DvdReturnedEvent>
{
    private readonly IMediator _mediator = mediator;
    private readonly ILogger<DvdReturnedConsumer> _logger = logger;

    public async Task Consume(ConsumeContext<DvdReturnedEvent> context)
    {
        try
        {
            var @event = context?.Message ?? throw new ArgumentException(null, nameof(context));
            var command = new ReturnDvdCommand(@event.Id, @event.UpdatedAt);

            _logger.LogInformation($"Renturning Dvd {@event.Id}");

            var result = await _mediator.Send(command, default);
            if (!result)
            {
                _logger.LogError($"Something wrong during the returning of dvd {@event.Id}");
                throw new InvalidOperationException($"Falied to Return dvd {@event.Id}");
            }

            _logger.LogInformation($"Dvd {@event.Id} Returned");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error ocurred while consuming the DvdReturnedEvent");
            throw;
        }
    }
}
