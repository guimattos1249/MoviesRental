using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.UpdateDirector;

public record UpdateDirectorCommand(
    string Id,
    string FullName,
    DateTime UpdatedAt) : IRequest<bool>;

