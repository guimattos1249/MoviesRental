using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.ReturnDvd;

public record ReturnDvdCommand(string Id, DateTime UpdatedAt) : IRequest<bool>;
