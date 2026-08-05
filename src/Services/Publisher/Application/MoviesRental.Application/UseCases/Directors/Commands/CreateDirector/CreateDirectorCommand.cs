using MediatR;

namespace MoviesRental.Application.UseCases.Directors.Commands.CreateDirector;

public record CreateDirectorCommand(string Name, string Surname) : IRequest<CreateDirectorResponse>;