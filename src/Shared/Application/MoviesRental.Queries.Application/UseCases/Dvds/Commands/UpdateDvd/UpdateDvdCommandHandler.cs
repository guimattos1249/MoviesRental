using MediatR;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Application.UseCases.Dvds.Commands.UpdateDvd;
using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Application.UseCases.Dvds.Commands.UpdateDvd;

public class UpdateDvdCommandHandler(IDvdsQueryRepository repository, UpdateDvdCommandValidator validator) : IRequestHandler<UpdateDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository = repository;
    private readonly UpdateDvdCommandValidator _validator = validator;

    public async Task<bool> Handle(UpdateDvdCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return false;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is null)
            return false;

        dvd = new Dvd
        {
            Id = request.Id,
            Title = request.Title,
            Genre = request.Genre,
            Published = request.Published,
            Copies = request.Copies,
            DirectorId = request.DirectorId,
            UpdatedAt = request.UpdatedAt
        };

        return await _repository.UpdateAsync(dvd);
    }
}
;