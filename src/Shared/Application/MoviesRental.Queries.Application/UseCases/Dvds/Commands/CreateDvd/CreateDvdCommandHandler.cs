using MediatR;
using MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Commands.CreateDvd;

public class CreateDvdCommandHandler(IDvdsQueryRepository repository, CreateDvdCommandValidator validator) : IRequestHandler<CreateDvdCommand, bool>
{
    private readonly IDvdsQueryRepository _repository = repository;
    private readonly CreateDvdCommandValidator _validator = validator;

    public async Task<bool> Handle(CreateDvdCommand request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
            return false;

        var dvd = await _repository.GetAsync(request.Id);
        if (dvd is not null)
            return false;

        dvd = new Dvd
        {
            Id = request.Id,
            Title = request.Title,
            Genre = request.Genre,
            Published = request.Published,
            Available = request.Available,
            Copies = request.Copies,
            DirectorId = request.DirectorId,
            CreatedAt = request.CreatedAt,
            UpdatedAt = request.UpdatedAt
        };

        var result = await _repository.CreateAsync(dvd);
        if (result is null)
            return false;

        return true;
    }
}
