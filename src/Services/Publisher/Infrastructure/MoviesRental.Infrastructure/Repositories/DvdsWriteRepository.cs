using Microsoft.EntityFrameworkCore;
using MoviesRental.Application.Contracts;
using MoviesRental.Domain.Entities;
using MoviesRental.Infrastructure.Context;

namespace MoviesRental.Infrastructure.Repositories;

public class DvdsWriteRepository(MoviesRentalWriteContext context) : IDvdsWriteRepository
{
    private readonly MoviesRentalWriteContext _context = context;

    public async Task<bool> CreateAsync(Dvd entity)
    {
        await _context.Dvds.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid Id)
    {
        await _context.Dvds
            .Where(d => d.Id == Id)
            .ExecuteDeleteAsync();
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Dvd> GetAsync(Guid Id) => await _context.Dvds.FindAsync(Id);

    public async Task<bool> UpdateAsync(Dvd entity)
    {
        _context.Dvds.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}
