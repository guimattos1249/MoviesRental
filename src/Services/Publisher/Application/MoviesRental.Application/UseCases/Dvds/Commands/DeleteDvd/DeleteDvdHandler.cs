using MediatR;
using MoviesRental.Application.Contracts;
using System.Runtime.InteropServices;

namespace MoviesRental.Application.UseCases.Dvds.Commands.DeleteDvd;

public class DeleteDvdHandler(IDvdsWriteRepository repository) : IRequestHandler<DeleteDvdCommand, DeleteDvdResponse>
{
    private readonly IDvdsWriteRepository _repository = repository;

    public async Task<DeleteDvdResponse> Handle(DeleteDvdCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return default;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return default;

        dvd.DeleteDvd();
        var result = await _repository.UpdateAsync(dvd);
        if (!result)
            return default;

        return new DeleteDvdResponse(dvd.Id.ToString(), (DateTime)dvd.DeletedAt!);
    }
}
