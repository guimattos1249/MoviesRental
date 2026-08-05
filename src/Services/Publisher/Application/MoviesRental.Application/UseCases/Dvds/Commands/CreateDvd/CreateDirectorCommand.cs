using MediatR;

namespace MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;

public record CreateDirectorCommand(string Name, string Surname) : IRequest<CreateDirectorResponse>;