namespace MoviesRental.Application.UseCases.Directors.Commands.CreateDirector;

public record CreateDirectorResponse(string Id,
    string FullName,
    DateTime CreatedAt,
    DateTime UpdatedAt);
