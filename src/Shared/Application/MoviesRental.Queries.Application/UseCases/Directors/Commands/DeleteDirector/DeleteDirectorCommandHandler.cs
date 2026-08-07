using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.DeleteDirector;

public class DeleteDirectorCommandHandler(IDirectorsQueryRepository repository) : IRequestHandler<DeleteDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository = repository;

    public async Task<bool> Handle(DeleteDirectorCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Id))
            return false;

        var director = await _repository.GetAsync(request.Id);
        if (director is null)
            return false;

        return await _repository.DeleteAsync(director.Id);
    }
}
