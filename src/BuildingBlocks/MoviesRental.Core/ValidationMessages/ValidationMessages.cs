namespace MoviesRental.Core.ValidationMessages;

public static class ValidationMessages
{
    public const string MIN_LENGTH_ERROR_MESSAGE = "The field {PropertyName} must be at least {MinLength} characters long.";
    public const string MAX_LENGTH_ERROR_MESSAGE = "The field {PropertyName} must be at most {MaxLength} characters long.";
    public const string NOT_EMPTY_ERROR_MESSAGE = "The field {PropertyName} cannot be empty.";
    public const string ERROR_MESSAGE = "Invalid {PropertyName}";
}
