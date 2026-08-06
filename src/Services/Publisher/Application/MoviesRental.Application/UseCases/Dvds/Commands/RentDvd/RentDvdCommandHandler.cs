using MediatR;
using MoviesRental.Application.Contracts;

namespace MoviesRental.Application.UseCases.Dvds.Commands.RentDvd;

public class RentDvdCommandHandler(IDvdsWriteRepository repository) : IRequestHandler<RentDvdCommand, RentDvdResponse>
{
    private readonly IDvdsWriteRepository _repository = repository;

    public async Task<RentDvdResponse> Handle(RentDvdCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return default;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return default;

        dvd.RentCopy();

        var result = await _repository.UpdateAsync(dvd);
        if (!result)
            return default;

        return new RentDvdResponse(dvd.Id.ToString(), dvd.UpdatedAt);
    }
}
