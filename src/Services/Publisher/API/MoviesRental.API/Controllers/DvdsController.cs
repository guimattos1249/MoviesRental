using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoviesRental.API.Cache;
using MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;
using MoviesRental.Application.UseCases.Dvds.Commands.DeleteDvd;
using MoviesRental.Application.UseCases.Dvds.Commands.RentDvd;
using MoviesRental.Application.UseCases.Dvds.Commands.ReturnDvd;
using MoviesRental.Application.UseCases.Dvds.Commands.UpdateDvd;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Dvds.Queries.GetDvd;
using System.Net;

namespace MoviesRental.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DvdsController(
    IMediator mediator,
    IPublishEndpoint publishEndpoint,
    ICacheRepository cacheRepository) : ApiController
{
    private readonly IMediator _mediator = mediator;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;
    private readonly ICacheRepository _cacheRepository = cacheRepository;

    [HttpGet("[action]/{title}", Name = "GetDvd")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDvd([FromRoute] string title)
    {
        var response = await _cacheRepository.GetAsync(title);
        if (response is not null)
            return CustomResponse((int)HttpStatusCode.OK, true, response);

        var query = new GetDvdQuery(title);

        var reponse = await _mediator.Send(query, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.NotFound, false);

        await _cacheRepository.Update(response);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpPost("CreateDvd")]
    [ProducesResponseType(typeof(CreateDvdResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateDvd(
        [FromBody] CreateDvdCommand command)
    {
        var response = await _mediator.Send(command, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        var @event = new DvdCreatedEvent(
            response.Id,
            response.Title,
            response.Genre,
            response.Published,
            response.Available,
            response.Copies,
            response.DirectorId,
            response.CreatedAt,
            response.UpdatedAt);

        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.Created, true, response);
    }

    [HttpPut("UpdateDvd")]
    [ProducesResponseType(typeof(UpdateDvdResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateDvd(
        [FromBody] UpdateDvdCommand command)
    {
        var response = await _mediator.Send(command, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        var @event = new DvdUpdatedEvent(
            response.Id,
            response.Title,
            response.Genre,
            response.Published,
            response.Copies,
            response.DirectorId,
            response.UpdatedAt);

        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpPut("RendDvd/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> RentDvd([FromRoute] Guid Id)
    {
        var command = new RentDvdCommand(Id);
        var response = await _mediator.Send(command, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        var @event = new DvdRentedEvent(Id.ToString(), response.UpdatedAt);
        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpPut("ReturnDvd/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> ReturnDvd([FromRoute] Guid Id)
    {
        var command = new ReturnDvdCommand(Id);
        var response = await _mediator.Send(command, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        var @event = new DvdReturnedEvent(Id.ToString(), response.UpdatedAt);
        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpDelete("DeleteDvd/{id:guid}")]
    [ProducesResponseType((int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteDvd([FromRoute] Guid Id)
    {
        var command = new DeleteDvdCommand(Id);
        var response = await _mediator.Send(command, HttpContext.RequestAborted);
        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false);

        var @event = new DvdDeletedEvent(Id.ToString(), response.DeletedAt);
        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, true);
    }
}
