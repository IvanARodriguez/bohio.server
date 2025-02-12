using Homespirations.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using NUlid;

namespace Homespirations.Infrastructure.Repositories;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<T>> GetAllAsync() => await _context.Set<T>().ToListAsync();

    public async Task<T?> GetByIdAsync(Ulid id) => await _context.Set<T>().FindAsync(id);

    public async Task AddAsync(T entity) => await _context.Set<T>().AddAsync(entity);

    public async Task DeleteAsync(Ulid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null) _context.Set<T>().Remove(entity);
    }
}
