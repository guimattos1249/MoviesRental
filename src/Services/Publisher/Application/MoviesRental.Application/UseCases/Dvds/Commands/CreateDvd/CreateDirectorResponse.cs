namespace MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;

public record CreateDirectorResponse(string Id,
    string FullName,
    DateTime CreatedAt,
    DateTime UpdatedAt);
