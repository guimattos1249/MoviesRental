using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.DeleteDvd;

public record DeleteDvdCommand(string Id, DateTime DeletedAt) : IRequest<bool>;
