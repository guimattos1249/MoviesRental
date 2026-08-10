using MoviesRental.Queries.Application.UseCases.Dvds.Queries.GetDvd;

namespace MoviesRental.API.Cache;

public interface ICacheRepository
{
    Task<GetDvdResponse> GetAsync(string title);
    Task Update(GetDvdResponse response);
}
