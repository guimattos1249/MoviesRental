namespace MoviesRental.Core.EventBus.Events;

public record DirectorCreatedEvent(string Id, string FullName, DateTime CreatedAt, DateTime UpdatedAt);