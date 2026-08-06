using MediatR;

namespace MoviesRental.Application.UseCases.Dvds.Commands.RentDvd;

public record RentDvdCommand(Guid Id) : IRequest<RentDvdResponse>;
