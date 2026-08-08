using MediatR;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.DeleteDvd;

namespace MoviesRental.Application.UseCases.Dvds.Commands.DeleteDvd;

public class DeleteDvdHandler(IDvdsQueryRepository repository) : IRequestHandler<DeleteDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository = repository;

    public async Task<bool> Handle(DeleteDvdCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id) || request.DeletedAt > DateTime.UtcNow)
            return false;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return false;

        dvd.Available = false;
        dvd.DeletedAt = request.DeletedAt;
        dvd.Copies = 0;

        return await _repository.UpdateAsync(dvd);
    }
}
