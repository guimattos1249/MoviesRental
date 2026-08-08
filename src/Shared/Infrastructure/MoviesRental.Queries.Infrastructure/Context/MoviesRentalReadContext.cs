using MongoDB.Driver;
using MoviesRental.Queries.Domain.Models;
using MoviesRental.Queries.Infrastructure.Settings;

namespace MoviesRental.Queries.Infrastructure.Context;

public class MoviesRentalReadContext : IMoviesRentalReadContext
{
    public MoviesRentalReadContext(MongoDbSettings settings)
    {
        var client = new MongoClient(settings.ConnectionString);
        var database = client.GetDatabase(settings.DatabaseName);
        Directors = database.GetCollection<Director>(settings.DirectorsCollection);
        Dvds = database.GetCollection<Dvd>(settings.DirectorsCollection);
    }

    public IMongoCollection<Dvd> Dvds { get; }

    public IMongoCollection<Director> Directors { get; }
}
