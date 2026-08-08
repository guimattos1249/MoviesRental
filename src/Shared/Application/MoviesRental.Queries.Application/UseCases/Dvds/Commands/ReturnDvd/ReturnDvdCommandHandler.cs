using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.ReturnDvd;

public class ReturnDvdCommandHandler(IDvdsQueryRepository repository) : IRequestHandler<ReturnDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository = repository;

    public async Task<bool> Handle(ReturnDvdCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id) || request.UpdatedAt > DateTime.UtcNow)
            return false;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return false;

        dvd.Copies += 1;

        return await _repository.UpdateAsync(dvd);
    }
}
