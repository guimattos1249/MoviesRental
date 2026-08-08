using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.RentDvd;

public record RentDvdCommand(string Id, DateTime UpdatedAt) : IRequest<bool>;
