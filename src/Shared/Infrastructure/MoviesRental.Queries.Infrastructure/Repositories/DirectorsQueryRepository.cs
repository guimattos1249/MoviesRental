using MongoDB.Driver;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Domain.Models;
using MoviesRental.Queries.Infrastructure.Context;

namespace MoviesRental.Queries.Infrastructure.Repositories;

public class DirectorsQueryRepository(IMoviesRentalReadContext context) : IDirectorsQueryRepository
{
    private readonly IMoviesRentalReadContext _context = context;

    public async Task<Director> CreateAsync(Director entity)
    {
        await _context.Directors.InsertOneAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(string Id)
    {
        var result = await _context.Directors.DeleteOneAsync(d => d.Id == Id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<Director> GetAsync(string Id) =>
        await _context
            .Directors
            .Find(d => d.Id == Id)
            .FirstOrDefaultAsync();

    public async Task<Director> GetByNameAsync(string name) =>
        await _context
            .Directors
            .Find(d => d.FullName == name)
            .FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(Director entity)
    {
        var result = await _context
            .Directors
            .ReplaceOneAsync(d => d.Id == entity.Id, entity);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
