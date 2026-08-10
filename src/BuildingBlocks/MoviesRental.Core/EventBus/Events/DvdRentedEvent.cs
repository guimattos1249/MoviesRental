namespace MoviesRental.Core.EventBus.Events;

public record DvdRentedEvent(string Id, DateTime UpdatedAt);
