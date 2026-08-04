using MongoDB.Bson.Serialization.Attributes;

namespace MoviesRental.Queries.Domain.Models;

public class Dvd
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string Title { get; private set; } = string.Empty;
    public string Genre { get; private set; } = string.Empty;
    public DateTime Published { get; private set; }
    public bool Available { get; private set; }
    public int Copies { get; private set; }
    public string DirectorId { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}
