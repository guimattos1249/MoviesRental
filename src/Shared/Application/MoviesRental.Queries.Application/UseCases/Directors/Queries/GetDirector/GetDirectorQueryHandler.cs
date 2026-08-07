using MediatR;
using MoviesRental.Queries.Application.Contracts;

namespace MoviesRental.Queries.Application.UseCases.Directors.Queries.GetDirector;

public class GetDirectorQueryHandler(IDirectorsQueryRepository repository) : IRequestHandler<GetDirectorQuery, GetDirectorResponse>
{
    private readonly IDirectorsQueryRepository _repository = repository;

    public async Task<GetDirectorResponse> Handle(GetDirectorQuery request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.FullName))
            return default;

        var director = await _repository.GetByNameAsync(request.FullName);
        if (director is null)
            return default;

        return new GetDirectorResponse(director.Id, director.FullName);
    }
}
