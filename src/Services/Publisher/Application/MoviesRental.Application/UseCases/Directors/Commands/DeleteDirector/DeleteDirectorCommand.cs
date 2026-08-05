using MediatR;

namespace MoviesRental.Application.UseCases.Directors.Commands.DeleteDirector;

public record DeleteDirectorCommand(Guid Id) : IRequest<bool>;
