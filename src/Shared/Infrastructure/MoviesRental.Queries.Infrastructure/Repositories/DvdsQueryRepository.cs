using MongoDB.Driver;
using MoviesRental.Queries.Application.Contracts;
using MoviesRental.Queries.Domain.Models;
using MoviesRental.Queries.Infrastructure.Context;
using System.Xml.Linq;

namespace MoviesRental.Queries.Infrastructure.Repositories;

public class DvdsQueryRepository(IMoviesRentalReadContext context) : IDvdsQueryRepository
{
    private readonly IMoviesRentalReadContext _context = context;

    public async Task<Dvd> CreateAsync(Dvd entity)
    {
        await _context.Dvds.InsertOneAsync(entity);
        return entity;
    }

    public async Task<bool> DeleteAsync(string Id)
    {
        var result = await _context.Dvds.DeleteOneAsync(d => d.Id == Id);
        return result.IsAcknowledged && result.DeletedCount > 0;
    }

    public async Task<Dvd> GetAsync(string Id) =>
        await _context
            .Dvds
            .Find(d => d.Id == Id)
            .FirstOrDefaultAsync();

    public async Task<Dvd> GetByTitleAsync(string title) =>
        await _context
            .Dvds
            .Find(d => d.Title == title && d.Available)
            .FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(Dvd entity)
    {
        var result = await _context
            .Dvds
            .ReplaceOneAsync(d => d.Id == entity.Id, entity);

        return result.IsAcknowledged && result.ModifiedCount > 0;
    }
}
