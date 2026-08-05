using MediatR;
using MoviesRental.Application.Contracts;

namespace MoviesRental.Application.UseCases.Directors.Commands.UpdateDirector;

public class UpdateDirectorCommandHandler(IDirectorsWriteRepository repository, UpdateDirectorCommandValidator validator) : IRequestHandler<UpdateDirectorCommand, UpdateDirectorResponse>
{
    private readonly IDirectorsWriteRepository _repository = repository;
    private readonly UpdateDirectorCommandValidator _validator = validator;

    public async Task<UpdateDirectorResponse> Handle(UpdateDirectorCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return default;

        var director = await _repository.GetAsync(request.Id);
        if (director is null)
            return default;

        director.UpdateName(request.Name);
        director.UpdateSurname(request.Surname);

        var result = await _repository.UpdateAsync(director);
        if (!result)
            return default;

        return new UpdateDirectorResponse(director.Id.ToString(), director.FullName(), director.UpdatedAt);
    }
}
