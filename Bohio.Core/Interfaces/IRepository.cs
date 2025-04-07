using NUlid;

namespace Bohio.Core.Interfaces;

public interface IRepository<T> where T : class
{
  Task<IEnumerable<T>> GetAllAsync();
  Task<T?> GetByIdAsync(Ulid id);
  Task AddAsync(T entity);
  Task DeleteAsync(Ulid id);
}
