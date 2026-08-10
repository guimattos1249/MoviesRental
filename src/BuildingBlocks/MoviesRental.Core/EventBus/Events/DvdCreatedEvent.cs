namespace MoviesRental.Core.EventBus.Events;

public record DvdCreatedEvent(string Id,
    string Title,
    string Genre,
    DateTime Published,
    bool Abailable,
    int Copies,
    string DirectorId,
    DateTime CreatedAt,
    DateTime UpdatedAt);
