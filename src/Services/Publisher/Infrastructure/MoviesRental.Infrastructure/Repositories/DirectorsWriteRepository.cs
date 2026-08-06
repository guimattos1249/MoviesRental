using MoviesRental.Application.Contracts;
using MoviesRental.Domain.Entities;

namespace MoviesRental.Infrastructure.Repositories;

public class DirectorsWriteRepository(MoviesRentalWriteContext context) : IDirectorsWriteRepository
{
    private readonly MoviesRentalWriteContext _context = context;

    public async Task<bool> CreateAsync(Director entity)
    {
        await _context.Directors.AddAsync(entity);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(Guid Id)
    {
        await _context.Directors.Where(d => d.Id == Id).ExecuteDeleteAsync();
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<Director> GetAsync(Guid Id) => await _context.DirectorsFindAsync(Id);

    public async Task<Director> GetDirectorWithMoviesAsync(Guid Id) => await _context
        .Directors
        .AsNoTracking()
        .Include(d => d.Dvds)
        .Where(d => d.Id == Id)
        .FirstOrDefaultAsync();

    public async Task<bool> UpdateAsync(Director entity)
    {
        _context.Directors.Update(entity);
        return await _context.SaveChangesAsync() > 0;
    }
}
