namespace MoviesRental.Queries.Application.Contracts;

public interface IQueryRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<bool> UpdateAsync(T entity);
    Task<bool> DeleteAsync(string Id);
    Task<T> GetAsync(string Id);
}
