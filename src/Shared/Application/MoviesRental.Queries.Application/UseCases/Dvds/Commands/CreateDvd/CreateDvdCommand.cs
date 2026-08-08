using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.CreateDvd;

public record CreateDvdCommand(
    string Id,
    string Title,
    string Genre,
    DateTime Published,
    bool Available,
    int Copies,
    string DirectorId,
    DateTime UpdatedAt,
    DateTime CreatedAt) : IRequest<bool>;
