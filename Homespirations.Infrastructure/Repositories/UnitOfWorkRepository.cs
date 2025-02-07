using Homespirations.Core.Entities;
using Homespirations.Core.Interfaces;
using Homespirations.Core.Repositories;

namespace Homespirations.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private IRepository<HomeSpace>? _homeSpaces;

    public IRepository<HomeSpace> HomeSpaces => _homeSpaces ??= new Repository<HomeSpace>(_context);

    IRepository<HomeSpace> IUnitOfWork.HomeSpaces => throw new NotImplementedException();

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
