using MediatR;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.CreateDirector;

public class CreateDirectorCommandHandler(IDirectorsQueryRepository repository, CreateDirectorCommandValidator validator) : IRequestHandler<CreateDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository = repository;
    private readonly CreateDirectorCommandValidator _validator = validator;

    public async Task<bool> Handle(CreateDirectorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return default;

        var director = await _repository.GetAsync(request.Id);
        if (director is not null)
            return false;

        director = new Director { Id = request.Id, FullName = request.FullName, CreatedAt = request.CreatedAt, UpdatedAt = request.UpdatedAt };
        var result = await _repository.CreateAsync(director);
        if (result is null)
            return default;

        return true;
    }
}
