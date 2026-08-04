using MongoDB.Bson.Serialization.Attributes;

namespace MoviesRental.Queries.Domain.Models;

public class Director
{
    [BsonId]
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
}
