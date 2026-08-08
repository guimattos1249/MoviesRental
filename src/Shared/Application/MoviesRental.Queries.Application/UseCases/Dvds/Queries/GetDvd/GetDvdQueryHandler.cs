using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Queries.GetDvd;

public class GetDvdQueryHandler(IDvdsQueryRepository repository) : IRequestHandler<GetDvdQuery, GetDvdResponse>
{
    private readonly IDvdsQueryRepository _repository = repository;

    public async Task<GetDvdResponse> Handle(GetDvdQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.Title))
            return default;

        var dvd = await _repository.GetByTitleAsync(request.Title);
        if (dvd is null)
            return default;

        return new GetDvdResponse(dvd.Id, 
            dvd.Title,
            dvd.Genre, 
            dvd.Published, 
            dvd.Copies, 
            dvd.DirectorId, 
            dvd.CreatedAt, 
            dvd.UpdatedAt);
    }
}
