using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandler(IDirectorsQueryRepository repository, UpdateDirectorCommandValidator validator) : IRequestHandler<UpdateDirectorCommand, bool>
{
    private readonly IDirectorsQueryRepository _repository = repository;
    private readonly UpdateDirectorCommandValidator _validator = validator;

    public async Task<bool> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return default;

        var director = await _repository.GetAsync(request.Id);
        if (director is null)
            return default;

        director.FullName = request.FullName;
        director.UpdatedAt = request.UpdatedAt;

        return await _repository.UpdateAsync(director);
    }
}
