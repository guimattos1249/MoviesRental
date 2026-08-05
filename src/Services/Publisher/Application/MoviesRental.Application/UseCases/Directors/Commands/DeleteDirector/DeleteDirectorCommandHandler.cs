using MediatR;
using MoviesRental.Application.Contracts;

namespace MoviesRental.Application.UseCases.Directors.Commands.DeleteDirector;

public class DeleteDirectorCommandHandler : IRequestHandler<DeleteDirectorCommand, bool>
{
    private readonly IDirectorsWriteRepository _repository;

    public DeleteDirectorCommandHandler(IDirectorsWriteRepository repository)
    {
        _repository = repository;
    }

    public async Task<bool> Handle(DeleteDirectorCommand request, CancellationToken cancellationToken)
    {
        if (request.Id == Guid.Empty)
            return false;

        var director = await _repository.GetDirectorWithMoviesAsync(request.Id);
        if (director is null || director.Dvds.Any(x => x.Available))
            return false;

        return await _repository.DeleteAsync(director.Id);
    }
}
