using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using MoviesRental.Application.UseCases.Directors.Commands.CreateDirector;
using MoviesRental.Application.UseCases.Directors.Commands.DeleteDirector;
using MoviesRental.Application.UseCases.Directors.Commands.UpdateDirector;
using MoviesRental.Core;
using MoviesRental.Core.EventBus.Events;
using MoviesRental.Queries.Application.UseCases.Directors.Queries.GetDirector;
using System.Net;

namespace MoviesRental.API.Controllers;

public class DirectorController(IMediator mediator, IPublishEndpoint publishEndpoint) : ApiController
{
    private readonly IMediator _mediator = mediator;
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet("[action]/{fullName}", Name = "GetDirector")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDirector([FromRoute] string fullName)
    {
        var query = new GetDirectorQuery(fullName);
        var response = await _mediator.Send(query, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.NotFound, false, null);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpPost("CreateDirector")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> CreateDirector(
        [FromBody] CreateDirectorCommand command)
    {
        var response = await _mediator.Send(command, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false, null);

        var @event = new DirectorCreatedEvent(response.Id, response.FullName, response.CreatedAt, response.UpdatedAt);

        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.Created, true, response);
    }

    [HttpPut("UpdateDirector")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> UpdateDirector(
        [FromBody] UpdateDirectorCommand command)
    {
        var response = await _mediator.Send(command, HttpContext.RequestAborted);

        if (response is null)
            return CustomResponse((int)HttpStatusCode.BadRequest, false, null);

        var @event = new DirectorUpdatedEvent(response.Id, response.FullName, response.UpdatedAt);

        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, true, response);
    }

    [HttpDelete("DeleteDirector/{id:guid}")]
    [ProducesResponseType(typeof(BaseResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<IActionResult> DeleteDirector([FromRoute] Guid id)
    {
        var command = new DeleteDirectorCommand(id);
        var response = await _mediator.Send(command, HttpContext.RequestAborted);

        if (!response)
            return CustomResponse((int)HttpStatusCode.BadRequest, response);

        var @event = new DirectorDeletedEvent(id.ToString());

        await _publishEndpoint.Publish(@event);

        return CustomResponse((int)HttpStatusCode.OK, response);
    }
}
