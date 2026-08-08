using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Queries.Application.Contracts;

public interface IDvdsQueryRepository : IQueryRepository<Dvd>
{
    Task<Dvd> GetByTitleAsync(string title);
}
