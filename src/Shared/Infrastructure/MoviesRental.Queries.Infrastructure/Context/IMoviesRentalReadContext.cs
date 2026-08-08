using MongoDB.Driver;
using MoviesRental.Queries.Domain.Models;

namespace MoviesRental.Queries.Infrastructure.Context;

public interface IMoviesRentalReadContext
{
    IMongoCollection<Dvd> Dvds { get; }
    IMongoCollection<Director> Directors { get; }
}
