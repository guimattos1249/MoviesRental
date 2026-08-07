using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Queries.Application.Contracts;

public interface IDirectorsQueryRepository : IQueryRepository<Director>
{
    Task<Director> GetByNameAsync(string name);
}
