namespace MoviesRental.Core.EventBus.Events;

public record DvdDeletedEvent(string Id, DateTime DeletedAt);
