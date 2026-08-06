using MediatR;

namespace MoviesRental.Application.UseCases.Dvds.Commands.ReturnDvd;

public record ReturnDvdCommand(Guid Id) : IRequest<ReturnDvdResponse>;
