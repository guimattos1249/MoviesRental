using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace MoviesRental.Application.UseCases.Dvds.Commands.CreateDvd;

public record CreateDvdCommand(
    string Title,
    int Genre,
    DateTime Published,
    int Copies,
    Guid DirectorId) : IRequest<CreateDvdResponse>;
