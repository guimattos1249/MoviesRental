using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.DeleteDirector;

public record DeleteDirectorCommand(string Id) : IRequest<bool>;
