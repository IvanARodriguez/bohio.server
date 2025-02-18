using AutoMapper;
using Homespirations.Core.Entities;
using Homespirations.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace Homespirations.Infrastructure.Repositories;

public class UnitOfWork(AppDbContext context) : IUnitOfWork
{
    private readonly AppDbContext _context = context;
    private IRepository<HomeSpace>? _homeSpaces;

    private IRepository<Media>? _media;
    private IHomeSpaceRepository _homeSpaceAndMedia = null!;

    public IRepository<HomeSpace> HomeSpaces => _homeSpaces ??= new Repository<HomeSpace>(_context);
    public IRepository<Media> Media => _media ??= new Repository<Media>(_context);

    public IHomeSpaceRepository HomeSpaceAndMedia => _homeSpaceAndMedia ??= new HomeSpaceRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
