using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Directors.Queries.GetDirector;

public record GetDirectorQuery(string FullName) : IRequest<GetDirectorResponse>;
