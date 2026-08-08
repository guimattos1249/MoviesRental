using MediatR;

namespace MoviesRental.Queries.Application.UseCases.Dvds.Queries.GetDvd;

public record GetDvdQuery(string Title) : IRequest<GetDvdResponse>;
