using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.RentDvd;

public class RentDvdCommandHandler(IDvdsQueryRepository repository) : IRequestHandler<RentDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository = repository;

    public async Task<bool> Handle(RentDvdCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id) || request.UpdatedAt > DateTime.UtcNow)
            return false;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null || dvd is { Copies: 0 })
            return false;

        dvd.Copies -= 1;

        return await _repository.UpdateAsync(dvd);
    }
}
