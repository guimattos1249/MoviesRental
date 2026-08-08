using MongoDB.Bson.Serialization.Attributes;

namespace MoviesRental.Queries.Domain.Models;

public class Dvd
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public DateTime Published { get; set; }
    public bool Available { get; set; }
    public int Copies { get; set; }
    public string DirectorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}
