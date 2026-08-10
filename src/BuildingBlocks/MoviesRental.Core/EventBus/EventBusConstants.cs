namespace MoviesRental.Core.EventBus;

public static class EventBusConstants
{
    public const string CREATED_DVD_QUEUE = "created-dvd-queue";
    public const string UPDATED_DVD_QUEUE = "updated-dvd-queue";
    public const string DELETED_DVD_QUEUE = "deleted-dvd-queue";
    public const string RENTED_DVD_QUEUE = "rented-dvd-queue";
    public const string RETURNED_DVD_QUEUE = "returned-dvd-queue";
    public const string CREATED_DIRECTOR_QUEUE = "created-director-queue";
    public const string DELETED_DIRECTOR_QUEUE = "deleted-director-queue";
    public const string UPDATED_DIRECTOR_QUEUE = "updated-director-queue";
}
