using MediatR;

namespace MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;

public record CreateDvdCommand(
    string Title,
    int Genre,
    DateTime Published,
    int Copies,
    Guid DirectorId) : IRequest<CreateDvdResponse>;
