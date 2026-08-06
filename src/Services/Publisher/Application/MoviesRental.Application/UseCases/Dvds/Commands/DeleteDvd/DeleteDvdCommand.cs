using MediatR;

namespace MoviesRental.Application.UseCases.Dvds.Commands.DeleteDvd;

public record DeleteDvdCommand(Guid Id) : IRequest<DeleteDvdResponse>;
