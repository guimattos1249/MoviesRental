using MediatR;
using MoviesRental.Application.Contracts;

namespace MoviesRental.Application.UseCases.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandler(IDvdsWriteRepository repository, UpdateDvdCommandValidator validator) : IRequestHandler<UpdateDvdCommand, UpdateDvdResponse>
{
    private readonly IDvdsWriteRepository _repository = repository;
    private readonly UpdateDvdCommandValidator _validator = validator;

    public async Task<UpdateDvdResponse> Handle(UpdateDvdCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return default;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return default;

        dvd.UpdateTitle(request.Title);
        dvd.UpdateCopies(request.Copies);
        dvd.UpdatePublishedDate(request.Published);
        dvd.UpdateGenre(request.Genre);
        dvd.UpdateDirector(request.DirectorId);

        var result = await _repository.UpdateAsync(dvd);
        if (!result)
            return default;

        return new UpdateDvdResponse(dvd.Id.ToString(),
            dvd.Title,
            dvd.Genre.ToString(),
            dvd.Published,
            dvd.Copies,
            dvd.DirectorId.ToString(),
            dvd.UpdatedAt);
    }
}
