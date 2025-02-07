using Homespirations.Core.Interfaces;
using Homespirations.Core.Entities;
using NUlid;

namespace Homespirations.Application.Services;

public class HomeSpaceService(IRepository<HomeSpace> repo)
{
  private readonly IRepository<HomeSpace> _repo = repo;

  public async Task<IEnumerable<HomeSpace>> GetAllAsync() => await _repo.GetAllAsync();
  public async Task<HomeSpace?> GetByIdAsync(Ulid id) => await _repo.GetByIdAsync(id);
  public async Task AddAsync(HomeSpace homeSpace) => await _repo.AddAsync(homeSpace);
  public async Task DeleteAsync(Ulid id) => await _repo.DeleteAsync(id);

}