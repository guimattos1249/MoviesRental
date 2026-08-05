using MediatR;

namespace MoviesRental.Application.UseCases.Directors.Commands.UpdateDirector;

public record UpdateDirectorCommand(Guid Id,
    string Name,
    string Surname) : IRequest<UpdateDirectorResponse>;

