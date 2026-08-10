namespace MoviesRental.Core.EventBus.Events;

public record DvdReturnedEvent(string Id, DateTime UpdatedAt);
