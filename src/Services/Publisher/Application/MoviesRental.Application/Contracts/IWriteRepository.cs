using MoviesRental.Core.DomainObjects;

namespace MoviesRental.Application.Contracts;

public interface IWriteRepository<T> where T : Entity
{
    Task<bool> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(Guid Id);
    Task<T> GetAsync(Guid Id);
}
